Imports System.Windows.Forms

Public Class DlgNewPassword
    Public ChangePassword As Boolean
    Public ResourceName As String
    Public ResourceUsername As String
    Public ResourceURL As String
    Public ResourcePassword As String
    Public ResourceDescription As String

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        ResourceName = Txt_Name.Text
        ResourceUsername = Txt_Username.Text
        ResourceURL = Txt_URL.Text
        ResourcePassword = Txt_Password.Text
        ResourceDescription = Txt_Description.Text

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        ResourceName = ""
        ResourceUsername = ""
        ResourceURL = ""
        ResourcePassword = ""
        ResourceDescription = ""

        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub DlgNewPassword_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If ChangePassword Then
            Txt_Name.Text = ResourceName
            Txt_Username.Text = ResourceUsername
            Txt_URL.Text = ResourceURL
            Txt_Password.Text = ResourcePassword
            Txt_Description.Text = ResourceDescription
        End If
    End Sub
End Class
