Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Globalization
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Interactivity
Imports System.Windows.Threading
Imports DevExpress.Utils
Imports DevExpress.Xpf.Core.Native
Imports DevExpress.Xpf.Grid

Namespace vb_DXGrid.ColumnSync
    Public Class DetailSyncBehavior
        Inherits Behavior(Of GridControl)
        Private onAttached_Renamed As Integer = 0
        Private widthAdjustmentConverter As New WidthAdjustmentConverter()
        Private widthAdjustmentValue As Double = 0.0F

        Private masterColumns As New Dictionary(Of Integer, GridColumn)()
        Private detailColumns As New Dictionary(Of Integer, GridColumn)()

        Private Sub SetColumnBinding(ByVal masterCol As GridColumn, ByVal detailCol As GridColumn)
            DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).AddValueChanged(masterCol, AddressOf OnMasterColumnVisibleChanged)
            DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).AddValueChanged(detailCol, AddressOf OnDetailColumnVisibleChanged)
            detailCol.AllowResizing = DefaultBoolean.False

            If detailCol.VisibleIndex = 0 Then
                detailCol.SetBinding(GridColumn.WidthProperty, New Binding("ActualDataWidth") With {.Source = masterCol, .Converter = widthAdjustmentConverter, .ConverterParameter = widthAdjustmentValue})
            Else
                detailCol.SetBinding(GridColumn.WidthProperty, New Binding("ActualDataWidth") With {.Source = masterCol})
            End If

            masterColumns(masterCol.VisibleIndex) = masterCol
            detailColumns(detailCol.VisibleIndex) = detailCol
        End Sub

        Private Sub RemoveColumnBinding(ByVal masterCol As GridColumn, ByVal detailCol As GridColumn)
            If masterCol IsNot Nothing Then
                DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(masterCol, AddressOf OnMasterColumnVisibleChanged)
            End If
            DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(detailCol, AddressOf OnDetailColumnVisibleChanged)
            BindingOperations.ClearBinding(detailCol, GridColumn.WidthProperty)
            detailCol.AllowResizing = DefaultBoolean.Default
        End Sub

        Private Sub SetWidthBindings()
            Dim masterView As TableView = TryCast(MasterGrid.View, TableView)
            Dim detailView As TableView = TryCast(DetailGrid.View, TableView)

            For Each detailCol As GridColumn In detailView.VisibleColumns
                If detailCol.VisibleIndex < 0 OrElse detailCol.VisibleIndex >= masterView.VisibleColumns.Count Then
                    Continue For
                End If

                Dim masterCol As GridColumn = masterView.VisibleColumns(detailCol.VisibleIndex)
                SetColumnBinding(masterCol, detailCol)
            Next detailCol
        End Sub

        Private Sub RemoveWidthBindings()
            Dim detailView As TableView = TryCast(DetailGrid.View, TableView)
            For Each detailCol As GridColumn In detailView.VisibleColumns
                RemoveColumnBinding(Nothing, detailCol)
            Next detailCol

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
                DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(storedMasterColumn, AddressOf OnMasterColumnVisibleChanged)
                Return True
            End If

            If storedDetailColumn Is Nothing Then
                Return True
            End If

            If masterCol.GetHashCode() <> storedMasterColumn.GetHashCode() OrElse detailCol.GetHashCode() <> storedDetailColumn.GetHashCode() Then
                DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(storedMasterColumn, AddressOf OnMasterColumnVisibleChanged)
                DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(storedDetailColumn, AddressOf OnDetailColumnVisibleChanged)
                Return True
            End If

            Return False
        End Function

        Private Sub ResetWidthBindings()
            Dim masterView As TableView = TryCast(MasterGrid.View, TableView)
            Dim detailView As TableView = TryCast(DetailGrid.View, TableView)

            For Each detailCol As GridColumn In detailView.VisibleColumns
                Dim masterCol As GridColumn = Nothing
                If detailCol.VisibleIndex >= 0 AndAlso detailCol.VisibleIndex < masterView.VisibleColumns.Count Then
                    masterCol = masterView.VisibleColumns(detailCol.VisibleIndex)
                End If

                If (Not CheckNeedReset(masterCol, detailCol)) Then
                    Continue For
                End If

                RemoveColumnBinding(masterCol, detailCol)
                If masterCol IsNot Nothing Then
                    SetColumnBinding(masterCol, detailCol)
                End If
            Next detailCol
        End Sub

        Private Sub OnMasterColumnVisibleChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim masterColumn As GridColumn = CType(sender, GridColumn)
            If (Not masterColumn.Visible) Then
                Dispatcher.BeginInvoke(New Action(Function() AnonymousMethod1(masterColumn)), DispatcherPriority.Render)
            End If
        End Sub

        Private Function AnonymousMethod1(ByVal masterColumn As GridColumn) As Boolean
            DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(masterColumn, AddressOf OnMasterColumnVisibleChanged)
            ResetWidthBindings()
            Return True
        End Function

        Private Sub MasterGridColumns_CollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
            If onAttached_Renamed <> 0 Then
                Return
            End If

            If e.Action = NotifyCollectionChangedAction.Reset Then
                ResetWidthBindings()
            End If
        End Sub

        Private Sub OnDetailColumnVisibleChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim detailColumn As GridColumn = CType(sender, GridColumn)
            If (Not detailColumn.Visible) Then
                Dispatcher.BeginInvoke(New Action(Function() AnonymousMethod2(detailColumn)), DispatcherPriority.Render)
            End If
        End Sub

        Private Function AnonymousMethod2(ByVal detailCol As GridColumn) As Boolean
            DependencyPropertyDescriptor.FromProperty(GridColumn.VisibleProperty, GetType(GridColumn)).RemoveValueChanged(detailCol, AddressOf OnDetailColumnVisibleChanged)
            BindingOperations.ClearBinding(detailCol, GridColumn.WidthProperty)
            ResetWidthBindings()
            Return True
        End Function

        Private Sub DetailGrid_ItemsSourceChanged(ByVal sender As Object, ByVal e As ItemsSourceChangedEventArgs)
            If onAttached_Renamed = 0 Then
                ResetWidthBindings()
            End If
        End Sub

        Private Sub DetailGridColumns_CollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
            If onAttached_Renamed <> 0 Then
                Return
            End If

            If e.Action = NotifyCollectionChangedAction.Reset Then
                ResetWidthBindings()
            End If
        End Sub

        Protected Overrides Sub OnAttached()
            MyBase.OnAttached()
            DetailGrid.Dispatcher.BeginInvoke(New Action(Function() AnonymousMethod3()), DispatcherPriority.Render)
        End Sub

        Private Function AnonymousMethod3() As Boolean
            onAttached_Renamed += 1
            MasterGrid = LayoutHelper.FindParentObject(Of GridControl)(DetailGrid.TemplatedParent)
            widthAdjustmentValue = (TryCast(MasterGrid.View, TableView)).ExpandDetailButtonWidth
            Dim cntnt As FrameworkElement = LayoutHelper.FindElement(MasterGrid, Function(element) TypeOf element Is ContentPresenter AndAlso element.Name = "content")
            If cntnt IsNot Nothing Then
                widthAdjustmentValue += cntnt.Margin.Left
            End If
            SetWidthBindings()
            AddHandler DetailGrid.ItemsSourceChanged, AddressOf DetailGrid_ItemsSourceChanged
            AddHandler DetailGrid.Columns.CollectionChanged, AddressOf DetailGridColumns_CollectionChanged
            AddHandler MasterGrid.Columns.CollectionChanged, AddressOf MasterGridColumns_CollectionChanged
            onAttached_Renamed -= 1
            Return True
		End Function

        Protected Overrides Sub OnDetaching()
            RemoveHandler DetailGrid.ItemsSourceChanged, AddressOf DetailGrid_ItemsSourceChanged
            RemoveHandler DetailGrid.Columns.CollectionChanged, AddressOf DetailGridColumns_CollectionChanged
            RemoveWidthBindings()
            RemoveHandler MasterGrid.Columns.CollectionChanged, AddressOf MasterGridColumns_CollectionChanged
            MyBase.OnDetaching()
        End Sub

        Public ReadOnly Property DetailGrid() As GridControl
            Get
                Return AssociatedObject
            End Get
        End Property
        Private privateMasterGrid As GridControl
        Public Property MasterGrid() As GridControl
            Get
                Return privateMasterGrid
            End Get
            Set(ByVal value As GridControl)
                privateMasterGrid = value
            End Set
        End Property
    End Class

    Public Class WidthAdjustmentConverter
        Implements IValueConverter
        Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
            If Not (TypeOf value Is Double) OrElse Not (TypeOf parameter Is Double) Then
                Return value
            End If

            Dim doubleValue As Double = CDbl(value)
            Dim doubleParameter As Double = CDbl(parameter)

            If Double.IsNaN(doubleValue) OrElse Double.IsNaN(doubleParameter) Then
                Return doubleValue
            End If

            Return doubleValue - doubleParameter
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
