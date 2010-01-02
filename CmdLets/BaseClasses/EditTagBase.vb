Public MustInherit Class EditTagBase : Inherits CmdLetBase
    Public Const DefaultParameterSetName As String = "byFilenName"
    Public Const TagParaneterSetName As String = "byTag"
    Private Const HelpMessageFileName As String = "path to mediafile (mp3,ogg...)"
    Private Const HelpMessageTag As String = "target tag for operation"

    Protected Overrides Sub DoProcessRecord()

        Try
            Dim targetFile = GetTargetFile()
            Dim filename = targetFile.Name

            Dim needSave = DoEdit(targetFile)

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

        Catch ex As System.IO.FileNotFoundException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.ObjectNotFound, FileName))
        Catch ex As FileCache.FileNotFoundException
            Throw New InternalException("requestet file for given tag not found", ex)
        Catch ex As TagLibException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidResult, FileName))
        Catch ex As TagLib.UnsupportedFormatException
            Me.WriteWarning(String.Format("unsupported Format: '{0}", FileName))
        Catch ex As TagLib.CorruptFileException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidData, FileName))
        End Try

    End Sub

    Private Function GetTargetFile() As TagLib.File
        Dim back As TagLib.File
        Select Case Me.ParameterSetName
            Case TagParaneterSetName
                back = FileCache.GetFile(Me.Tag)
            Case DefaultParameterSetName
                back = FileCache.GetFile(Me.FileName)
            Case Else
                Throw New InternalException(String.Format("unknown Parametersetname {0}", Me.ParameterSetName))
        End Select
        Return back
    End Function

    ''' <summary>is overriden by children to perform the edit task </summary>
    ''' <param name="TargetFile">file, that's tag will be edited</param>
    ''' <returns>true, if cmdlet should save TargetFile; false otherwise</returns>
    Protected Overridable Function DoEdit(ByVal TargetFile As TagLib.File) As Boolean
        Return True
    End Function

#Region "Parameter"

    Private myFileName As String
    <Parameter(Mandatory:=True, ParameterSetName:=DefaultParameterSetName, ValueFromPipeline:=True, HelpMessage:=HelpMessageFileName)> _
    Public Property FileName() As String
        Get
            Return myFileName
        End Get
        Set(ByVal value As String)
            myFileName = value
        End Set
    End Property

    Private myTag As TagLib.Tag
    <Parameter(Mandatory:=True, ParameterSetName:=TagParaneterSetName, ValueFromPipeline:=True, HelpMessage:=HelpMessageTag)> _
    Public Property Tag() As TagLib.Tag
        Get
            Return myTag
        End Get
        Set(ByVal value As TagLib.Tag)
            myTag = value
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

#End Region


End Class
