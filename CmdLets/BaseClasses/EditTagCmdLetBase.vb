Public MustInherit Class EditTagCmdLetBase : Inherits TagCmdLetBase

    Protected Overrides Sub ProcessTag(ByVal TargetTag As Tag)
        Dim needSave = ProcessEditTag(TargetTag)

        If needSave _
          AndAlso (Not WhatIfMode = WhatIfModes.true) _
          AndAlso (WhatIfMode = WhatIfModes.false OrElse ShouldProcess(TargetTag.Path)) Then
            TargetTag.Save()
        End If

        If Me.PassThru.IsPresent Then
            targetTag = PowerTag.Tag.Create(FileName)
            Me.WriteObject(targetTag)
        End If
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

    Private Shared myWhatifMode As WhatIfModes = WhatIfModes.none
    Protected Shared Property WhatIfMode() As WhatIfModes
        Get
            Return myWhatifMode
        End Get
        Set(ByVal value As WhatIfModes)
            myWhatifMode = value
        End Set
    End Property

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

End Class
