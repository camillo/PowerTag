<Cmdlet(VerbsCommon.Clear, TagNounes.Tag, _
        SupportsShouldProcess:=True, DefaultParameterSetName:=Clear_Tag.DefaultParameterSetName)> _
Public Class Clear_Tag : Inherits EditTagCmdLetBase
    Protected Overrides Function ProcessEditTag(ByVal TargetTag As Tag) As Boolean
        TargetTag.Clear()
        Return True
    End Function
End Class
