Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq

Namespace DXGrid.ColumnSync
	Public Class ViewModel
		Public Sub New()
			Data = New ObservableCollection(Of TestData)()
			GenerateData(20)
		End Sub

		Private Sub GenerateData(ByVal count As Integer)
			For i As Integer = 0 To count - 1
				Data.Add(New TestData(i))
			Next i
		End Sub

		Private privateData As ObservableCollection(Of TestData)
		Public Property Data() As ObservableCollection(Of TestData)
			Get
				Return privateData
			End Get
			Private Set(ByVal value As ObservableCollection(Of TestData))
				privateData = value
			End Set
		End Property
	End Class

	Public Class TestData
		Public Sub New()
		End Sub

		Public Sub New(ByVal seed As Integer)
			IntValue = seed
			Text = "Text " & seed
			DateValue = DateTime.Now.AddDays(-seed)
			SecondText = "Second Text " & seed
			Details = New ObservableCollection(Of DetailData)()

			Dim detailCount As Integer = (New Random()).Next(5, 10)
			For i As Integer = 0 To detailCount - 1
				Details.Add(New DetailData(i + 100 + seed))
			Next i
		End Sub

		Public Property IntValue() As Integer
		Public Property Text() As String
		Public Property DateValue() As DateTime
		Public Property SecondText() As String
		Private privateDetails As ObservableCollection(Of DetailData)
		Public Property Details() As ObservableCollection(Of DetailData)
			Get
				Return privateDetails
			End Get
			Private Set(ByVal value As ObservableCollection(Of DetailData))
				privateDetails = value
			End Set
		End Property
	End Class

	Public Class DetailData
		Public Sub New(ByVal seed As Integer)
			DetailIntValue = seed
			DetailText = "Detail Text " & seed
			DetailDate = DateTime.Now.Date.AddDays(-seed)
			DetailBool = (New Random()).Next(0, 99) > 50
			DetailSecondText = "Detail Second Text " & seed
		End Sub

		Public Property DetailIntValue() As Integer
		Public Property DetailText() As String
		Public Property DetailDate() As DateTime
		Public Property DetailBool() As Boolean
		Public Property DetailSecondText() As String
	End Class
End Namespace
