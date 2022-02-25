<div id="top"></div>

# PassboltAPI.NET
Passbolt is an opensource password manager and as a HTTPS REST API.
This project is developed to use this API inside a .NET application.

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#dependencies">Dependencies</a></li>
        <li><a href="#imports">Imports</a></li>
      </ul>
    </li>
    <li><a href="#possibilities">Possibilities</a></li>
    <li><a href="#todo">ToDo</a></li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
    <li><a href="#remarks">Remarks</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

Passbolt is a great open source solution for a password manager for teams. They developed a HTTPS REST API to interact with the passbolt server, local or remotely hosted. They even support a community edition which is free to use. I wanted to use passbolt as a password manager for the company i work for, however most users are used to a windows based application. And i wanted to accomodate those users by making this application.

However i noticed that Passbolt did not have samples or a guide for .NET application. So i decided to write one myself, following the guide on the Passbolt API website and following tips from developers/users on the forums. I managed to create a .NET class that handles the basics of Passbolt.
As Passbolt itself is opensource i decided to share my own code as well, perhaps some will find it helpful for themselves.

I have used Visual Studio 2022 Community Edition 17.0.4 to develop this library.
<p align="right">(<a href="#top">back to top</a>)</p>

## Getting Started

### Dependencies
``` vb.net
   Newtonsoft.JSON 13.0.1
   PgpCode 5.5.0
```
### Imports
``` vb.net
   Imports Passbolt.NET
   Imports Passbolt.NET.PassboltResults
   Imports Passbolt.NET.Structs
```
<p align="right">(<a href="#top">back to top</a>)</p>

## Possibilities

- [ ] Verify server certificate, now all are accepted due to using a self signed certificate on a local server
- [x] Verify server fingerprint
- [x] Login a user with their private key and passphrase
- [x] Get a list of available users on the passbolt server
- [x] Adding a new password resource
- [x] Changing the password and update it for all shared users
- [x] Sharing a password resource with other users
- [x] Removing a user from the shared list
- [ ] Adding a new user
- [ ] Removing a user
- [ ] Deleting a password resource
- [ ] Working with user groups
- [ ] Multi factor authentication
- [ ] Detecting that the server logged out the user
<p align="right">(<a href="#top">back to top</a>)</p>

## ToDo
- [ ] Handling self signed certificates for local servers
- [ ] Detecting that the server logged out the user
- [ ] Working with user groups
<p align="right">(<a href="#top">back to top</a>)</p>

## Usage
Verifying the server fingerprint
``` vb.net
   Private Async Sub VerifyServer(serverURL as String, serverFingerprint as String, APIVersion as Integer)
        Dim Passbolt = New PassboltAPI(serverURL, serverFingerprint, APIVersion)
        
        If Await Passbolt.VerifyServer() Then
            'Success
        Else
            'Failed
        End If
   End Sub
```
<p align="right">(<a href="#top">back to top</a>)</p>

Logging in a user, either load the users private key from a file or secure store it in the settings. Same with the passphrase
``` vb.net
   Private Async Sub LoginUser(serverURL as String, serverFingerprint as String, APIVersion as Integer, userPrivateKeyFile as string, userFingerprint as String, userPassphrase as String)
	Dim Passbolt = New PassboltAPI(serverURL, serverFingerprint, APIVersion)

	'Load the private key file of the user you want to log in
	Passbolt.userPrivateKey = System.IO.File.ReadAllText(userPrivateKeyFile, Encoding.UTF8)

	If Await Passbolt.LoginUser(userFingerprint, userPassphrase) = LoginResult.Success Then
		'Success
	Else
		'Failed
	End If
   End Sub
```
<p align="right">(<a href="#top">back to top</a>)</p>

Load the available user list
``` vb.net
   Private Async Sub LoadUsers()        
	If Await Passbolt.GetAros() = GetArosResult.Success Then
		'Success, now we can read the resulted user list
		Dim UserList as List(Of Passbolt.NET.Structs.Users) = Passbolt.GetUsers

		For Each User In UserList
			'Read all user info you would like
			Dim CurrentUserFullName = User.UserFullName
			Dim CurrentUserUUID = Resource.UserUUID
		Next
	Else
		'Failed
	End If
   End Sub
```
<p align="right">(<a href="#top">back to top</a>)</p>

