<Cmdlet(VerbsCommon.Clear, TagNounes.TagCache, supportsshouldprocess:=False)> _
Public Class Clear_TagCache : Inherits CmdLetBase

#Region "process record"
    Protected Overrides Sub DoProcessRecord()
        Tag.ClearCache()
    End Sub
#End Region

End Class
