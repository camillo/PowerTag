<Cmdlet(TagVerbs.Save, TagNounes.Tag, SupportsShouldProcess:=False)> _
Public Class Save_Tag : Inherits EditTagCmdLetBase
    Protected Overrides Function ProcessEditTag(ByVal TargetTag As Tag) As Boolean
        Return True
    End Function
End Class
