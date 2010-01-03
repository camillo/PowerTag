Public MustInherit Class TagCmdLetBase : Inherits CmdLetBase
    Public Const DefaultParameterSetName As String = "byFilenName"
    Private Const HelpMessageFileName As String = "path to mediafile (mp3,ogg...)"
    Public Const TagParaneterSetName As String = "byTag"
    Private Const HelpMessageTag As String = "target tag for operation"

    Protected Overrides Sub DoProcessRecord()
        Try
            Dim targetFile = GetTargetFile()
            Dim filename = targetFile.Name
            ProcessTag(targetFile)
        Catch ex As System.IO.FileNotFoundException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.ObjectNotFound, FileName))
        Catch ex As FileCache.MediaFileNotFoundException
            Throw New InternalException("requestet file for given tag not found", ex)
        Catch ex As TagLibException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidResult, FileName))
        Catch ex As TagLib.UnsupportedFormatException
            Me.WriteWarning(String.Format("unsupported Format: '{0}", FileName))
        Catch ex As TagLib.CorruptFileException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidData, FileName))
        End Try
    End Sub

    Protected Overridable Sub ProcessTag(ByVal TargetFile As TagLib.File)
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


#End Region

End Class
