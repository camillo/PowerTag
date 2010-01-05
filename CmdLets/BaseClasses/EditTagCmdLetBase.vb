Public MustInherit Class EditTagCmdLetBase : Inherits TagCmdLetBase

    Protected Overrides Sub ProcessTag(ByVal TargetTag As Tag)
        Dim msg = String.Format("perfom {0} on {1}", Me.Name, TargetTag)
        If ShouldProcess(msg, msg, msg) Then
            Dim needSave = ProcessEditTag(TargetTag)

            If Me.Virtual.IsPresent Then
                Me.WriteVerbose("virtual mode; do not save tag to disk.")
                TargetTag.MarkDirty()
            Else
                If needSave Then
                    TargetTag.Save()
                Else
                    TargetTag.MarkDirty()
                End If
            End If
        End If

        If Me.PassThru.IsPresent Then Me.WriteObject(TargetTag)
    End Sub

    ''' <summary>is overriden by children to perform the edit task </summary>
    ''' <param name="TargetFile">file, that's tag will be edited</param>
    ''' <returns>true, if cmdlet should save TargetFile; false otherwise</returns>
    Protected Overridable Function ProcessEditTag(ByVal TargetFile As Tag) As Boolean
        Return True
    End Function

    Protected Enum WhatIfModes
        none = 0
        [true]
        [false]
    End Enum

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

End Class
