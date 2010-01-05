<Cmdlet(TagVerbs.Save, TagNounes.Tag, SupportsShouldProcess:=False)> _
Public Class Save_Tag : Inherits EditTagCmdLetBase
    Private Const VirtualNotAllowedError As String = "Virtual parameter is not allowed for saving"

#Region "process record"
    Protected Overrides Sub ProcessEditTag(ByVal TargetTag As Tag)
        If Me.Virtual.IsPresent Then Throw New ArgumentException(VirtualNotAllowedError)
    End Sub
#End Region

End Class
