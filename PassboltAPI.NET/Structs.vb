Public Class Structs
    Public Structure Users
        Public ID As Integer
        Public Username As String
        Public UserFullName As String
        Public UserFirstName As String
        Public UserLastName As String
        Public UserUUID As String
        Public GPGKey As String
        Public GroupUUIDs() As String
    End Structure

    Public Structure Resources
        Public ID As Integer
        Public ResourceUUID As String
        Public ResourceName As String
        Public ResourceUsername As String
        Public ResourceURI As String
        Public ResourceDescription As String
        Public ResourceDeleted As Boolean
        Public ResourceCreated As String
        Public ResourceModified As String
        Public ResourceCreatedBy As String
        Public ResourceModifiedBy As String
        Public ResourceTypeID As String
    End Structure

    Public Structure Secret
        Public Password As String
        Public Description As String
    End Structure
End Class
