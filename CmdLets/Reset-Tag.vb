<Cmdlet(TagVerbs.Reset, TagNounes.Tag)> _
Public Class Reset_Tag : Inherits TagCmdLetBase

#Region "process record"
    Protected Overrides Sub ProcessTag(ByVal TargetTag As Tag)
        TargetTag.Reset()

        If Me.PassThru.IsPresent Then
            Dim back = PowerTag.Tag.ExchangeIfNeeded(TargetTag)
            Me.WriteObject(back)
        End If
    End Sub
#End Region

#Region "parameter"
    Private myPassThru As SwitchParameter
    <Parameter()> _
    Public Property PassThru() As SwitchParameter
        Get
            Return myPassThru
        End Get
        Set(ByVal value As SwitchParameter)
            myPassThru = value
        End Set
    End Property
#End Region

End Class