Load the available resource list
``` vb.net
   Private Async Sub LoadResources()       
   	Dim result = Await Passbolt.GetAcos(False) 'Owned and shared resources, True for all passwords even those that the user does not have access to
        If result = GetAcosResult.Success Then
		'Success, now we can read the resulted user list
		Dim ResourceList as List(Of Passbolt.NET.Structs.Resource) = Passbolt.GetResources

		For Each Resource In ResourceList
			'Read all resource info you would like
			Dim CurrentResourceName = Resource.ResourceName
			Dim CurrentResourceUUID = Resource.ResourceUUID
    		Next
	ElseIf result = GetAcosResult.FailedNoResources Then
		'User does not have access to any resources
	ElseIf Passbolt.readException IsNot Nothing Then
		'In case an exception was raised you can read here what happened
		Dim ex as Exception = Passbolt.readException()
	Else
		'Failed
	End If
   End Sub
```
<p align="right">(<a href="#top">back to top</a>)</p>

Add a new resource
``` vb.net
   Private Async Sub AddResource(passphrase as String)		
	Dim result = Await Passbolt.CreateResource(NewResourceName,
                                                   NewResourceUsername,
                                                   NewResourcePassword,
                                                   NewResourceDescription,
                                                   NewResourceURL,
                                                   passphrase)
        
        If result = CreateResourceResult.Success Then
		'Success, the new resource is added
	ElseIf Passbolt.readException IsNot Nothing Then
		'In case an exception was raised you can read here what happened
		Dim ex as Exception = Passbolt.readException()
	Else
		'Failed
	End If
   End Sub
```
<p align="right">(<a href="#top">back to top</a>)</p>

Change an existing resource
``` vb.net
   Private Async Sub ChangeResource(ResourceUUID as String, passphrase as String)
	'Changing a resource password requires the UUID of the resource, this can be retrieved from the LoadResources() function
	Dim result = Await Passbolt.UpdateResource(ResourceUUID,
						   NewResourceName,
						   NewResourceUsername,
						   UpdatedResourcePassword,
						   NewResourceDescription,
						   NewResourceURL,
						   passphrase)
        
        If result = UpdateResourceResult.Success Then
		'Success, the new resource is added
	ElseIf Passbolt.readException IsNot Nothing Then
		'In case an exception was raised you can read here what happened
		Dim ex as Exception = Passbolt.readException()
	Else
		'Failed
	End If
   End Sub
```
<p align="right">(<a href="#top">back to top</a>)</p>

Sharing an existing resource with other users
``` vb.net
   Private Async Sub ShareResource(ResourceUUID as String, UsersUUID() as String, passphrase as String)
	'Sharing a resource password requires the UUID of the resource, this can be retrieved from the LoadResources() function
		
	'First we simulate a share to see if the Passbolt server accepts the request
	Dim SimulateResult = Await Passbolt.SimulateShare(ResourceUUID, UsersUUID)
		
	If SimulateResult = SimulateShareResult.Success Then
		'Simulate succeeded, we can now share the resource
		Dim ShareResult = Await Passbolt.Share(ResourceUUID, UsersUUID, passphrase)
			
		If ShareResult = ShareResult.Success Then
			'Success
		ElseIf Passbolt.readException IsNot Nothing Then
			'In case an exception was raised you can read here what happened
			Dim ex as Exception = Passbolt.readException()
		Else
			'Failed
		End if
	ElseIf Passbolt.readException IsNot Nothing Then
		'In case an exception was raised you can read here what happened
		Dim ex as Exception = Passbolt.readException()
	Else
		'Failed
	End If
   End Sub
```
<p align="right">(<a href="#top">back to top</a>)</p>

Removing a share for an existing resource
``` vb.net
   Private Async Sub RemoveShare(ResourceUUID as String, UsersUUID() as String)
	'Sharing a resource password requires the UUID of the resource, this can be retrieved from the LoadResources() function
		
	Dim RemoveResult = Await Passbolt.RemoveShare(ResourceUUID, UsersUUID)
		
	If RemoveResult = RemoveShareResult.Success Then
		'Remove share succeeded
	ElseIf Passbolt.readException IsNot Nothing Then
		'In case an exception was raised you can read here what happened
		Dim ex as Exception = Passbolt.readException()
	Else
		'Failed
	End If
   End Sub
```
<p align="right">(<a href="#top">back to top</a>)</p>

## License
This project is shared under the GNU General Public License v3
[GNU GPLv3](https://www.gnu.org/licenses/gpl-3.0.html)
<p align="right">(<a href="#top">back to top</a>)</p>

## Acknowledgments
The Passbolt team for supplying an open source Password manager with an available API.
The Passbolt Forum for aiding me on issues i ran into and in using the API calls.
<p align="right">(<a href="#top">back to top</a>)</p>

## Remarks
I have developed this library using a localized instance of Passbolt CE on a VM running Ubuntu 20.04.
The plan is to have a small Ubuntu server running Passbolt CE, still in the local network.
Perhaps in the future i will transfer it to a remote server.
<p align="right">(<a href="#top">back to top</a>)</p>
