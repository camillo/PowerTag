Partial Public Class Tag
    Private Shared Cache As New TagCache
    Private Shared SupportedExtensions As New List(Of String)(TagLib.SupportedMimeType.AllExtensions)

    Friend Shared Function Create(ByVal Filename As String) As Tag
        Dim back As Tag = Nothing
        Dim Fullname = System.IO.Path.GetFullPath(Filename)
        SyncLock Cache
            If Not Cache.TryGetValue(Fullname, back) Then
                back = New Tag(Fullname)
                Cache.Add(Filename, back)
            End If
        End SyncLock
        Return back
    End Function

    Friend Shared Function HasSupportedExtension(ByVal Filename As String) As Boolean
        Dim back As Boolean = False
        Dim extension = System.IO.Path.GetExtension(Filename)
        If Not String.IsNullOrEmpty(extension) Then
            back = SupportedExtensions.Contains(extension.Substring(1))
        End If
        Return back
    End Function

End Class
