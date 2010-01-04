<Cmdlet(TagVerbs.Reset, TagNounes.Tag)> _
Public Class Reset_Tag : Inherits EditTagCmdLetBase
    Protected Overrides Function ProcessEditTag(ByVal TargetTag As Tag) As Boolean
        TargetTag.Reset()
        Return False
    End Function
End Class
