<Cmdlet(VerbsCommon.Clear, TagNounes.TagCache, supportsshouldprocess:=False)> _
Public Class Clear_TagCache : Inherits CmdLetBase
    Protected Overrides Sub DoProcessRecord()
        Tag.ClearCache()
    End Sub
End Class
