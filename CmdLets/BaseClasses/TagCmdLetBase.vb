Public MustInherit Class TagCmdLetBase : Inherits CmdLetBase
    Public Const DefaultParameterSetName As String = "byFilenName"
    Private Const HelpMessageFileName As String = "path to mediafile (mp3,ogg...)"
    Public Const TagParaneterSetName As String = "byTag"
    Private Const HelpMessageTag As String = "target tag for operation"

    Protected Overrides Sub DoProcessRecord()
        Try
            Dim targetTag = GetTargetTag()
            Dim filename = targetTag.Path
            ProcessTag(targetTag)
        Catch ex As System.IO.FileNotFoundException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.ObjectNotFound, FileName))
        Catch ex As TagLibException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidResult, FileName))
        Catch ex As TagLib.UnsupportedFormatException
            Me.WriteVerbose(String.Format("unsupported Format: '{0}", FileName))
        Catch ex As TagLib.CorruptFileException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidData, FileName))
        End Try
    End Sub

    Protected Overridable Sub ProcessTag(ByVal TargetFile As Tag)
    End Sub

    Private Function GetTargetTag() As Tag
        Dim back As Tag
        Select Case Me.ParameterSetName
            Case TagParaneterSetName
                back = Me.Tag
            Case DefaultParameterSetName
                back = Tag.Create(Me.FileName)
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

    Private myTag As Tag
    <Parameter(Mandatory:=True, ParameterSetName:=TagParaneterSetName, ValueFromPipeline:=True, HelpMessage:=HelpMessageTag)> _
    Public Property Tag() As Tag
        Get
            Return myTag
        End Get
        Set(ByVal value As Tag)
            myTag = value
        End Set
    End Property


#End Region

End Class
