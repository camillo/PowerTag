Public MustInherit Class TagCmdLetBase : Inherits CmdLetBase
    Public Const NoParameterParameterSetName As String = "NoParameterParametersetName"
    Public Const DefaultParameterSetName As String = "byFilenName"
    Private Const HelpMessageFileName As String = "path to mediafile (mp3,ogg...)"
    Public Const TagParaneterSetName As String = "byTag"
    Private Const HelpMessageTag As String = "target tag for operation"

    Protected Overrides Sub DoProcessRecord()
        Try
            For Each targetTag In GetTargetTags()
                Dim filename = targetTag.Path
                ProcessTag(targetTag)
            Next
        Catch ex As System.IO.FileNotFoundException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.ObjectNotFound, FullName))
        Catch ex As TagLibException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidResult, FullName))
        Catch ex As TagLib.UnsupportedFormatException
            Me.WriteVerbose(String.Format("unsupported Format: '{0}", FullName))
        Catch ex As TagLib.CorruptFileException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidData, FullName))
        End Try
    End Sub

    Protected Overridable Sub ProcessTag(ByVal TargetFile As Tag)
    End Sub

    Protected Overridable Function GetTargetTags() As Tag()
        Dim back As Tag()
        Select Case Me.ParameterSetName
            Case TagParaneterSetName
                back = New Tag() {Me.Tag}
            Case DefaultParameterSetName
                back = New Tag() {Tag.Create(Me.FullName, Me.SessionPath)}
            Case Else
                Throw New InternalException(String.Format("unknown Parametersetname {0}", Me.ParameterSetName))
        End Select
        Return back
    End Function

#Region "Parameter"

    Private myFullName As String
    <Parameter(Position:=0, Mandatory:=False, ParameterSetName:=DefaultParameterSetName, ValueFromPipeline:=True, ValueFromPipelinebyPropertyName:=True, HelpMessage:=HelpMessageFileName)> _
    Public Property FullName() As String
        Get
            Return myFullName
        End Get
        Set(ByVal value As String)
            myFullName = value
        End Set
    End Property

    Private myTag As Tag
    <Parameter(Position:=0, Mandatory:=False, ParameterSetName:=TagParaneterSetName, ValueFromPipeline:=True, HelpMessage:=HelpMessageTag)> _
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
