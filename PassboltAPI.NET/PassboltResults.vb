Public Class PassboltResults
    Public Enum LoginResult
        Success
        FailedCreateURL
        FailedUserKey
        FailedDecrypt
        FailedToken
        FailedWrongUser
        FailedError
    End Enum
    Public Enum LogoutResult
        Success
        FailedError
    End Enum

    Public Enum CreateResourceResult
        Success
        FailedCreateURL
        FailedNoResourceType
        FailedNoID
        FailedError
    End Enum

    Public Enum GetAcosResult
        Success
        FailedCreateURL
        FailedNoResources
        FailedNoData
        FailedError
    End Enum

    Public Enum GetArosResult
        Success
        FailedCreateURL
        FailedNoData
        FailedError
    End Enum

    Public Enum GetSharesResult
        Success
        FailedCreateURL
        FailedNoData
        FailedError
    End Enum

    Public Enum SimulateShareResult
        Success
        FailedCreateURL
        FailedNoData
        FailedNoDataBody
        FailedRequestFailed
        FailedError
    End Enum

    Public Enum ShareResult
        Success
        FailedCreateURL
        FailedRequestFailed
        FailedError
    End Enum

    Public Enum RemoveShareResult
        Success
        FailedCreateURL
        FailedNoData
        FailedNoDataBody
        FailedRequestFailed
        FailedError
    End Enum

    Public Enum UpdateResourceResult
        Success
        FailedCreateURL
        FailedNoUsers
        FailedRequest
        FailedError
    End Enum
End Class
