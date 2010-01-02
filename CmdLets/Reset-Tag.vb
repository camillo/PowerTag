<Cmdlet(TagVerbs.Reset, TagNounes.Tag)> _
Public Class Reset_Tag : Inherits EditTagCmdLetBase
    Protected Overrides Function ProcessEditTag(ByVal TargetFile As TagLib.File) As Boolean
        Dim filename = TargetFile.Name
        If FileCache.Contains(filename) Then
            Me.WriteVerbose("delete '{0}' from cache", filename)
            FileCache.Remove(filename)
        End If
        Return False
    End Function
End Class
