Imports System
Imports System.Collections.ObjectModel

Namespace DXGrid.ColumnSync

    Public Class ViewModel

        Private _Data As ObservableCollection(Of DXGrid.ColumnSync.TestData)

        Public Sub New()
            Data = New ObservableCollection(Of TestData)()
            GenerateData(20)
        End Sub

        Private Sub GenerateData(ByVal count As Integer)
            For i As Integer = 0 To count - 1
                Data.Add(New TestData(i))
            Next
        End Sub

        Public Property Data As ObservableCollection(Of TestData)
            Get
                Return _Data
            End Get

            Private Set(ByVal value As ObservableCollection(Of TestData))
                _Data = value
            End Set
        End Property
    End Class

    Public Class TestData

        Private _Details As ObservableCollection(Of DXGrid.ColumnSync.DetailData)

        Public Sub New()
        End Sub

        Public Sub New(ByVal seed As Integer)
            IntValue = seed
            Text = "Text " & seed
            DateValue = Date.Now.AddDays(-seed)
            SecondText = "Second Text " & seed
            Details = New ObservableCollection(Of DetailData)()
            Dim detailCount As Integer = New Random().Next(5, 10)
            For i As Integer = 0 To detailCount - 1
                Details.Add(New DetailData(i + 100 + seed))
            Next
        End Sub

        Public Property IntValue As Integer

        Public Property Text As String

        Public Property DateValue As Date

        Public Property SecondText As String

        Public Property Details As ObservableCollection(Of DetailData)
            Get
                Return _Details
            End Get

            Private Set(ByVal value As ObservableCollection(Of DetailData))
                _Details = value
            End Set
        End Property
    End Class

    Public Class DetailData

        Public Sub New(ByVal seed As Integer)
            DetailIntValue = seed
            DetailText = "Detail Text " & seed
            DetailDate = Date.Now.Date.AddDays(-seed)
            DetailBool = New Random().Next(0, 99) > 50
            DetailSecondText = "Detail Second Text " & seed
        End Sub

        Public Property DetailIntValue As Integer

        Public Property DetailText As String

        Public Property DetailDate As Date

        Public Property DetailBool As Boolean

        Public Property DetailSecondText As String
    End Class
End Namespace
