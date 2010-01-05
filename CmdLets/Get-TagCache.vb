<Cmdlet(VerbsCommon.Get, TagNounes.TagCache)> _
Public Class Get_TagCache : Inherits CmdLetBase

#Region "process record"
    Protected Overrides Sub DoProcessRecord()
        Dim showOnlyDirty = Me.Dirty.IsPresent
        For Each currentTag In Tag.GetCachedTags
            If currentTag.Dirty OrElse Not showOnlyDirty Then
                Me.WriteObject(currentTag)
            End If
        Next
    End Sub
#End Region

#Region "parameter"
    Private myDirty As SwitchParameter
    <Parameter()> _
    Public Property Dirty() As SwitchParameter
        Get
            Return myDirty
        End Get
        Set(ByVal value As SwitchParameter)
            myDirty = value
        End Set
    End Property
#End Region

End Class
