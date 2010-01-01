<Cmdlet(VerbsCommon.Clear, TagNounes.Tag, _
        SupportsShouldProcess:=True, DefaultParameterSetName:=Clear_Tag.DefaultParameterSetName)> _
Public Class Clear_Tag : Inherits EditTagBase
    Protected Overrides Sub DoEdit(ByVal TargetFile As TagLib.File)
        If Me.ShouldProcess(TargetFile.Name) Then
            TargetFile.Tag.Clear()
        End If
    End Sub
End Class
