Imports System.IO
Imports System.Text
Imports Passbolt.NET
Imports Passbolt.NET.PassboltResults
Imports Passbolt.NET.Structs

Public Class Form1
    Dim Passbolt As PassboltAPI
    Dim UserLoggedOn As Boolean

    Dim UserList As List(Of Users)
    Dim ResourceList As List(Of Resources)

    Private Sub Btn_ClearStatus_Click(sender As Object, e As EventArgs) Handles Btn_ClearStatus.Click
        Txt_Status.Text = ""
    End Sub

    Private Sub UpdateStatusBox(NewText As String)
        Try
            Txt_Status.Text += NewText & Environment.NewLine
            Txt_Status.Select(Txt_Status.Text.Length, 0)
            Txt_Status.ScrollToCaret()
            Txt_Status.Select(Txt_Status.Text.Length - 1, 0)
        Catch ex As Exception

        End Try
    End Sub

    Private Async Sub Btn_Verify_Click(sender As Object, e As EventArgs) Handles Btn_Verify.Click

        If Txt_ServerURL.Text = "" OrElse
            Txt_ServerFingerprint.Text = "" Then
            UpdateStatusBox("Please enter the server details")
            Exit Sub
        End If

        'First create a Passbolt API instance
        If Passbolt Is Nothing Then
            Passbolt = New PassboltAPI(Txt_ServerURL.Text, Txt_ServerFingerprint.Text, 2)
        End If

        'Check if the server verify checks out
        If Await Passbolt.VerifyServer() Then
            UpdateStatusBox("Server verified with the supplied fingerprint")
        Else
            UpdateStatusBox("Server verify failed, do not attempt to login!")
        End If
    End Sub

    Private Async Sub Btn_Login_Click(sender As Object, e As EventArgs) Handles Btn_Login.Click
        'Log out the user if already logged in
        If UserLoggedOn AndAlso Passbolt IsNot Nothing Then
            Passbolt.LogoutUser()
            UserLoggedOn = False
            UpdateStatusBox("User logged out")
        End If

        'Clear any lists we might have
        Lb_Passwords.Items.Clear()
        Lb_Users.Items.Clear()

        'First we check if all login fields contain a value
        If Txt_UserFingerprint.Text = "" OrElse
            Txt_Passphrase.Text = "" OrElse
            Txt_ServerURL.Text = "" Then
            UpdateStatusBox("Please enter the user and server details")
            Exit Sub
        End If

        'Reset the UserLoggedOn
        UserLoggedOn = False

        'Then we create a new instance of the Passbolt API and enter the ServerURL and fingerprint
        Passbolt = New PassboltAPI(Txt_ServerURL.Text, Txt_ServerFingerprint.Text, 2)

        'Then we need to load the users private key before we can progress with the login request
        Dim ofd As New OpenFileDialog With {
            .Title = "Open users private key file",
            .Filter = "Armoured ASCII file (*.asc)|*.asc|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            .Multiselect = False
        }

        If ofd.ShowDialog(Me) = DialogResult.OK Then
            If File.Exists(ofd.FileName) Then
                UpdateStatusBox("User private key file found, reading contents")
                Passbolt.userPrivateKey = File.ReadAllText(ofd.FileName, Encoding.UTF8)

                UpdateStatusBox("File read attempting login")
                If Await Passbolt.LoginUser(Txt_UserFingerprint.Text, Txt_Passphrase.Text) = LoginResult.Success Then
                    UpdateStatusBox("Login succeeded")
                    UserLoggedOn = True
                Else
                    UpdateStatusBox("Login failed")
                    UpdateStatusBox(Passbolt.readException.Message)
                End If
            Else
                UpdateStatusBox("Selected user private key file does not exist")
                UpdateStatusBox(ofd.FileName)
            End If
        Else
            UpdateStatusBox("Login cancelled")
        End If
    End Sub

    Private Async Sub Btn_GetUsers_Click(sender As Object, e As EventArgs) Handles Btn_GetUsers.Click
        'First check if a passbolt instance exists and a user has logged in
        If Not UserLoggedOn OrElse Passbolt Is Nothing Then
            UpdateStatusBox("Cannot request user list, you are not logged in")
            Exit Sub
        End If

        'Request the ARO list from Passbolt
        Dim result = Await Passbolt.GetAros()
        If result = GetArosResult.Success Then
            UpdateStatusBox("User list retrieved, attempting to read the list")
            UserList = Passbolt.GetUsers
            Lb_Users.Items.Clear()

            For Each User In UserList
                Lb_Users.Items.Add(User.UserFullName)
            Next
        Else
            UpdateStatusBox("Getting user list resulted in an error")
            UpdateStatusBox(Passbolt.readException.Message)
        End If

    End Sub

    Private Async Sub Btn_GetPasswords_Click(sender As Object, e As EventArgs) Handles Btn_GetPasswords.Click
        'First check if a passbolt instance exists and a user has logged in
        If Not UserLoggedOn OrElse Passbolt Is Nothing Then
            UpdateStatusBox("Cannot request password list, you are not logged in")
            Exit Sub
        End If

        'Get all owned and shared resources
        Dim result = Await Passbolt.GetAcos(False)
        If result = GetAcosResult.Success Then
            ResourceList = Passbolt.GetResources
            Lb_Passwords.Items.Clear()

            'Put all resource names into a visible list
            For Each Resource In ResourceList
                Lb_Passwords.Items.Add(Resource.ResourceName)
            Next
        ElseIf result = GetAcosResult.FailedNoResources Then
            UpdateStatusBox("No passwords visible for this user")
            Lb_Passwords.Items.Clear()
        Else
            UpdateStatusBox("Getting password list resulted in an error")
            If Passbolt.readException IsNot Nothing Then
                UpdateStatusBox(Passbolt.readException.Message)
            End If
        End If
    End Sub

    Private Async Sub Lb_Passwords_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Lb_Passwords.SelectedIndexChanged
        Try
            'Find the current resource, there is probably a more effecient method. But as example this was the easiest way.
            For Each Resource In ResourceList
                If Resource.ID = Lb_Passwords.SelectedIndex Then
                    Lbl_PasswordName.Text = Resource.ResourceName
                    Lbl_PasswordURL.Text = Resource.ResourceURI
                    Lbl_PasswordUsername.Text = Resource.ResourceUsername

                    Dim PlainSecret = Await Passbolt.GetPlainTextPassword(Resource.ResourceUUID, Txt_Passphrase.Text)
                    Lbl_PasswordValue.Text = PlainSecret.Password
                    Lbl_PasswordDescription.Text = PlainSecret.Description
                    UpdateStatusBox("Password retrieved")

                    Dim ShareResult = Await Passbolt.GetResourceShares(Resource.ResourceUUID)
                    If ShareResult = GetSharesResult.Success Then
                        Dim UserShares = Passbolt.GetResourceShareList
                        If UserShares IsNot Nothing AndAlso UserShares.Count > 0 Then
                            Dim UserShareString As New StringBuilder
                            'Put all users name in a string
                            For Each Share In UserShares
                                UserShareString.Append(Share.UserFullName).Append(" | ")
                            Next

                            Lbl_SharedWith.Text = UserShareString.ToString
                        ElseIf UserShares Is Nothing Then
                            UpdateStatusBox("Getting share list resulted in an error")
                            UpdateStatusBox(Passbolt.readException.Message)
                        Else
                            UpdateStatusBox("No shares found")
                        End If
                    Else
                        If Passbolt.readException IsNot Nothing Then
                            UpdateStatusBox("Getting shares resulted in an error")
                            UpdateStatusBox(Passbolt.readException.Message)
                        Else
                            UpdateStatusBox("No shares found")
                        End If
                    End If
                    Exit For
                End If
            Next
        Catch ex As Exception
            UpdateStatusBox("Getting password resulted in an error")
            UpdateStatusBox(ex.Message)
        End Try
    End Sub

    Private Async Sub Btn_AddPassword_Click(sender As Object, e As EventArgs) Handles Btn_AddPassword.Click
        Dim newPassword As New DlgNewPassword

        'Let the user input a new password
        If newPassword.ShowDialog(Me) Then
            'Create a new resource with the new data
            Dim result = Await Passbolt.CreateResource(newPassword.ResourceName,
                                                       newPassword.ResourceUsername,
                                                       newPassword.ResourcePassword,
                                                       newPassword.ResourceDescription,
                                                       newPassword.ResourceURL,
                                                       Txt_Passphrase.Text)

            If result = CreateResourceResult.Success Then
                UpdateStatusBox("New password added")
            Else
                UpdateStatusBox("Adding new password resulted in an error")
                If Passbolt.readException IsNot Nothing Then
                    UpdateStatusBox(Passbolt.readException.Message)
                End If
            End If
        End If
    End Sub

    Private Async Sub Btn_ChangePassword_Click(sender As Object, e As EventArgs) Handles Btn_ChangePassword.Click
        'Only continue if the user selected a password
        If Lb_Passwords.SelectedIndex < 0 Then
            UpdateStatusBox("No password selected, please select a password to change")
            Exit Sub
        End If

        'Use the same dialog as new password but indicate it is an existing password
        Dim newPassword As New DlgNewPassword With {
            .ChangePassword = True,
            .ResourceName = Lbl_PasswordName.Text,
            .ResourceUsername = Lbl_PasswordUsername.Text,
            .ResourceURL = Lbl_PasswordURL.Text,
            .ResourceDescription = Lbl_PasswordDescription.Text
        }

        'Get the Resource of the selected password
        Dim SelectedResource As Resources = ResourceList(Lb_Passwords.SelectedIndex)

        If newPassword.ShowDialog(Me) Then
            'Create a new resource with the new data
            Dim result = Await Passbolt.UpdateResource(SelectedResource.ResourceUUID,
                                                       newPassword.ResourceName,
                                                       newPassword.ResourceUsername,
                                                       newPassword.ResourcePassword,
                                                       newPassword.ResourceDescription,
                                                       newPassword.ResourceURL,
                                                       Txt_Passphrase.Text)

            If result = UpdateResourceResult.Success Then
                UpdateStatusBox("Password changed")
            Else
                UpdateStatusBox("Changing password resulted in an error")
                If Passbolt.readException IsNot Nothing Then
                    UpdateStatusBox(Passbolt.readException.Message)
                End If
            End If
        End If
    End Sub

    Private Async Sub Btn_SharePassword_Click(sender As Object, e As EventArgs) Handles Btn_SharePassword.Click
        'Only continue if the user selected a password and selected users
        If Lb_Passwords.SelectedIndex < 0 Then
            UpdateStatusBox("No password selected, please select a password to change")
            Exit Sub
        End If
        If Lb_Users.SelectedIndex < 0 Then
            UpdateStatusBox("No user(s) selected, please select a one or more users to share")
            Exit Sub
        End If

        'Get the Resource of the selected password
        Dim SelectedResource As Resources = ResourceList(Lb_Passwords.SelectedIndex)

        'Get the selected users
        Dim SelectedUsers As New List(Of Users)
        For Each SelUser In Lb_Users.SelectedIndices
            SelectedUsers.Add(UserList(SelUser))
        Next

        'First attempt a simulated share
        UpdateStatusBox("Simulating sharing the password")
        Dim SimResult = Await Passbolt.SimulateShare(SelectedResource.ResourceUUID, SelectedUsers.ToArray)
        If SimResult = SimulateShareResult.Success Then
            UpdateStatusBox("Simulating sharing the password succeeded")
            Dim SimulateShareUUIDs = Passbolt.GetSimulateResult

            'Check if the simulate results in the same number of UUIDs
            If SimulateShareUUIDs.Count = SelectedUsers.Count Then
                'If it is the same number then the simulate succeeded and we can share the resource
                UpdateStatusBox("Sharing the password")
                Dim ShareResult = Await Passbolt.Share(SelectedResource.ResourceUUID, SelectedUsers.ToArray, Txt_Passphrase.Text)

                If ShareResult = ShareResult.Success Then
                    UpdateStatusBox("Sharing the password succeeded")
                Else
                    UpdateStatusBox("Sharing the password resulted in an error")
                    If Passbolt.readException IsNot Nothing Then
                        UpdateStatusBox(Passbolt.readException.Message)
                    End If
                End If
            Else
                UpdateStatusBox("Simulating sharing the password returned a different number of shares")
                UpdateStatusBox("Is a user selected that already has access?")
            End If
        Else
            UpdateStatusBox("Simulating sharing the password resulted in an error")
            If Passbolt.readException IsNot Nothing Then
                UpdateStatusBox(Passbolt.readException.Message)
            End If
        End If
    End Sub

    Private Async Sub Btn_RemoveShare_Click(sender As Object, e As EventArgs) Handles Btn_RemoveShare.Click
        'Only continue if the user selected a password and selected users
        If Lb_Passwords.SelectedIndex < 0 Then
            UpdateStatusBox("No password selected, please select a password to change")
            Exit Sub
        End If
        If Lb_Users.SelectedIndex < 0 Then
            UpdateStatusBox("No user(s) selected, please select a one or more users to remove")
            Exit Sub
        End If

        'Get the Resource of the selected password
        Dim SelectedResource As Resources = ResourceList(Lb_Passwords.SelectedIndex)

        'Get the selected users
        Dim SelectedUsers As New List(Of Users)
        For Each SelUser In Lb_Users.SelectedIndices
            SelectedUsers.Add(UserList(SelUser))
        Next

        Dim RemoveShare = Await Passbolt.RemoveShare(SelectedResource.ResourceUUID, SelectedUsers.ToArray)
        If RemoveShare = RemoveShareResult.Success Then
            UpdateStatusBox("Removing share of the password succeeded")
        ElseIf RemoveShare = RemoveShareResult.FailedRequestFailed Then
            UpdateStatusBox("Removing share of the password failed, are you the owner of the resource?")
        Else
            UpdateStatusBox("Removing share of the password resulted in an error")
            If Passbolt.readException IsNot Nothing Then
                UpdateStatusBox(Passbolt.readException.Message)
            End If
        End If
    End Sub
End Class
