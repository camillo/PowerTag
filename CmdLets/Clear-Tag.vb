<Cmdlet(VerbsCommon.Clear, TagNounes.Tag, _
        SupportsShouldProcess:=True, DefaultParameterSetName:=Clear_Tag.DefaultParameterSetName)> _
Public Class Clear_Tag : Inherits EditTagCmdLetBase

#Region "process record"
    Protected Overrides Sub ProcessEditTag(ByVal TargetTag As Tag)
        TargetTag.Clear()
    End Sub
#End Region

End Class
