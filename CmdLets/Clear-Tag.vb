<Cmdlet(VerbsCommon.Clear, TagNounes.Tag, _
        SupportsShouldProcess:=True, DefaultParameterSetName:=Clear_Tag.DefaultParameterSetName)> _
Public Class Clear_Tag : Inherits EditTagBase
    Protected Overrides Function DoEdit(ByVal TargetFile As TagLib.File) As Boolean
        TargetFile.Tag.Clear()
        Return True
    End Function
End Class
