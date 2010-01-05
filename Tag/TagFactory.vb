Partial Public Class Tag
    Private Shared Cache As New TagCache
    Private Shared SupportedExtensions As New List(Of String)(TagLib.SupportedMimeType.AllExtensions)

    Friend Shared Function Create(ByVal Filename As String, ByVal SessionPath As String) As Tag
        Return Create(Filename, SessionPath, False)
    End Function

    Friend Shared Function Create(ByVal Filename As String, ByVal SessionPath As String, ByVal Force As Boolean) As Tag
        Dim back As Tag = Nothing
        Dim Fullname = Util.GetFullPath(Filename, SessionPath)

        If Force Then
            WriteVerbose("force read on disk; ignore cache; not updating cache")
            back = DoCreate(Fullname)
        Else
            SyncLock Cache
                If Not Cache.TryGetValue(Fullname, back) Then
                    WriteVerbose("cache miss")
                    back = DoCreate(Fullname)
                    Cache.Add(Fullname, back)
                Else
                    WriteVerbose("cache hit")
                End If
            End SyncLock
        End If
        Return back
    End Function

    Friend Shared Function ExchangeIfNeeded(ByVal Item As Tag) As Tag
        Dim back As Tag = Nothing
        Dim FullName = Item.FullName
        SyncLock Cache
            If Not Cache.TryGetValue(FullName, back) Then
                back = DoCreate(FullName)
                Cache.Add(FullName, back)
            End If
        End SyncLock
        If Not back Is Item Then WriteVerbose("Tagobject changed; return new object")
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

    Friend Shared Function GetCachedTags() As IEnumerable(Of Tag)
        Dim back = New List(Of Tag)(Cache.Values)
        Return back
    End Function
End Class
