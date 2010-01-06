Public MustInherit Class TagCmdLetBase : Inherits CmdLetBase
    Public Const NoParameterParameterSetName As String = "NoParameterParametersetName"
    Public Const DefaultParameterSetName As String = "byFilenName"

    Private Const HelpMessageFileName As String = "path to mediafile (mp3,ogg...)"

    Protected Const UnknownParameterSetNameMessage As String = "unknown Parametersetname {0}"

#Region "process record"
    Protected Overrides Sub DoProcessRecord()
        Try
            For Each targetTag In GetTargetTags()
                ProcessTag(targetTag)
            Next
        Catch ex As System.IO.FileNotFoundException
            Me.WriteError(New ErrorRecord(ex, DefaultErrorId, ErrorCategory.ObjectNotFound, ex.FileName))
        Catch ex As TagLibException
            Me.WriteError(New ErrorRecord(ex, DefaultErrorId, ErrorCategory.InvalidResult, Me))
        Catch ex As TagLib.UnsupportedFormatException
            Dim filename = System.IO.Path.GetFileNameWithoutExtension(Me.FullName)
            Me.WriteVerbose(UnsupportedFormatMessage, Me.FullName)
        Catch ex As TagLib.CorruptFileException
            Me.WriteError(New ErrorRecord(ex, DefaultErrorId, ErrorCategory.InvalidData, Me.FullName))
        End Try

    End Sub

    Protected Overridable Sub ProcessTag(ByVal TargetFile As Tag)
    End Sub
#End Region

#Region "private helper"
    Private Function GetTargetTags() As IEnumerable(Of Tag)
        Dim back = New List(Of Tag)
        Dim fullName As String = Util.GetFullPath(Me.FullName, Me.SessionPath)
        If System.IO.Directory.Exists(fullName) Then
            Dim command = String.Format("Get-MediaItem -FullName {0}", Util.ConvertToParameterText(fullName))
            Dim result = Me.ExecuteNewPipeline(command, Nothing)
            For Each file In Util.ToArray(Of System.IO.FileInfo)(result)
                back.Add(Tag.Create(file.FullName, Me.SessionPath))
            Next
        Else
            back.Add(Tag.Create(Me.FullName, Me.SessionPath))
        End If

        Return back.ToArray
    End Function
#End Region

#Region "Parameter"

    Private myFullName As String
    <Parameter(Position:=0, Mandatory:=False, ParameterSetName:=DefaultParameterSetName, _
               ValueFromPipeline:=True, ValueFromPipelinebyPropertyName:=True, HelpMessage:=HelpMessageFileName)> _
    Public Property FullName() As String
        Get
            Return myFullName
        End Get
        Set(ByVal value As String)
            myFullName = value
        End Set
    End Property

#End Region

End Class
