Public Class FileCache : Inherits Dictionary(Of String, TagLib.File)
    Private Shared Instance As FileCache = New FileCache
    Private Sub New()
    End Sub

    Public Shared Function GetFile(ByVal FileName As String) As TagLib.File
        Dim back As TagLib.File = Nothing
        If Not Instance.TryGetValue(FileName, back) Then
            If Not System.IO.File.Exists(FileName) Then Throw New System.IO.FileNotFoundException("File not found", FileName)
            back = TagLib.File.Create(FileName)
            If back Is Nothing Then Throw New TagLibException("TagLib.File.Create does not return a file object")
            Instance.Add(FileName, back)
        End If
        Return back
    End Function

    Public Shared Function GetFile(ByVal Tag As TagLib.Tag) As TagLib.File
        Dim back As TagLib.File = Nothing
        For Each kvp In Instance
            If kvp.Value.Tag Is Tag Then
                back = kvp.Value
                Exit For
            End If
        Next
        If back Is Nothing Then Throw New FileNotFoundException("No file found for given tag.")
        Return back
    End Function

    <Serializable()> _
    Public Class FileNotFoundException
        Inherits System.Exception

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
        End Sub

        Public Sub New( _
            ByVal info As System.Runtime.Serialization.SerializationInfo, _
            ByVal context As System.Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class

End Class
