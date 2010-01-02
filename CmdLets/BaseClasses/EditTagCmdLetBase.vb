﻿Public MustInherit Class EditTagCmdLetBase : Inherits TagCmdLetBase

    Protected Overrides Sub ProcessTag(ByVal targetFile As TagLib.File)
        Dim needSave = ProcessEditTag(targetFile)

        If needSave _
          AndAlso (Not WhatIfMode = WhatIfModes.true) _
          AndAlso (WhatIfMode = WhatIfModes.false OrElse ShouldProcess(targetFile.Name)) Then
            Me.WriteVerbose("saving '{0}'", targetFile.Name)
            targetFile.Save()
        End If

        If Me.PassThru.IsPresent Then
            targetFile = FileCache.GetFile(filename)
            Me.WriteObject(targetFile.Tag)
        End If
    End Sub


    ''' <summary>is overriden by children to perform the edit task </summary>
    ''' <param name="TargetFile">file, that's tag will be edited</param>
    ''' <returns>true, if cmdlet should save TargetFile; false otherwise</returns>
    Protected Overridable Function ProcessEditTag(ByVal TargetFile As TagLib.File) As Boolean
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