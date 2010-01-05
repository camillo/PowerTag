Public MustInherit Class EditTagCmdLetBase : Inherits TagCmdLetBase
    Public Const WhatIfMessagePattern As String = "perfom {0} on {1}"

#Region "process record"
    Protected Overrides Sub ProcessTag(ByVal TargetTag As Tag)
        Dim msg = String.Format(WhatIfMessagePattern, Me.Name, TargetTag)

        If ShouldProcess(msg, msg, msg) Then
            ProcessEditTag(TargetTag)
            Save(TargetTag)
        End If

        If Me.PassThru.IsPresent Then Me.WriteObject(TargetTag)
    End Sub

    ''' <summary>is overriden by children to perform the edit task </summary>
    Protected Overridable Sub ProcessEditTag(ByVal TargetTag As Tag)
    End Sub
#End Region

#Region "Properties"
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

    Private myVirtual As SwitchParameter
    <Parameter()> _
    Public Property Virtual() As SwitchParameter
        Get
            Return myVirtual
        End Get
        Set(ByVal value As SwitchParameter)
            myVirtual = value
        End Set
    End Property
#End Region

#Region "private helper"
    Private Sub Save(ByVal TargetTag As Tag)
        If Me.Virtual.IsPresent Then
            TargetTag.MarkDirty()
        Else
            TargetTag.Save()
        End If
    End Sub
#End Region
End Class
