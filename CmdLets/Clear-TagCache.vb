<Cmdlet(VerbsCommon.Clear, TagNounes.TagCache, supportsshouldprocess:=False)> _
Public Class Clear_TagCache : Inherits CmdLetBase
    Protected Overrides Sub ProcessRecord()
        FileCache.Clear()
        Me.WriteVerbose("FileCache cleared")
    End Sub
End Class
