<Cmdlet(VerbsCommon.Get, TagNounes.Tag, _
        SupportsShouldProcess:=False)> _
Public Class Get_Tag : Inherits CmdLetBase
    Private Const HelpMessageFileName As String = "path to mediafile (mp3,ogg...)"
    Private myFileName As String

    Protected Overrides Sub DoProcessRecord()
        Dim TargetFiles As IEnumerable(Of String)
        Dim parameterFileName = Me.FileName
        If String.IsNullOrEmpty(parameterFileName) Then
            Dim sessionPath = Me.SessionState.Path.CurrentLocation.Path
            Me.WriteVerbose("No filename given. Use files in '{0}'", sessionPath)
            Dim FileList = New List(Of String)
            FillFileList(sessionPath, FileList, Me.Recursive.IsPresent)
            TargetFiles = FileList
        Else
            TargetFiles = New String() {parameterFileName}
        End If

        For Each currentFileName In TargetFiles
            Me.WriteVerbose("processing file '{0}'", currentFileName)
            Me.TargetObject = currentFileName
            Try
                Dim file = FileCache.GetFile(currentFileName)
                Dim tag = file.Tag
                If tag Is Nothing Then
                    Me.WriteWarning(String.Format("File '{0}' does not have a tag", currentFileName))
                Else
                    Me.WriteObject(tag)
                End If
            Catch ex As System.IO.FileNotFoundException
                Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.ObjectNotFound, currentFileName))
            Catch ex As TagLibException
                Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidResult, currentFileName))
            Catch ex As TagLib.UnsupportedFormatException
                Me.WriteWarning(String.Format("unsupported Format: '{0}'", currentFileName))
            Catch ex As TagLib.CorruptFileException
                Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidData, currentFileName))
            End Try
        Next

    End Sub

    Private Sub FillFileList(ByVal Directory As String, ByVal FileList As List(Of String), ByVal Recursive As Boolean)
        Try
            FileList.AddRange(System.IO.Directory.GetFiles(Directory))
            If Recursive Then
                For Each currentDirectory In System.IO.Directory.GetDirectories(Directory)
                    FillFileList(currentDirectory, FileList, Recursive)
                Next
            End If
        Catch ex As Exception
            Me.WriteWarning("error, scanning directory: '{0}' - {1}", Directory, ex.Message)
        End Try
    End Sub

    Private myRecursive As SwitchParameter
    <Parameter()> _
    Public Property Recursive() As SwitchParameter
        Get
            Return myRecursive
        End Get
        Set(ByVal value As SwitchParameter)
            myRecursive = value
        End Set
    End Property


    <Parameter(Position:=0, Mandatory:=False, ValueFromPipeline:=True, HelpMessage:=HelpMessageFileName)> _
    Public Property FileName() As String
        Get
            Return myFileName
        End Get
        Set(ByVal value As String)
            myFileName = value
        End Set
    End Property

End Class
