Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq

Namespace vb_DXGrid.ColumnSync
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

			Dim detailCount As Integer = New Random().Next(5, 10)
			For i As Integer = 0 To detailCount - 1
				Details.Add(New DetailData(i + 100 + seed))
			Next i
		End Sub

		Private privateIntValue As Integer
		Public Property IntValue() As Integer
			Get
				Return privateIntValue
			End Get
			Set(ByVal value As Integer)
				privateIntValue = value
			End Set
		End Property
		Private privateText As String
		Public Property Text() As String
			Get
				Return privateText
			End Get
			Set(ByVal value As String)
				privateText = value
			End Set
		End Property
		Private privateDateValue As DateTime
		Public Property DateValue() As DateTime
			Get
				Return privateDateValue
			End Get
			Set(ByVal value As DateTime)
				privateDateValue = value
			End Set
		End Property
		Private privateSecondText As String
		Public Property SecondText() As String
			Get
				Return privateSecondText
			End Get
			Set(ByVal value As String)
				privateSecondText = value
			End Set
		End Property
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
			DetailBool = New Random().Next(0, 99) > 50
			DetailSecondText = "Detail Second Text " & seed
		End Sub

		Private privateDetailIntValue As Integer
		Public Property DetailIntValue() As Integer
			Get
				Return privateDetailIntValue
			End Get
			Set(ByVal value As Integer)
				privateDetailIntValue = value
			End Set
		End Property
		Private privateDetailText As String
		Public Property DetailText() As String
			Get
				Return privateDetailText
			End Get
			Set(ByVal value As String)
				privateDetailText = value
			End Set
		End Property
		Private privateDetailDate As DateTime
		Public Property DetailDate() As DateTime
			Get
				Return privateDetailDate
			End Get
			Set(ByVal value As DateTime)
				privateDetailDate = value
			End Set
		End Property
		Private privateDetailBool As Boolean
		Public Property DetailBool() As Boolean
			Get
				Return privateDetailBool
			End Get
			Set(ByVal value As Boolean)
				privateDetailBool = value
			End Set
		End Property
		Private privateDetailSecondText As String
		Public Property DetailSecondText() As String
			Get
				Return privateDetailSecondText
			End Get
			Set(ByVal value As String)
				privateDetailSecondText = value
			End Set
		End Property
	End Class
End Namespace
