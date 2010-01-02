<Cmdlet(TagVerbs.Save, TagNounes.Tag, SupportsShouldProcess:=False)> _
Public Class Save_Tag : Inherits EditTagBase
    Protected Overrides Function DoEdit(ByVal TargetFile As TagLib.File) As Boolean
        Return True
    End Function
End Class
