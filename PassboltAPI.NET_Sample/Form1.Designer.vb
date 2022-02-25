<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Btn_Login = New System.Windows.Forms.Button()
        Me.Txt_Status = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Txt_ServerURL = New System.Windows.Forms.TextBox()
        Me.Btn_GetUsers = New System.Windows.Forms.Button()
        Me.Btn_GetPasswords = New System.Windows.Forms.Button()
        Me.Txt_ServerFingerprint = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Txt_UserFingerprint = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Lb_Users = New System.Windows.Forms.ListBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Lb_Passwords = New System.Windows.Forms.ListBox()
        Me.Btn_SharePassword = New System.Windows.Forms.Button()
        Me.Btn_RemoveShare = New System.Windows.Forms.Button()
        Me.Btn_AddPassword = New System.Windows.Forms.Button()
        Me.Btn_ChangePassword = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Btn_ClearStatus = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Lbl_PasswordName = New System.Windows.Forms.Label()
        Me.Lbl_PasswordUsername = New System.Windows.Forms.Label()
        Me.Lbl_PasswordURL = New System.Windows.Forms.Label()
        Me.Lbl_PasswordValue = New System.Windows.Forms.Label()
        Me.Lbl_PasswordDescription = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Txt_Passphrase = New System.Windows.Forms.TextBox()
        Me.Btn_Verify = New System.Windows.Forms.Button()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Lbl_SharedWith = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Btn_Login
        '
        Me.Btn_Login.Location = New System.Drawing.Point(12, 41)
        Me.Btn_Login.Name = "Btn_Login"
        Me.Btn_Login.Size = New System.Drawing.Size(110, 23)
        Me.Btn_Login.TabIndex = 5
        Me.Btn_Login.Text = "Login"
        Me.Btn_Login.UseVisualStyleBackColor = True
        '
        'Txt_Status
        '
        Me.Txt_Status.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Txt_Status.Location = New System.Drawing.Point(0, 26)
        Me.Txt_Status.Multiline = True
        Me.Txt_Status.Name = "Txt_Status"
        Me.Txt_Status.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.Txt_Status.Size = New System.Drawing.Size(758, 87)
        Me.Txt_Status.TabIndex = 6
        Me.Txt_Status.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(39, 15)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Status"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(168, 74)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(63, 15)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Server URL"
        '
        'Txt_ServerURL
        '
        Me.Txt_ServerURL.Location = New System.Drawing.Point(272, 71)
        Me.Txt_ServerURL.Name = "Txt_ServerURL"
        Me.Txt_ServerURL.Size = New System.Drawing.Size(474, 23)
        Me.Txt_ServerURL.TabIndex = 2
        '
        'Btn_GetUsers
        '
        Me.Btn_GetUsers.Location = New System.Drawing.Point(12, 70)
        Me.Btn_GetUsers.Name = "Btn_GetUsers"
        Me.Btn_GetUsers.Size = New System.Drawing.Size(110, 23)
        Me.Btn_GetUsers.TabIndex = 6
        Me.Btn_GetUsers.Text = "Get users"
        Me.Btn_GetUsers.UseVisualStyleBackColor = True
        '
        'Btn_GetPasswords
        '
        Me.Btn_GetPasswords.Location = New System.Drawing.Point(12, 99)
        Me.Btn_GetPasswords.Name = "Btn_GetPasswords"
        Me.Btn_GetPasswords.Size = New System.Drawing.Size(110, 23)
        Me.Btn_GetPasswords.TabIndex = 7
        Me.Btn_GetPasswords.Text = "Get passwords"
        Me.Btn_GetPasswords.UseVisualStyleBackColor = True
        '
        'Txt_ServerFingerprint
        '
        Me.Txt_ServerFingerprint.Location = New System.Drawing.Point(272, 100)
        Me.Txt_ServerFingerprint.Name = "Txt_ServerFingerprint"
        Me.Txt_ServerFingerprint.Size = New System.Drawing.Size(474, 23)
        Me.Txt_ServerFingerprint.TabIndex = 3
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(168, 103)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(98, 15)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Server fingerprint"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(168, 16)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(89, 15)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "User fingerprint"
        '
        'Txt_UserFingerprint
        '
        Me.Txt_UserFingerprint.Location = New System.Drawing.Point(272, 13)
        Me.Txt_UserFingerprint.Name = "Txt_UserFingerprint"
        Me.Txt_UserFingerprint.Size = New System.Drawing.Size(474, 23)
        Me.Txt_UserFingerprint.TabIndex = 0
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(272, 126)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(35, 15)
        Me.Label5.TabIndex = 3
        Me.Label5.Text = "Users"
        '
        'Lb_Users
        '
        Me.Lb_Users.FormattingEnabled = True
        Me.Lb_Users.ItemHeight = 15
        Me.Lb_Users.Location = New System.Drawing.Point(272, 144)
        Me.Lb_Users.Name = "Lb_Users"
        Me.Lb_Users.ScrollAlwaysVisible = True
        Me.Lb_Users.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple
        Me.Lb_Users.Size = New System.Drawing.Size(230, 229)
        Me.Lb_Users.TabIndex = 4
        Me.Lb_Users.TabStop = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(516, 126)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(57, 15)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "Password"
        '
        'Lb_Passwords
        '
        Me.Lb_Passwords.FormattingEnabled = True
        Me.Lb_Passwords.ItemHeight = 15
        Me.Lb_Passwords.Location = New System.Drawing.Point(516, 144)
        Me.Lb_Passwords.Name = "Lb_Passwords"
        Me.Lb_Passwords.ScrollAlwaysVisible = True
        Me.Lb_Passwords.Size = New System.Drawing.Size(230, 229)
        Me.Lb_Passwords.TabIndex = 5
        Me.Lb_Passwords.TabStop = False
        '
        'Btn_SharePassword
        '
        Me.Btn_SharePassword.Location = New System.Drawing.Point(12, 186)
        Me.Btn_SharePassword.Name = "Btn_SharePassword"
        Me.Btn_SharePassword.Size = New System.Drawing.Size(110, 23)
        Me.Btn_SharePassword.TabIndex = 10
        Me.Btn_SharePassword.Text = "Share password"
        Me.Btn_SharePassword.UseVisualStyleBackColor = True
        '
        'Btn_RemoveShare
        '
        Me.Btn_RemoveShare.Location = New System.Drawing.Point(12, 215)
        Me.Btn_RemoveShare.Name = "Btn_RemoveShare"
        Me.Btn_RemoveShare.Size = New System.Drawing.Size(110, 23)
        Me.Btn_RemoveShare.TabIndex = 11
        Me.Btn_RemoveShare.Text = "Remove share"
        Me.Btn_RemoveShare.UseVisualStyleBackColor = True
        '
        'Btn_AddPassword
        '
        Me.Btn_AddPassword.Location = New System.Drawing.Point(12, 128)
        Me.Btn_AddPassword.Name = "Btn_AddPassword"
        Me.Btn_AddPassword.Size = New System.Drawing.Size(110, 23)
        Me.Btn_AddPassword.TabIndex = 8
        Me.Btn_AddPassword.Text = "Add password"
        Me.Btn_AddPassword.UseVisualStyleBackColor = True
        '
        'Btn_ChangePassword
        '
        Me.Btn_ChangePassword.Location = New System.Drawing.Point(12, 157)
        Me.Btn_ChangePassword.Name = "Btn_ChangePassword"
        Me.Btn_ChangePassword.Size = New System.Drawing.Size(110, 23)
        Me.Btn_ChangePassword.TabIndex = 9
        Me.Btn_ChangePassword.Text = "Change password"
        Me.Btn_ChangePassword.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Txt_Status)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.Btn_ClearStatus)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 400)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(758, 113)
        Me.Panel1.TabIndex = 8
        '
        'Btn_ClearStatus
        '
        Me.Btn_ClearStatus.Location = New System.Drawing.Point(636, 2)
        Me.Btn_ClearStatus.Name = "Btn_ClearStatus"
        Me.Btn_ClearStatus.Size = New System.Drawing.Size(110, 23)
        Me.Btn_ClearStatus.TabIndex = 12
        Me.Btn_ClearStatus.Text = "Clear status"
        Me.Btn_ClearStatus.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(12, 258)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(97, 15)
        Me.Label7.TabIndex = 9
        Me.Label7.Text = "Password details:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(12, 278)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(42, 15)
        Me.Label8.TabIndex = 9
        Me.Label8.Text = "Name:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(12, 298)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(63, 15)
        Me.Label9.TabIndex = 9
        Me.Label9.Text = "Username:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(12, 318)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(31, 15)
        Me.Label10.TabIndex = 9
        Me.Label10.Text = "URL:"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(12, 338)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(60, 15)
        Me.Label11.TabIndex = 9
        Me.Label11.Text = "Password:"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(12, 358)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(70, 15)
        Me.Label12.TabIndex = 9
        Me.Label12.Text = "Description:"
        '
        'Lbl_PasswordName
        '
        Me.Lbl_PasswordName.AutoSize = True
        Me.Lbl_PasswordName.Location = New System.Drawing.Point(88, 278)
        Me.Lbl_PasswordName.Name = "Lbl_PasswordName"
        Me.Lbl_PasswordName.Size = New System.Drawing.Size(12, 15)
        Me.Lbl_PasswordName.TabIndex = 9
        Me.Lbl_PasswordName.Text = "-"
        '
        'Lbl_PasswordUsername
        '
        Me.Lbl_PasswordUsername.AutoSize = True
        Me.Lbl_PasswordUsername.Location = New System.Drawing.Point(88, 298)
        Me.Lbl_PasswordUsername.Name = "Lbl_PasswordUsername"
        Me.Lbl_PasswordUsername.Size = New System.Drawing.Size(12, 15)
        Me.Lbl_PasswordUsername.TabIndex = 9
        Me.Lbl_PasswordUsername.Text = "-"
        '
        'Lbl_PasswordURL
        '
        Me.Lbl_PasswordURL.AutoSize = True
        Me.Lbl_PasswordURL.Location = New System.Drawing.Point(88, 318)
        Me.Lbl_PasswordURL.Name = "Lbl_PasswordURL"
        Me.Lbl_PasswordURL.Size = New System.Drawing.Size(12, 15)
        Me.Lbl_PasswordURL.TabIndex = 9
        Me.Lbl_PasswordURL.Text = "-"
        '
        'Lbl_PasswordValue
        '
        Me.Lbl_PasswordValue.AutoSize = True
        Me.Lbl_PasswordValue.Location = New System.Drawing.Point(88, 338)
        Me.Lbl_PasswordValue.Name = "Lbl_PasswordValue"
        Me.Lbl_PasswordValue.Size = New System.Drawing.Size(12, 15)
        Me.Lbl_PasswordValue.TabIndex = 9
        Me.Lbl_PasswordValue.Text = "-"
        '
        'Lbl_PasswordDescription
        '
        Me.Lbl_PasswordDescription.AutoSize = True
        Me.Lbl_PasswordDescription.Location = New System.Drawing.Point(88, 358)
        Me.Lbl_PasswordDescription.Name = "Lbl_PasswordDescription"
        Me.Lbl_PasswordDescription.Size = New System.Drawing.Size(12, 15)
        Me.Lbl_PasswordDescription.TabIndex = 9
        Me.Lbl_PasswordDescription.Text = "-"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(168, 45)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(91, 15)
        Me.Label13.TabIndex = 3
        Me.Label13.Text = "User passphrase"
        '
        'Txt_Passphrase
        '
        Me.Txt_Passphrase.Location = New System.Drawing.Point(272, 42)
        Me.Txt_Passphrase.Name = "Txt_Passphrase"
        Me.Txt_Passphrase.PasswordChar = Global.Microsoft.VisualBasic.ChrW(8226)
        Me.Txt_Passphrase.Size = New System.Drawing.Size(474, 23)
        Me.Txt_Passphrase.TabIndex = 1
        '
        'Btn_Verify
        '
        Me.Btn_Verify.Location = New System.Drawing.Point(12, 12)
        Me.Btn_Verify.Name = "Btn_Verify"
        Me.Btn_Verify.Size = New System.Drawing.Size(110, 23)
        Me.Btn_Verify.TabIndex = 4
        Me.Btn_Verify.Text = "Verify server"
        Me.Btn_Verify.UseVisualStyleBackColor = True
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(12, 378)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(72, 15)
        Me.Label14.TabIndex = 9
        Me.Label14.Text = "Shared with:"
        '
        'Lbl_SharedWith
        '
        Me.Lbl_SharedWith.AutoSize = True
        Me.Lbl_SharedWith.Location = New System.Drawing.Point(88, 378)
        Me.Lbl_SharedWith.Name = "Lbl_SharedWith"
        Me.Lbl_SharedWith.Size = New System.Drawing.Size(12, 15)
        Me.Lbl_SharedWith.TabIndex = 9
        Me.Lbl_SharedWith.Text = "-"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(758, 513)
        Me.Controls.Add(Me.Lbl_SharedWith)
        Me.Controls.Add(Me.Lbl_PasswordDescription)
        Me.Controls.Add(Me.Lbl_PasswordValue)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Lbl_PasswordURL)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Lbl_PasswordUsername)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Lbl_PasswordName)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Lb_Passwords)
        Me.Controls.Add(Me.Lb_Users)
        Me.Controls.Add(Me.Btn_RemoveShare)
        Me.Controls.Add(Me.Btn_SharePassword)
        Me.Controls.Add(Me.Btn_ChangePassword)
        Me.Controls.Add(Me.Btn_AddPassword)
        Me.Controls.Add(Me.Btn_GetPasswords)
        Me.Controls.Add(Me.Btn_GetUsers)
        Me.Controls.Add(Me.Txt_ServerFingerprint)
        Me.Controls.Add(Me.Txt_Passphrase)
        Me.Controls.Add(Me.Txt_UserFingerprint)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Txt_ServerURL)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Btn_Verify)
        Me.Controls.Add(Me.Btn_Login)
        Me.Name = "Form1"
        Me.Text = "PassboltAPI.NET Sample"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Btn_Login As Button
    Friend WithEvents Txt_Status As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Txt_ServerURL As TextBox
    Friend WithEvents Btn_GetUsers As Button
    Friend WithEvents Btn_GetPasswords As Button
    Friend WithEvents Txt_ServerFingerprint As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Txt_UserFingerprint As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Lb_Users As ListBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Lb_Passwords As ListBox
    Friend WithEvents Btn_SharePassword As Button
    Friend WithEvents Btn_RemoveShare As Button
    Friend WithEvents Btn_AddPassword As Button
    Friend WithEvents Btn_ChangePassword As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents Lbl_PasswordName As Label
    Friend WithEvents Lbl_PasswordUsername As Label
    Friend WithEvents Lbl_PasswordURL As Label
    Friend WithEvents Lbl_PasswordValue As Label
    Friend WithEvents Lbl_PasswordDescription As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Txt_Passphrase As TextBox
    Friend WithEvents Btn_ClearStatus As Button
    Friend WithEvents Btn_Verify As Button
    Friend WithEvents Label14 As Label
    Friend WithEvents Lbl_SharedWith As Label
End Class
