<Cmdlet(TagVerbs.Reset, TagNounes.Tag)> _
Public Class Reset_Tag : Inherits EditTagBase
    Protected Overrides Function DoEdit(ByVal TargetFile As TagLib.File) As Boolean
        Dim filename = TargetFile.Name
        If FileCache.Contains(filename) Then
            Me.WriteVerbose("delete '{0}' from cache", filename)
            FileCache.Remove(filename)
        End If
        Return False
    End Function
End Class
