Class PassboltConstants
    'Server Urls
    Public Const URI_ServerVerify As String = "auth/verify.json"
    Public Const URI_ServerLogin As String = "auth/login.json"
    'User Urls
    Public Const URI_Users As String = "users.json"
    Public Const URI_User As String = "users/me.json"
    'Resource Urls
    Public Const URI_Resources As String = "resources.json"
    Public Const URI_ResourcesSpecific As String = "resources/"
    Public Const URI_ResourceTypes As String = "resource-types.json"
    Public Const URI_ResourceSecret As String = "secrets/resource/"
    'Share Urls
    Public Const URI_ShareAros As String = "share/search-aros.json"
    Public Const URI_ShareSimulate As String = "share/simulate/resource/"
    Public Const URI_ShareResource As String = "share/resource/"
    'Permission Urls
    Public Const URI_PermissionsResource As String = "permissions/resource/"

    'Passbolt API JSON Keys.
    'I've made a global constant list. Incase we need to change these values it is just this list we need to adjust.
    Public Const cHeader As String = "header"
    Public Const cBody As String = "body"
    Public Const cGpgAuth As String = "gpg_auth"
    Public Const cGpgKey As String = "gpg_key"
    Public Const cKeyId As String = "keyid"
    Public Const cUserTokenResult As String = "user_token_result"
    Public Const cGpgTokenCookie As String = "X-GPGAuth-User-Auth-Token"
    Public Const cSecret As String = "secret"
    Public Const cPassword As String = "password"
    Public Const cPasswordDescription As String = "password-and-description"
    Public Const cDeleted As String = "deleted"
    Public Const cCreated As String = "created"
    Public Const cModified As String = "modified"
    Public Const cCreatedBy As String = "created-by"
    Public Const cModifiedBy As String = "modified-by"
    Public Const cFolderParentId As String = "folder_parent_id"
    Public Const cPersonal As String = "personal"
    Public Const cUserCount As String = "user_count"
    Public Const cUserFullName As String = "user_fullname"
    Public Const cUserGroups As String = "user_groups"
    Public Const cFirstName As String = "first_name"
    Public Const cLastName As String = "last_name"

    'Resource
    Public Const cDescription As String = "description"
    Public Const cId As String = "id"
    Public Const cName As String = "name"
    Public Const cResourceTypeId As String = "resource_type_id"
    Public Const cSecrets As String = "secrets"
    Public Const cUri As String = "uri"
    Public Const cUsername As String = "username"
    Public Const cData As String = "data"
    Public Const cUserID As String = "user_id"

    'Share
    Public Const cPermissions As String = "permissions"
    Public Const cUser As String = "User"
    Public Const cResource As String = "Resource"
    Public Const cIsNew As String = "is_new"
    Public Const cAro As String = "aro"
    Public Const cAroForeignKey As String = "aro_foreign_key"
    Public Const cAco As String = "aco"
    Public Const cAcoForeignKey As String = "aco_foreign_key"
    Public Const cPermissionType As String = "type"


End Class
