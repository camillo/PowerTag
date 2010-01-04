Partial Public Class Tag
    Private Shared Cache As New TagCache
    Private Shared SupportedExtensions As New List(Of String)(TagLib.SupportedMimeType.AllExtensions)

    Friend Shared Function Create(ByVal Filename As String) As Tag
        Return Create(Filename, False)
    End Function

    Friend Shared Function Create(ByVal Filename As String, ByVal Force As Boolean) As Tag
        Dim back As Tag = Nothing
        Dim Fullname = System.IO.Path.GetFullPath(Filename)
        If Force Then
            WriteVerbose("force read on disk; ignore cache; not updating cache")
            back = DoCreate(Fullname)
        Else
            SyncLock Cache
                If Not Cache.TryGetValue(Fullname, back) Then
                    WriteVerbose("cache miss")
                    back = DoCreate(Fullname)
                    Cache.Add(Filename, back)
                Else
                    WriteVerbose("cache hit", Fullname)
                End If
            End SyncLock
        End If
        Return back
    End Function

    Private Shared Function DoCreate(ByVal Fullname As String) As Tag
        Dim back As Tag
        Dim file = TagLib.File.Create(Fullname)
        Select Case file.Properties.MediaTypes
            Case TagLib.MediaTypes.None
                WriteWarning("detected type: none")
                back = New Tag(file)
            Case TagLib.MediaTypes.Audio
                WriteVerbose("detected type: audio")
                back = New AudioTag(file)
            Case TagLib.MediaTypes.Video
                WriteVerbose("detected type: video")
                back = New VideoTag(file)
            Case (TagLib.MediaTypes.Audio Or TagLib.MediaTypes.Video)
                WriteVerbose("detected type: audio/video")
                back = New AudioVideoTag(file)
            Case Else
                WriteWarning("Unknown MediaTypes: '{0}'", file.Properties.MediaTypes)
                back = New Tag(file)
        End Select
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

    Friend Shared Sub ClearCache()
        SyncLock Cache
            WriteVerbose("clearing cache ({0} items)", Cache.Count)
            Cache.Clear()
        End SyncLock
    End Sub
End Class
