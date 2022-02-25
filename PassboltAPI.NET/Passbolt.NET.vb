Imports System.Dynamic
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json.Linq
Imports Passbolt.NET.Structs
Imports Passbolt.NET.PassboltResults
Imports Passbolt.NET.PassboltConstants
Imports PgpCore

Public Class PassboltAPI
    'Variables
    Private SimulateShareList As List(Of String)
    Private ShareList As List(Of String)
    Private ResourceList As List(Of Resources)
    Private UserList As List(Of Users)
    Private AcoResourceList As Dictionary(Of String, Object)
    Private AcoSharesList As Dictionary(Of String, Users)
    Private AroGroupsList As Dictionary(Of String, Object)
    Private AroUserList As Dictionary(Of String, Object)

    'Properties
    Private _serverURI As String
    Private _apiVersion As Integer = 2
    Private _serverKey As String
    Private _serverFingerprint As String
    Private _userPrivateKey As String
    Private _userPublicKey As String
    Private _FunctionException As Exception

    'Http handlers
    Private CookieJar As CookieContainer
    Private pbHandler As HttpClientHandler
    Private pbClient As HttpClient


#Region "Properties"
    ''' <summary>
    ''' Gets/Sets the API version, default value = 2
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property APIVersion(Version As Integer) As Integer
        Set(value As Integer)
            _apiVersion = Version
        End Set
        Get
            Return _apiVersion
        End Get
    End Property

    ''' <summary>
    ''' Gets/Sets the server URL
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property server(URL As String) As String
        Set(value As String)
            'We make sure the server ends with a \
            If Not URL.EndsWith("\") Then
                URL += "\"
            End If

            _serverURI = URL
        End Set
        Get
            Return _serverURI
        End Get
    End Property

    ''' <summary>
    ''' Gets/Sets the server fingerprint
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property serverFingerprint(Fingerprint As String) As String
        Set(value As String)
            _serverFingerprint = Fingerprint
        End Set
        Get
            Return _serverFingerprint
        End Get
    End Property

    ''' <summary>
    ''' Sets the users private key, WriteOnly
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property userPrivateKey() As String
        Set(ByVal value As String)
            _userPrivateKey = value
        End Set
    End Property


    ''' <summary>
    ''' Gets the last exception.
    ''' Some functions do not return an error code have use this value to show their exception
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public ReadOnly Property readException() As Exception
        Get
            Return _FunctionException
        End Get
    End Property
#End Region

#Region "OpenPGP"
    ''' <summary>
    ''' The keys received from the HTTP requests need some cleaning up before .NET can read them properly
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Private Function CleanKey(srcKey As String) As String
        'Clean up the OpenPGP keys so we can read them without errors
        Dim EncryptedString = srcKey.Replace("%0D%0A", vbCrLf)
        EncryptedString = EncryptedString.Replace("%0A", vbCrLf)
        EncryptedString = EncryptedString.Replace("%2B", "+")
        EncryptedString = EncryptedString.Replace("%2F", "/")
        EncryptedString = EncryptedString.Replace("%3D", "=")
        EncryptedString = EncryptedString.Replace("\+", " ")
        Return EncryptedString
    End Function

    ''' <summary>
    ''' Encrypt the given source text with the given public key and passphrase.
    ''' Private key is loaded from the settings
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Private Async Function Encrypt(srcText As String, gpgkey As String, passphrase As String) As Task(Of String)
        Try
            'Create the key ring
            Dim EncKeys As EncryptionKeys = New EncryptionKeys(gpgkey, _userPrivateKey, passphrase)

            'Encrypt with the specified keyring
            Using opgp As PGP = New PGP(EncKeys)
                Dim outString As String = Await opgp.EncryptArmoredStringAndSignAsync(srcText)
                Return outString
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Decrypt the given source text with the passphrase.
    ''' Private key is loaded from the settings
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Private Async Function Decrypt(srcText As String, passphrase As String) As Task(Of String)
        Try
            'Get public key
            Dim EncKeys As EncryptionKeys = New EncryptionKeys(_userPrivateKey, passphrase)

            Dim inStream As Stream = New MemoryStream(Encoding.UTF8.GetBytes(srcText))
            Dim outStream As Stream = New MemoryStream

            'Decrypt
            Using opgp As PGP = New PGP(EncKeys)
                Await opgp.DecryptStreamAsync(inStream, outStream)
                outStream.Position = 0
                Using sr As StreamReader = New StreamReader(outStream)
                    Return sr.ReadToEnd
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function IsValidJSON(JSONstring As String) As Boolean
        Try
            'Newtonsoft does not have a function to check for validity, so we attempt to parse the string.
            'If we get an error it is not valid, if we don't it is valid.
            Dim TryParse = JToken.Parse(JSONstring)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
#End Region

#Region "HTTP events"

    ''' <summary>
    ''' Certificate handler for https requests
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Private Function AcceptAllCertifications(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) As Boolean
        'ToDo a proper server certificate validate function, this was added for locally hosted servers with self signed certificates
        'For now we check if the certificate request comes from the given server address
        Dim obj = sender.GetType
        If TypeOf sender Is HttpRequestMessage Then
            Dim hrm As HttpRequestMessage = sender
            If hrm.RequestUri.AbsoluteUri.Contains(_serverURI) Then
                Return True
            Else
                Return False
            End If
        End If

        'For now accept all certificates
        Return True
    End Function
#End Region

#Region "Private functions"
    ''' <summary>
    ''' Creates a new instance for the PassboltAPI.NET
    ''' It needs the ServerURL and API version as input
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Sub New(serverURL As String, serverFingerprint As String, APIVersion As Integer)
        'We make sure the server ends with a \
        If Not serverURL.EndsWith("/") Then
            serverURL += "/"
        End If

        'Update the properties with the correct values
        _serverURI = serverURL
        _apiVersion = APIVersion
        _serverFingerprint = CleanFingerprint(serverFingerprint)

        'Clear any list or dictionary
        SimulateShareList = New List(Of String)
        ShareList = New List(Of String)
        ResourceList = New List(Of Resources)
        UserList = New List(Of Users)
        AroGroupsList = New Dictionary(Of String, Object)
        AroUserList = New Dictionary(Of String, Object)

        'Create a new http client handler
        CookieJar = New CookieContainer
        pbHandler = New HttpClientHandler() With {
            .CookieContainer = CookieJar
        }

        'Make sure certificates are handled
        pbHandler.ServerCertificateCustomValidationCallback = AddressOf AcceptAllCertifications

        'Create the http client
        pbClient = New HttpClient(pbHandler)
    End Sub

    ''' <summary>
    ''' Cleans fingerprints from any characters that do not belong in the string
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Function CleanFingerprint(fingerprint) As String
        Dim CleanedFingerprint = Regex.Replace(fingerprint, "[^a-zA-Z0-9]", "")
        CleanedFingerprint = CleanedFingerprint.ToLower
        Return CleanedFingerprint
    End Function

    ''' <summary>
    ''' Clears the PassboltAPI stored keys,cookies
    ''' It needs the ServerURL and API version as input
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Private Sub Clear()
        'Clear any list or dictionary
        SimulateShareList = New List(Of String)
        ShareList = New List(Of String)
        ResourceList = New List(Of Resources)
        UserList = New List(Of Users)
        AroGroupsList = New Dictionary(Of String, Object)
        AroUserList = New Dictionary(Of String, Object)

        'Clear out the CookieContainer
        CookieJar = New CookieContainer

        'Release the old http client and create a new one
        pbHandler = New HttpClientHandler() With {
            .CookieContainer = CookieJar
        }
        pbClient = New HttpClient(pbHandler)
    End Sub

    ''' <summary>
    ''' Creates an URL for the API request
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Private Function CreateURL(Server As String, Path As String, AdditionalPath As String, ApiVersion As Integer, Filters As String, ByRef Err As Exception) As String
        Try
            'Init Err value
            Err = Nothing

            Dim result As New StringBuilder
            result.Append(Server).Append(Path)

            'Add to path incase of resourceID or something similar
            If AdditionalPath IsNot Nothing AndAlso AdditionalPath <> "" Then
                result.Append(AdditionalPath)
            End If

            'Add api version
            If ApiVersion > 0 Then
                result.Append("?api-version=v").Append(ApiVersion.ToString)
            End If

            'Add filters
            If Filters IsNot Nothing AndAlso Filters <> "" Then
                result.Append(Filters)
            End If

            'Return the build URL string
            Return result.ToString
        Catch ex As Exception
            Err = ex
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Encrypts the secret data for Passbolt
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Private Async Function GetEncryptedJSON(Value As String, Description As String, passphrase As String, Optional UserGPGKey As String = Nothing) As Task(Of String)
        Try
            'Clear last error
            _FunctionException = Nothing

            'Create secret data
            Dim newSecret As New Dictionary(Of String, String) From {
                {cPassword, Value},
                {cDescription, Description}
            }

            'Create a JSON string and encrypt
            Dim jData As String = JsonConvert.SerializeObject(newSecret)
            Dim eData As String

            'Either encrypt with a given shared public key or with the users own key
            If UserGPGKey IsNot Nothing Then
                eData = Await Encrypt(jData, UserGPGKey, passphrase)
            Else
                eData = Await Encrypt(jData, _userPublicKey, passphrase)
            End If

            'Return encrypted string
            Return eData
        Catch ex As Exception
            _FunctionException = ex
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Retrieve the resource data that we need for other API requests. Returns a dictionary for easier reading later on
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Private Async Function GetResourceData(resourceID As String) As Task(Of Dictionary(Of String, String))
        Try
            'Clear last error
            _FunctionException = Nothing

            'Build the URI we need for the API request
            Dim ResourceJSON As String = resourceID & ".json"
            Dim GetResource As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Get,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_ResourcesSpecific, ResourceJSON, _apiVersion, Nothing, _FunctionException))
            }

            'Send the request
            Dim result = Await pbClient.SendAsync(GetResource)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await result.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            'If the object has more then 1 value we can read it (header and body)
            If jsData.Count > 1 Then
                For Each jData In jsData
                    'Read the content of the body
                    If jData.Key.ToLower = cBody Then
                        Dim bData = jData.Value

                        'Create a new dictionary to return with the required resource data
                        Dim ResourceData As New Dictionary(Of String, String) From {
                            {cId, bData.id},
                            {cDescription, bData.description},
                            {cName, bData.name},
                            {cResourceTypeId, bData.resource_type_id},
                            {cUri, bData.uri},
                            {cUsername, bData.username}
                        }

                        'Return the dictionary
                        Return ResourceData
                    End If
                Next
            End If

            'Return an empty dictionary if we havent received anything
            Return New Dictionary(Of String, String)
        Catch ex As Exception
            _FunctionException = ex
            Return Nothing
        End Try
    End Function
#End Region

#Region "Public functions"

    ''' <summary>
    ''' Verifies the returned fingerprint of the server with the server fingerprint entered in the properties
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Async Function VerifyServer() As Task(Of Boolean)
        Try
            'Clear last error
            _FunctionException = Nothing

            'Build the URI we need for the API request
            Dim request As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Get,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_ServerVerify, Nothing, _apiVersion, Nothing, _FunctionException))
            }

            'If we got an error return false
            If _FunctionException IsNot Nothing Then
                Return False
            End If

            'Send the request
            Dim result = Await pbClient.SendAsync(request)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await result.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            If jsData.Count > 1 Then
                For Each jData In jsData
                    'Read the content of the body
                    If jData.Key.ToLower = cBody Then
                        Dim bData = jData.Value

                        'Read server keydata
                        _serverKey = bData.keydata

                        'Read server fingerprint and verify with the expected fingerprint
                        If _serverFingerprint.ToUpper = bData.fingerprint.ToString.ToUpper Then
                            Return True
                        Else
                            Return False
                        End If
                    End If
                Next
            End If

            'No valid data received
            Return False
        Catch ex As Exception
            'If an error occurs assume serverkey is invalid and clear it
            _serverKey = Nothing
            _FunctionException = ex
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Logs out the current user
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Function LogoutUser() As LogoutResult
        Try
            'Clear last error
            _FunctionException = Nothing

            Clear()
            Return LogoutResult.Success
        Catch ex As Exception
            _FunctionException = ex
            Return LogoutResult.FailedError
        End Try
    End Function

    ''' <summary>
    ''' Logs the user in with the supplied fingerprint and passphrase
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Async Function LoginUser(userFingerprint As String, passphrase As String) As Task(Of LoginResult)
        Try
            'Clear last error
            _FunctionException = Nothing

            'Build the URI we need for the API request
            Dim LoginRequest As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Post,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_ServerLogin, Nothing, _apiVersion, Nothing, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return LoginResult.FailedCreateURL
            End If

            'Clean fingerprint to remove all non alphanumeric characters, and lowercase it for the API request
            userFingerprint = CleanFingerprint(userFingerprint)

            'Build JSON dictionary
            Dim LoginRequestJSON = New Dictionary(Of String, Object) From {
                {cGpgAuth, New Dictionary(Of String, Object) From {
                    {cKeyId, userFingerprint}
                }}
            }

            'Create JSON string and send request
            LoginRequest.Content = New StringContent(JsonConvert.SerializeObject(LoginRequestJSON), Encoding.UTF8, "application/json")
            Dim LoginRequestResult = Await pbClient.SendAsync(LoginRequest)

            'Read the response and get the login token
            If LoginRequestResult.IsSuccessStatusCode Then
                Dim values As IEnumerable(Of String) = Nothing
                Dim Token = LoginRequestResult.Headers.TryGetValues(cGpgTokenCookie, values)

                'If we got a token decrypt it and send it back to finish the login request
                If values.Count > 0 Then
                    'Clean and decrypt the token
                    Dim cleKey = CleanKey(values(0))
                    Dim decKey = Await Decrypt(cleKey, passphrase)

                    'Build the URI we need for the API request
                    Dim LoginFinishRequest As HttpRequestMessage = New HttpRequestMessage() With {
                        .Method = HttpMethod.Post,
                        .RequestUri = New Uri(CreateURL(_serverURI, URI_ServerLogin, Nothing, _apiVersion, Nothing, _FunctionException))
                    }

                    'If we got an error return error index
                    If _FunctionException IsNot Nothing Then
                        Return PassboltResults.LoginResult.FailedCreateURL
                    End If

                    'Build JSON dictionary
                    Dim LoginFinishRequestJSON = New Dictionary(Of String, Object) From {
                        {cGpgAuth, New Dictionary(Of String, Object) From {
                            {cKeyId, userFingerprint},
                            {cUserTokenResult, decKey}
                        }}
                    }

                    'Create JSON string and send login request
                    LoginFinishRequest.Content = New StringContent(JsonConvert.SerializeObject(LoginFinishRequestJSON), Encoding.UTF8, "application/json")
                    Dim LoginFinishResult = Await pbClient.SendAsync(LoginFinishRequest)

                    'If response returns succeeded we have been logged in, now to get the users public key
                    If LoginFinishResult.IsSuccessStatusCode Then

                        'Build GET request to get user public key
                        Dim getUser As HttpRequestMessage = New HttpRequestMessage() With {
                            .Method = HttpMethod.Get,
                            .RequestUri = New Uri(CreateURL(_serverURI, URI_User, Nothing, _apiVersion, Nothing, _FunctionException))
                        }

                        'If we got an error return error index
                        If _FunctionException IsNot Nothing Then
                            Return LoginResult.FailedCreateURL
                        End If

                        'Send request
                        Dim UserDataResult = Await pbClient.SendAsync(getUser)

                        'Read the response and parse it into a JSON dynamic object
                        Dim Data = Await UserDataResult.Content.ReadAsStringAsync()
                        Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

                        'If the object has more then 1 value we can read it (header and body)
                        If jsData.Count > 1 Then
                            For Each jData In jsData
                                'Read the content of the body
                                If jData.Key.ToLower = cBody Then
                                    Dim bData = jData.Value

                                    'Read the users public key
                                    _userPublicKey = bData.GPGKey.armored_key

                                    Return LoginResult.Success
                                End If
                            Next
                        End If

                        'Failed to get the key from result
                        Return LoginResult.FailedUserKey
                    Else
                        'Failed to decrypt the key and got a failed result
                        Return LoginResult.FailedDecrypt
                    End If
                Else
                    'Failed to get the token
                    Return LoginResult.FailedToken
                End If
            Else
                'Failed to get the userdata with the given UUID
                Return LoginResult.FailedWrongUser
            End If
        Catch ex As Exception
            _FunctionException = ex
            Return LoginResult.FailedError
        End Try
    End Function

    ''' <summary>
    ''' Creates a new resource supplied with the name, username, secret, description, uri, and passphrase
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Async Function CreateResource(Name As String, UserName As String, Secret As String, Description As String, URI As String, passphrase As String) As Task(Of CreateResourceResult)
        Try
            'Clear last error
            _FunctionException = Nothing

            'Build the URI we need for the API request
            Dim request As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Get,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_ResourceTypes, Nothing, _apiVersion, Nothing, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return CreateResourceResult.FailedCreateURL
            End If

            'Send request
            Dim result = Await pbClient.SendAsync(request)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await result.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            'Define the new TypeID
            Dim TypeID As String = ""

            'If the object has more then 1 value we can read it (header and body)
            If jsData.Count > 1 Then
                For Each jData In jsData
                    'Read the content of the body
                    If jData.Key.ToLower = cBody Then
                        'Body contains an array with resource types, parse through them until we find the right one
                        If jData.Value.Count > 0 Then
                            For Each bData In jData.Value
                                'If we find the right Resource type, save the ID for future use
                                If bData.slug.ToLower = cPasswordDescription Then
                                    TypeID = bData.id
                                End If
                            Next
                        End If
                    End If
                Next
            End If

            'If we cannot find the correct resource type return an error
            If TypeID = "" Then
                Return CreateResourceResult.FailedNoResourceType
            End If

            'Build JSON dictionary
            Dim newResource As New Dictionary(Of String, Object) From {
                {cName, Name},
                {cUsername, UserName},
                {cResourceTypeId, TypeID},
                {cUri, URI},
                {cSecrets, {New Dictionary(Of String, String) From {
                    {cData, Await GetEncryptedJSON(Secret, Description, passphrase)}
                }}}
            }

            'Build the URI we need for the API request
            Dim addRequest As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Post,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_Resources, Nothing, _apiVersion, Nothing, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return CreateResourceResult.FailedCreateURL
            End If

            'Add content
            Dim jContent As String = JsonConvert.SerializeObject(newResource)
            addRequest.Content = New StringContent(jContent, Encoding.UTF8, "application/json")

            'Get crfs token value
            Dim Cookies = CookieJar.GetCookies(New System.Uri(_serverURI))
            Dim crfsValue As String = ""

            'Loop through cookies to find the csrf token
            For Each newCookie As Cookie In Cookies
                If newCookie.Name.ToLower.Contains("csrf") Then
                    crfsValue = newCookie.Value
                End If
            Next

            'Add headers to include the CSRF token
            addRequest.Headers.Add("Accept", "application/json")
            addRequest.Headers.Add("User-Agent", "Passbolt.NET/1.0")
            addRequest.Headers.Add("X-CSRF-Token", crfsValue)

            'Send request
            Dim requestResult = Await pbClient.SendAsync(addRequest)

            'Read the response for debug purposes
            Dim resultContent = requestResult.Content
            Dim resultString = Await resultContent.ReadAsStringAsync

            'If succeeded return the function, else report a fail
            If requestResult.StatusCode = HttpStatusCode.OK Then
                Return CreateResourceResult.Success
            Else
                Return CreateResourceResult.FailedNoID
            End If
        Catch ex As Exception
            _FunctionException = ex
            Return CreateResourceResult.FailedError
        End Try
    End Function

    ''' <summary>
    ''' Gets all available resources for the current user
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Async Function GetAcos(GetAll As Boolean) As Task(Of GetAcosResult)
        Try
            'Clear last error
            _FunctionException = Nothing

            'Create the Http GET request
            Dim FilterRequest As New StringBuilder

            'We can choose to get all resources available, or only the resources visible to the current user
            If GetAll Then
                FilterRequest.Append("")
            Else
                FilterRequest.Append("&filter[is-owned-by-me,is-shared-with-me]=1")
            End If

            'Build the URI we need for the API request
            Dim request As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Get,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_Resources, Nothing, _apiVersion, FilterRequest.ToString, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return GetAcosResult.FailedCreateURL
            End If

            'Send the request
            Dim result = Await pbClient.SendAsync(request)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await result.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            'Build new resource list
            AcoResourceList = New Dictionary(Of String, Object)

            'If the object has more then 1 value we can read it (header and body)
            If jsData.Count > 1 Then
                For Each jData In jsData
                    'Read the content of the body
                    If jData.Key.ToLower = cBody Then
                        'Body contains an array with resources
                        If jData.Value.Count > 0 Then
                            For Each bData In jData.Value
                                'Resource data
                                Dim newResource As New Dictionary(Of String, Object) From {
                                        {cId, bData.id},
                                        {cName, bData.name},
                                        {cUsername, bData.username},
                                        {cUri, bData.uri},
                                        {cDescription, bData.description},
                                        {cDeleted, bData.deleted},
                                        {cCreated, bData.created},
                                        {cModified, bData.modified},
                                        {cCreatedBy, bData.created_by},
                                        {cModifiedBy, bData.modified_by},
                                        {cResourceTypeId, bData.resource_type_id}
                                    }

                                'Add the resource data to the main dictionary, keyvalue is the resource ID
                                AcoResourceList.Add(bData.id, newResource)
                            Next
                        Else
                            'No resources were found, are there any stored on the server?
                            Return GetAcosResult.FailedNoResources
                        End If
                    End If
                Next
            Else
                'Failed request, no data received
                Return GetAcosResult.FailedNoData
            End If

            Return GetAcosResult.Success
        Catch ex As Exception
            _FunctionException = ex
            Return GetAcosResult.FailedError
        End Try
    End Function

    Public Async Function GetResourceShares(ResourceID As String) As Task(Of GetSharesResult)
        Try
            'Clear last error
            _FunctionException = Nothing

            'Create additional URL string
            Dim ResourceIDPath As String = ResourceID & ".json"
            Dim FilterRequest As New StringBuilder
            FilterRequest.Append("&filter[has-id][]=").Append(ResourceID).Append("&contain[permissions.user.profile]=1")

            'Build the URI we need for the API request
            Dim GetResources As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Get,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_Resources, Nothing, _apiVersion, FilterRequest.ToString, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return GetSharesResult.FailedCreateURL
            End If

            'Send request
            Dim result = Await pbClient.SendAsync(GetResources)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await result.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            'Create new dictionary
            AcoSharesList = New Dictionary(Of String, Users)

            'If the object has more then 1 value we can read it (header and body)
            If jsData.Count > 1 Then
                For Each jData In jsData
                    'Read the content of the body
                    If jData.Key.ToLower = cBody Then
                        If jData.Value.Count > 0 Then
                            Dim bData = jData.Value

                            For Each uData In bData(0).permissions
                                Dim newUser As New Users With {
                                    .Username = uData.user.username,
                                    .UserUUID = uData.user.id,
                                    .UserFirstName = uData.user.profile.first_name,
                                    .UserLastName = uData.user.profile.last_name,
                                    .UserFullName = .UserFirstName & " " & .UserLastName
                                }

                                AcoSharesList.Add(newUser.UserUUID, newUser)
                            Next

                            Return GetSharesResult.Success
                        End If
                    End If
                Next
            Else
                Return GetSharesResult.FailedNoData
            End If

            Return GetSharesResult.FailedNoData
        Catch ex As Exception
            _FunctionException = ex
            Return GetSharesResult.FailedError
        End Try
    End Function

    ''' <summary>
    ''' Gets the plain text password and description of the requested ResourceID.
    ''' Returns a dictionary with the keys password, and description.
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Async Function GetPlainTextPassword(ResourceID As String, passphrase As String) As Task(Of Secret)
        Try
            'Clear last error
            _FunctionException = Nothing

            'Create additional URL string
            Dim ResourceIDPath As String = ResourceID & ".json"

            'Build the URI we need for the API request
            Dim request As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Get,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_ResourceSecret, ResourceIDPath, _apiVersion, Nothing, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return Nothing
            End If

            'Send request
            Dim result = Await pbClient.SendAsync(request)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await result.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            'If the object has more then 1 value we can read it (header and body)
            If jsData.Count > 1 Then
                For Each jData In jsData
                    'Read the content of the body
                    If jData.Key.ToLower = cBody Then
                        Dim bData = jData.Value
                        Dim pData As String = Await Decrypt(bData.data, passphrase)

                        'First check if we have a valid JSON, else it is just the plain text password
                        If IsValidJSON(pData) Then
                            Dim sData = JsonConvert.DeserializeObject(Of ExpandoObject)(pData, New ExpandoObjectConverter)

                            'If we have data read it into a dictionary to return to the user
                            If sData.Any() Then
                                Dim PlainSecret As New Secret With {
                                    .Password = "",
                                    .Description = ""
                                }

                                For Each rData In sData
                                    If rData.Key = cPassword Then
                                        PlainSecret.Password = rData.Value
                                    ElseIf rData.Key = cDescription Then
                                        PlainSecret.Description = rData.Value
                                    End If
                                Next

                                Return PlainSecret
                            Else
                                'No data received
                                Return New Secret
                            End If

                        Else
                            'No valid JSON, we assume it is just the plaintext password
                            Dim PlainSecret As New Secret With {
                                    .Password = pData,
                                    .Description = ""
                                }

                            Return PlainSecret
                        End If
                    End If
                Next
            End If

            'No data received
            Return New Secret
        Catch ex As Exception
            _FunctionException = ex
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Gets all available Aros as users and groups
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Async Function GetAros() As Task(Of GetArosResult)
        Try
            'Clear last error
            _FunctionException = Nothing

            'Build the URI we need for the API request
            Dim request As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Get,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_ShareAros, Nothing, _apiVersion, Nothing, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return GetArosResult.FailedCreateURL
            End If

            'Send request
            Dim result = Await pbClient.SendAsync(request)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await result.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            If jsData.Count >= 2 Then
                'Clear the old lists
                AroGroupsList = New Dictionary(Of String, Object)
                AroUserList = New Dictionary(Of String, Object)
                UserList = New List(Of Users)

                'Loop through the available Aros
                For Each jData In jsData
                    If jData.Key.ToLower = "body" Then
                        If jData.Value.Count > 0 Then
                            For Each bData In jData.Value
                                'Convert to dictionary so we can check for keys to distinguish between groups and users
                                Dim settingsDictionary As IDictionary(Of String, Object) = CType(bData, IDictionary(Of String, Object))

                                If settingsDictionary.ContainsKey(cUserCount) Then
                                    'Group data
                                    Dim newAroGroup As New Dictionary(Of String, String) From {
                                        {cId, bData.id},
                                        {cUserCount, bData.user_count}
                                    }
                                    AroGroupsList.Add(bData.id, newAroGroup)
                                Else
                                    'User data
                                    Dim newAroUser As New Dictionary(Of String, Object) From {
                                        {cId, bData.profile.user_id},
                                        {cUsername, bData.username},
                                        {cFirstName, bData.profile.first_name},
                                        {cLastName, bData.profile.last_name},
                                        {cGpgKey, bData.gpgkey.armored_key},
                                        {cUserGroups, New Object}
                                    }

                                    'If user is part of a group add the group IDs
                                    If bData.groups_users.Count > 0 Then
                                        Dim newGroups As New List(Of String)
                                        For Each group In bData.groups_users
                                            newGroups.Add(group.group_id)
                                        Next

                                        'Assign group array to dictionary
                                        newAroUser(cUserGroups) = newGroups.ToArray
                                    End If

                                    AroUserList.Add(bData.profile.user_id, newAroUser)
                                End If
                            Next
                        End If
                    End If
                Next

                Return GetArosResult.Success
            Else
                Return GetArosResult.FailedNoData
            End If
        Catch ex As Exception
            _FunctionException = ex
            Return GetArosResult.FailedError
        End Try
    End Function

    ''' <summary>
    ''' Simulates a share request, takes the resource UUID and the index of all the users you wish to share with
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Async Function SimulateShare(resourceUUID As String, ShareUsers() As Users) As Task(Of SimulateShareResult)
        Try
            'Clear last error
            _FunctionException = Nothing

            SimulateShareList = New List(Of String)
            Dim ResourceJSON As String = resourceUUID & ".json"

            'Build the URI we need for the API request
            Dim request As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Post,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_ShareSimulate, ResourceJSON, _apiVersion, Nothing, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return SimulateShareResult.FailedCreateURL
            End If

            'Build JSON dictionary
            Dim newPermissions As New Dictionary(Of String, Object)
            For X As Integer = 0 To ShareUsers.Count - 1
                Dim newPermission As New Dictionary(Of String, String) From {
                    {cIsNew, True},
                    {cAro, cUser},
                    {cAroForeignKey, ShareUsers(X).UserUUID},
                    {cAco, cResource},
                    {cAcoForeignKey, resourceUUID},
                    {cPermissionType, "1"}
                }

                newPermissions.Add(X, newPermission)
            Next

            Dim newShare As New Dictionary(Of String, Object) From {
                {cPermissions, newPermissions}
            }

            'Get crfs token value
            Dim Cookies = CookieJar.GetCookies(New System.Uri(_serverURI))
            Dim crfsValue As String = ""
            For Each newCookie As Cookie In Cookies
                If newCookie.Name.Contains("csrf") Then
                    crfsValue = newCookie.Value
                End If
            Next

            'Add headers
            request.Headers.Add("Accept", "application/json")
            request.Headers.Add("User-Agent", "Passbolt.NET/1.0")
            request.Headers.Add("X-CSRF-Token", crfsValue)

            'Create JSON string and send request
            request.Content = New StringContent(JsonConvert.SerializeObject(newShare), Encoding.UTF8, "application/json")
            Dim requestResult = Await pbClient.SendAsync(request)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await requestResult.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            'Read response and save the userIDs, if we get an error try to log the error
            If requestResult.IsSuccessStatusCode Then
                If jsData.Count > 1 Then
                    For Each jData In jsData
                        If jData.Key = cBody Then
                            Dim bData = jData.Value

                            If bData.changes.added.Count > 0 Then
                                For Each sData In bData.changes.added
                                    For Each aroUser In AroUserList.Values
                                        If aroUser(cId) = sData.user.id Then
                                            SimulateShareList.Add(sData.user.id)
                                            Exit For
                                        End If
                                    Next
                                Next

                                Return SimulateShareResult.Success
                            Else
                                'Body contains no data
                                Return SimulateShareResult.FailedNoDataBody
                            End If
                        End If
                    Next
                Else
                    'Request contains no data
                    Return SimulateShareResult.FailedNoData
                End If
            Else
                'HTTP request failed
                Return SimulateShareResult.FailedRequestFailed
            End If

            'Request contains no data, jsData never contained a body key
            Return SimulateShareResult.FailedNoData
        Catch ex As Exception
            _FunctionException = ex
            Return SimulateShareResult.FailedError
        End Try
    End Function

    ''' <summary>
    ''' Returns a list of UUIDs after calling the SimulateShare function
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Function GetSimulateResult() As List(Of String)
        Try
            Return SimulateShareList
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Shares a resource with other users
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Async Function Share(resourceUUID As String, ShareUsers() As Users, passphrase As String) As Task(Of ShareResult)
        Try
            'Clear last error
            _FunctionException = Nothing

            ShareList = New List(Of String)
            Dim ResourceJSON As String = resourceUUID & ".json"

            'Build the URI we need for the API request
            Dim request As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Put,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_ShareResource, ResourceJSON, _apiVersion, Nothing, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return SimulateShareResult.FailedCreateURL
            End If

            'Build JSON dictionary
            Dim newPermissions As New List(Of Object)
            Dim newSecrets As New List(Of Object)

            'Get plaintext password for encrypting for other users
            Dim plain = Await GetPlainTextPassword(resourceUUID, passphrase)

            'Fill in JSON with an encrypted resource for each user
            For X As Integer = 0 To ShareUsers.Count - 1
                'Create permission dictionary for the user
                Dim newPermission = New Dictionary(Of String, Object) From {
                    {cIsNew, True},
                    {cAro, cUser},
                    {cAroForeignKey, ShareUsers(X).UserUUID},
                    {cAco, cResource},
                    {cAcoForeignKey, resourceUUID},
                    {cPermissionType, "1"}
                }

                'Get GPGkey of user
                Dim users = GetUsers()
                Dim GpgKey = ShareUsers(X).GPGKey

                Dim EncSecret = Await GetEncryptedJSON(plain.Password, plain.Description, passphrase, GpgKey)

                'Create secret dictionary for the user
                Dim newSecret As New Dictionary(Of String, String) From {
                    {cUserID, ShareUsers(X).UserUUID},
                    {cData, EncSecret}
                }

                'Add the user to the JSON dictionary
                newPermissions.Add(newPermission)
                newSecrets.Add(newSecret)
            Next

            'Build Share JSON
            Dim newShare As New Dictionary(Of String, Object) From {
                {cPermissions, newPermissions},
                {cSecrets, newSecrets}
            }

            'Get crfs token value
            Dim Cookies = CookieJar.GetCookies(New System.Uri(_serverURI))
            Dim crfsValue As String = ""
            For Each newCookie As Cookie In Cookies
                If newCookie.Name.Contains("csrf") Then
                    crfsValue = newCookie.Value
                End If
            Next

            'Add headers
            request.Headers.Add("Accept", "application/json")
            request.Headers.Add("User-Agent", "Passbolt.NET/1.0")
            request.Headers.Add("X-CSRF-Token", crfsValue)

            'Create JSON string and send request
            Dim jsString As String = JsonConvert.SerializeObject(newShare)
            request.Content = New StringContent(JsonConvert.SerializeObject(newShare), Encoding.UTF8, "application/json")
            Dim requestResult = Await pbClient.SendAsync(request)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await requestResult.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            If requestResult.IsSuccessStatusCode Then
                Return ShareResult.Success
            Else
                Return ShareResult.FailedRequestFailed
            End If
        Catch ex As Exception
            _FunctionException = ex
            Return SimulateShareResult.FailedError
        End Try
    End Function


    ''' <summary>
    ''' Removes user(s) from a shared resource
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Async Function RemoveShare(resourceUUID As String, RemoveUsers() As Users) As Task(Of RemoveShareResult)
        Try
            'Clear last error
            _FunctionException = Nothing

            Dim ResourceJSON As String = resourceUUID & ".json"
            Dim FilterRequest As New StringBuilder
            FilterRequest.Append("&filter[has-id][]=").Append(resourceUUID).Append("&contain[permissions.user.profile]=1")

            'Build the URI we need for the API request
            Dim GetResources As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Get,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_Resources, Nothing, _apiVersion, FilterRequest.ToString, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return RemoveShareResult.FailedCreateURL
            End If

            'Send request
            Dim result = Await pbClient.SendAsync(GetResources)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await result.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            'Create permissions dictionary
            Dim prData As New Dictionary(Of String, Object)
            Dim prIndex As Integer = 0

            'If the object has more then 1 value we can read it (header and body)
            If jsData.Count > 1 Then
                For Each jData In jsData
                    'Read the content of the body
                    If jData.Key.ToLower = cBody Then
                        If jData.Value.Count > 0 Then
                            Dim bData = jData.Value

                            'Loop through all users that have permission to get the JSON values
                            For Each uData In bData(0).permissions
                                For Each AroUser In RemoveUsers
                                    'If the user we want to remove exists in the permissions, save the permissions JSON for future use
                                    If AroUser.UserUUID = uData.user.id Then
                                        uData.delete = True
                                        prData.Add(prIndex, uData)
                                        prIndex += 1
                                        Exit For
                                    End If
                                Next
                            Next
                        Else
                            'Body did not contain any data
                            Return RemoveShareResult.FailedNoDataBody
                        End If
                    End If
                Next
            Else
                'Response did not contain any data
                Return RemoveShareResult.FailedNoData
            End If

            'Create JSON request
            Dim remShare As New Dictionary(Of String, Object) From {
                {cPermissions, prData}
            }

            'Build the URI we need for the API request
            Dim request As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Put,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_ShareResource, ResourceJSON, _apiVersion, Nothing, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return RemoveShareResult.FailedCreateURL
            End If

            'Get crfs token value
            Dim Cookies = CookieJar.GetCookies(New System.Uri(_serverURI))
            Dim crfsValue As String = ""
            For Each newCookie As Cookie In Cookies
                If newCookie.Name.Contains("csrf") Then
                    crfsValue = newCookie.Value
                End If
            Next

            'Add headers
            request.Headers.Add("Accept", "application/json")
            request.Headers.Add("User-Agent", "Passbolt.NET/1.0")
            request.Headers.Add("X-CSRF-Token", crfsValue)

            'Create JSON string and send request
            request.Content = New StringContent(JsonConvert.SerializeObject(remShare), Encoding.UTF8, "application/json")
            Dim requestResult = Await pbClient.SendAsync(request)

            'Read content for debug purposes
            Dim DataResult = Await requestResult.Content.ReadAsStringAsync

            'Return request result
            If requestResult.IsSuccessStatusCode Then
                Return RemoveShareResult.Success
            Else
                Return RemoveShareResult.FailedRequestFailed
            End If
        Catch ex As Exception
            _FunctionException = ex
            Return RemoveShareResult.FailedError
        End Try
    End Function

    ''' <summary>
    ''' Shares a resource with other users
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Async Function UpdateResource(resourceID As String, Name As String, Username As String, UpdatedSecret As String, Description As String, URI As String, passphrase As String) As Task(Of UpdateResourceResult)
        Try
            'Clear last error
            _FunctionException = Nothing

            Dim ResourceJSON As String = resourceID & ".json"
            Dim FilterRequest As New StringBuilder

            'Get list of users that have access to the selected resource
            FilterRequest.Append("&filter[has-access]=").Append(resourceID)
            Dim GetResources As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Get,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_Users, Nothing, _apiVersion, FilterRequest.ToString, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return UpdateResourceResult.FailedCreateURL
            End If

            'Send request
            Dim result = Await pbClient.SendAsync(GetResources)

            'Read the response and parse it into a JSON dynamic object
            Dim Data = Await result.Content.ReadAsStringAsync()
            Dim jsData = JsonConvert.DeserializeObject(Of ExpandoObject)(Data, New ExpandoObjectConverter)

            'Make a dictionary containing the userdata
            Dim Users As New Dictionary(Of String, String)

            For Each jData In jsData
                If jData.Key.ToLower = "body" Then
                    'The body contains an array, if it exists loop through it
                    If jData.Value.Count > 0 Then
                        For Each bData In jData.Value
                            Users.Add(bData.id, bData.gpgkey.armored_key)
                        Next
                    End If
                End If
            Next

            'Get the resource data of the selected resource
            Dim ResourceData = Await GetResourceData(resourceID)

            'Build a new dictionary for the JSON string to update the resource
            Dim LoginRequest = New Dictionary(Of String, Object) From {
                {cDescription, ResourceData(cDescription)},
                {cId, ResourceData(cId)},
                {cName, Name},
                {cResourceTypeId, ResourceData(cResourceTypeId)},
                {cSecrets, New Dictionary(Of String, Object)},
                {cUri, URI},
                {cUsername, Username}
            }

            'If we found users generate pgp messages for each of them
            If Users.Count > 0 Then
                Dim x As Integer = 0
                For Each UserKey In Users
                    Dim newUserSecret As New Dictionary(Of String, String) From {
                        {cData, Await GetEncryptedJSON(UpdatedSecret, Description, passphrase, UserKey.Value)},
                        {cUserID, UserKey.Key}
                    }
                    LoginRequest(cSecrets).Add(x, newUserSecret)
                    x += 1
                Next
            Else
                Return UpdateResourceResult.FailedNoUsers
            End If

            'Build the http request to update the resource
            Dim request As HttpRequestMessage = New HttpRequestMessage() With {
                .Method = HttpMethod.Put,
                .RequestUri = New Uri(CreateURL(_serverURI, URI_ResourcesSpecific, ResourceJSON, _apiVersion, Nothing, _FunctionException))
            }

            'If we got an error return error index
            If _FunctionException IsNot Nothing Then
                Return UpdateResourceResult.FailedCreateURL
            End If

            'Get crfs token value
            Dim Cookies = CookieJar.GetCookies(New System.Uri(_serverURI))
            Dim crfsValue As String = ""
            For Each newCookie As Cookie In Cookies
                If newCookie.Name.Contains("csrf") Then
                    crfsValue = newCookie.Value
                End If
            Next

            'Add headers
            request.Headers.Add("Accept", "application/json")
            request.Headers.Add("User-Agent", "Passbolt.NET/1.0")
            request.Headers.Add("X-CSRF-Token", crfsValue)

            'Send request to update the resource
            Dim DebugString = JsonConvert.SerializeObject(LoginRequest)
            request.Content = New StringContent(JsonConvert.SerializeObject(LoginRequest), Encoding.UTF8, "application/json")
            Dim requestResult = Await pbClient.SendAsync(request)

            'Read response for debug purposes
            Dim DataResult = Await requestResult.Content.ReadAsStringAsync

            'Check if request succeeded, else return error
            If requestResult.IsSuccessStatusCode Then
                Return UpdateResourceResult.Success
            Else
                Return UpdateResourceResult.FailedRequest
            End If

        Catch ex As Exception
            Return UpdateResourceResult.FailedError
        End Try
    End Function

    ''' <summary>
    ''' Gets a list of users, please call the function GetAros() before calling this
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Function GetUsers() As List(Of Users)
        Try
            If AroUserList IsNot Nothing AndAlso AroUserList.Count > 0 Then
                UserList = New List(Of Users)
                Dim UserIndex As Integer = 0
                For Each AroUser In AroUserList
                    Dim UserData = AroUser.Value

                    Dim newUser As New Users With {
                        .ID = UserIndex,
                        .UserUUID = UserData(cId),
                        .Username = UserData(cUsername),
                        .UserFullName = UserData(cFirstName) & " " & UserData(cLastName),
                        .UserFirstName = UserData(cFirstName),
                        .UserLastName = UserData(cLastName),
                        .GPGKey = UserData(cGpgKey),
                        .GroupUUIDs = Array.Empty(Of String)
                    }

                    UserList.Add(newUser)
                    UserIndex += 1
                Next
                Return UserList
            Else
                Return Nothing
            End If
        Catch ex As Exception
            _FunctionException = ex
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Gets a list of resources, please call the function GetAcos() before calling this
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Function GetResources() As List(Of Resources)
        Try
            If AcoResourceList IsNot Nothing AndAlso AcoResourceList.Count > 0 Then
                ResourceList = New List(Of Resources)

                Dim ResourceIndex As Integer = 0
                For Each AcoResource In AcoResourceList
                    Dim ResourceData = AcoResource.Value

                    Dim newResource As New Resources With {
                        .ID = ResourceIndex,
                        .ResourceUUID = ResourceData(cId),
                        .ResourceName = ResourceData(cName),
                        .ResourceUsername = ResourceData(cUsername),
                        .ResourceURI = ResourceData(cUri),
                        .ResourceDescription = ResourceData(cDescription),
                        .ResourceDeleted = ResourceData(cDeleted),
                        .ResourceCreated = ResourceData(cCreated),
                        .ResourceModified = ResourceData(cModified),
                        .ResourceCreatedBy = ResourceData(cCreatedBy),
                        .ResourceModifiedBy = ResourceData(cModifiedBy),
                        .ResourceTypeID = ResourceData(cResourceTypeId)
                    }

                    ResourceList.Add(newResource)
                    ResourceIndex += 1
                Next
                Return ResourceList
            Else
                Return New List(Of Resources)
            End If
        Catch ex As Exception
            _FunctionException = ex
            Return Nothing
        End Try
    End Function

    Public Function GetResourceShareList() As List(Of Users)
        Try
            If AcoSharesList IsNot Nothing AndAlso AcoSharesList.Count > 0 Then
                Dim ShareList As New List(Of Users)

                For Each AcoShare In AcoSharesList
                    Dim Share = AcoShare.Value
                    Dim UserList = GetUsers()

                    'Find User in Userlist
                    For Each User In UserList
                        If Share.UserUUID = User.UserUUID Then
                            ShareList.Add(User)
                            Exit For
                        End If
                    Next
                Next

                Return ShareList
            Else
                Return New List(Of Users)
            End If
        Catch ex As Exception
            _FunctionException = ex
            Return Nothing
        End Try
    End Function
#End Region
End Class
