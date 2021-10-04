Imports System
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Interactivity
Imports System.Windows.Threading
Imports DevExpress.Utils
Imports DevExpress.Xpf.Core.Native
Imports DevExpress.Xpf.Grid

Namespace DXGrid.ColumnSync

    Public Class DetailSyncBehavior
        Inherits Behavior(Of GridControl)

        Private onAttachedField As Integer = 0

        Private widthAdjustmentConverter As WidthAdjustmentConverter = New WidthAdjustmentConverter()

        Private widthAdjustmentValue As Double = 0.0F

        Private masterColumns As Dictionary(Of Integer, GridColumn) = New Dictionary(Of Integer, GridColumn)()

        Private detailColumns As Dictionary(Of Integer, GridColumn) = New Dictionary(Of Integer, GridColumn)()

        Private Sub SetColumnBinding(ByVal masterCol As GridColumn, ByVal detailCol As GridColumn)
            Call DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).AddValueChanged(masterCol, New EventHandler(AddressOf OnMasterColumnVisibleChanged))
            Call DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).AddValueChanged(detailCol, New EventHandler(AddressOf OnDetailColumnVisibleChanged))
            detailCol.AllowResizing = DefaultBoolean.[False]
            If detailCol.VisibleIndex = 0 Then
                detailCol.SetBinding(GridColumn.WidthProperty, New Binding("ActualDataWidth") With {.Source = masterCol, .Converter = widthAdjustmentConverter, .ConverterParameter = widthAdjustmentValue})
            Else
                detailCol.SetBinding(GridColumn.WidthProperty, New Binding("ActualDataWidth") With {.Source = masterCol})
            End If

            masterColumns(masterCol.VisibleIndex) = masterCol
            detailColumns(detailCol.VisibleIndex) = detailCol
        End Sub

        Private Sub RemoveColumnBinding(ByVal masterCol As GridColumn, ByVal detailCol As GridColumn)
            If masterCol IsNot Nothing Then Call DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(masterCol, New EventHandler(AddressOf OnMasterColumnVisibleChanged))
            Call DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(detailCol, New EventHandler(AddressOf OnDetailColumnVisibleChanged))
            BindingOperations.ClearBinding(detailCol, GridColumn.WidthProperty)
            detailCol.AllowResizing = DefaultBoolean.[Default]
        End Sub

        Private Sub SetWidthBindings()
            Dim masterView As TableView = TryCast(MasterGrid.View, TableView)
            Dim detailView As TableView = TryCast(DetailGrid.View, TableView)
            For Each detailCol As GridColumn In detailView.VisibleColumns
                If detailCol.VisibleIndex < 0 OrElse detailCol.VisibleIndex >= masterView.VisibleColumns.Count Then Continue For
                Dim masterCol As GridColumn = masterView.VisibleColumns(detailCol.VisibleIndex)
                Me.SetColumnBinding(masterCol, detailCol)
            Next
        End Sub

        Private Sub RemoveWidthBindings()
            Dim detailView As TableView = TryCast(DetailGrid.View, TableView)
            For Each detailCol As GridColumn In detailView.VisibleColumns
                Me.RemoveColumnBinding(Nothing, detailCol)
            Next

            masterColumns.Clear()
            detailColumns.Clear()
        End Sub

        Private Function CheckNeedReset(ByVal masterCol As GridColumn, ByVal detailCol As GridColumn) As Boolean
            Dim key As Integer = detailCol.VisibleIndex
            Dim storedMasterColumn As GridColumn = Nothing
            Dim storedDetailColumn As GridColumn = Nothing
            masterColumns.TryGetValue(key, storedMasterColumn)
            detailColumns.TryGetValue(key, storedDetailColumn)
            If masterCol Is Nothing AndAlso storedMasterColumn IsNot Nothing Then
                Call DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(storedMasterColumn, New EventHandler(AddressOf OnMasterColumnVisibleChanged))
                Return True
            End If

            If storedDetailColumn Is Nothing Then Return True
            If masterCol.GetHashCode() IsNot storedMasterColumn.GetHashCode() OrElse detailCol.GetHashCode() IsNot storedDetailColumn.GetHashCode() Then
                Call DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(storedMasterColumn, New EventHandler(AddressOf OnMasterColumnVisibleChanged))
                Call DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(storedDetailColumn, New EventHandler(AddressOf OnDetailColumnVisibleChanged))
                Return True
            End If

            Return False
        End Function

        Private Sub ResetWidthBindings()
            Dim masterView As TableView = TryCast(MasterGrid.View, TableView)
            Dim detailView As TableView = TryCast(DetailGrid.View, TableView)
            For Each detailCol As GridColumn In detailView.VisibleColumns
                Dim masterCol As GridColumn = Nothing
                If detailCol.VisibleIndex >= 0 AndAlso detailCol.VisibleIndex < masterView.VisibleColumns.Count Then masterCol = masterView.VisibleColumns(detailCol.VisibleIndex)
                If Not Me.CheckNeedReset(masterCol, detailCol) Then Continue For
                Me.RemoveColumnBinding(masterCol, detailCol)
                If masterCol IsNot Nothing Then Me.SetColumnBinding(masterCol, detailCol)
            Next
        End Sub

        Private Sub OnMasterColumnVisibleChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim masterColumn As GridColumn = CType(sender, GridColumn)
            If Not masterColumn.Visible Then
                Dispatcher.BeginInvoke(New Action(Sub()
                    Call DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(masterColumn, New EventHandler(AddressOf OnMasterColumnVisibleChanged))
                    ResetWidthBindings()
                End Sub), DispatcherPriority.Render)
            End If
        End Sub

        Private Sub MasterGridColumns_CollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
            If onAttachedField <> 0 Then Return
            If e.Action = NotifyCollectionChangedAction.Reset Then ResetWidthBindings()
        End Sub

        Private Sub OnDetailColumnVisibleChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim detailColumn As GridColumn = CType(sender, GridColumn)
            If Not detailColumn.Visible Then
                Dispatcher.BeginInvoke(New Action(Sub()
                    Call DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(detailColumn, New EventHandler(AddressOf OnDetailColumnVisibleChanged))
                    BindingOperations.ClearBinding(detailColumn, GridColumn.WidthProperty)
                    ResetWidthBindings()
                End Sub), DispatcherPriority.Render)
            End If
        End Sub

        Private Sub DetailGrid_ItemsSourceChanged(ByVal sender As Object, ByVal e As ItemsSourceChangedEventArgs)
            If onAttachedField = 0 Then ResetWidthBindings()
        End Sub

        Private Sub DetailGridColumns_CollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
            If onAttachedField <> 0 Then Return
            If e.Action = NotifyCollectionChangedAction.Reset Then ResetWidthBindings()
        End Sub

        Protected Overrides Sub OnAttached()
            MyBase.OnAttached()
            DetailGrid.Dispatcher.BeginInvoke(New Action(Sub()
                onAttachedField += 1
                If True Then
                    MasterGrid = LayoutHelper.FindParentObject(Of GridControl)(DetailGrid.TemplatedParent)
                    widthAdjustmentValue = TryCast(MasterGrid.View, TableView).ExpandDetailButtonWidth
                    Dim content As FrameworkElement = LayoutHelper.FindElement(MasterGrid, Function(element) TypeOf element Is ContentPresenter AndAlso element.Name Is "content")
                    If content IsNot Nothing Then widthAdjustmentValue += content.Margin.Left
                    SetWidthBindings()
                    Me.DetailGrid.ItemsSourceChanged += AddressOf DetailGrid_ItemsSourceChanged
                    Me.DetailGrid.Columns.CollectionChanged += AddressOf DetailGridColumns_CollectionChanged
                    Me.MasterGrid.Columns.CollectionChanged += AddressOf MasterGridColumns_CollectionChanged
                End If

                onAttachedField -= 1
            End Sub), DispatcherPriority.Render)
        End Sub

        Protected Overrides Sub OnDetaching()
            Me.DetailGrid.ItemsSourceChanged -= AddressOf DetailGrid_ItemsSourceChanged
            Me.DetailGrid.Columns.CollectionChanged -= AddressOf DetailGridColumns_CollectionChanged
            RemoveWidthBindings()
            Me.MasterGrid.Columns.CollectionChanged -= AddressOf MasterGridColumns_CollectionChanged
            MyBase.OnDetaching()
        End Sub

        Public ReadOnly Property DetailGrid As GridControl
            Get
                Return AssociatedObject
            End Get
        End Property

        Public Property MasterGrid As GridControl
    End Class

    Public Class WidthAdjustmentConverter
        Implements IValueConverter

        Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
            If Not(TypeOf value Is Double) OrElse Not(TypeOf parameter Is Double) Then Return value
            Dim doubleValue As Double = CDbl(value)
            Dim doubleParameter As Double = CDbl(parameter)
            If Double.IsNaN(doubleValue) OrElse Double.IsNaN(doubleParameter) Then Return doubleValue
            Return doubleValue - doubleParameter
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
