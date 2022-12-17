Imports System.Windows
Imports System.Windows.Controls

Namespace DXGrid.ColumnSync

    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Public Partial Class MainWindow
        Inherits Window

        Public Sub New()
            DataContext = New ViewModel()
            Me.InitializeComponent()
        End Sub
    End Class
End Namespace
