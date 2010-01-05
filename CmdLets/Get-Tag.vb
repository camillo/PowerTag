<Cmdlet(VerbsCommon.Get, TagNounes.Tag, _
        SupportsShouldProcess:=False)> _
Public Class Get_Tag : Inherits CmdLetBase
    Private Const HelpMessageFileName As String = "path to mediafile (mp3,ogg...)"

    Private Const NoTagMessage As String = "File '{0}' does not have a tag"

#Region "process record"
    Protected Overrides Sub DoProcessRecord()
        Dim TargetFiles = GetFileList()
        If HaveFilter Then HandleFilter(TargetFiles)
        ProcessFiles(TargetFiles)
    End Sub

    Private Sub ProcessFiles(ByVal TargetFiles As List(Of String))
        For Each currentFileName In TargetFiles
            Try
                Dim mediaTag = Tag.Create(currentFileName, Me.SessionPath, Me.Force.IsPresent)
                If mediaTag Is Nothing Then
                    Me.WriteWarning(NoTagMessage, currentFileName)
                Else
                    Me.WriteObject(mediaTag)
                End If
            Catch ex As System.IO.FileNotFoundException
                Me.WriteError(New ErrorRecord(ex, Me.DefaultErrorId, ErrorCategory.ObjectNotFound, currentFileName))
            Catch ex As TagLibException
                Me.WriteError(New ErrorRecord(ex, Me.DefaultErrorId, ErrorCategory.InvalidResult, currentFileName))
            Catch ex As TagLib.UnsupportedFormatException
                Me.WriteVerbose(UnsupportedFormatMessage, currentFileName)
            Catch ex As TagLib.CorruptFileException
                Me.WriteError(New ErrorRecord(ex, Me.DefaultErrorId, ErrorCategory.InvalidData, currentFileName))
            End Try
        Next
    End Sub
#End Region

#Region "private helper"
    Private Function GetFileList() As List(Of String)
        Dim back As List(Of String)
        Dim parameterFileName = Me.FullName
        If String.IsNullOrEmpty(parameterFileName) Then
            Dim sessionPath = Me.SessionPath
            Me.WriteVerbose("No filename given. Use files in '{0}'", sessionPath)
            Dim FileList = New List(Of String)
            FillFileList(sessionPath, FileList, Me.Recursive.IsPresent)
            back = FileList
        Else
            Dim fullPath As String = Util.GetFullPath(parameterFileName, Me.SessionPath)
            If System.IO.Directory.Exists(fullPath) Then
                Dim FileList = New List(Of String)
                FillFileList(fullPath, FileList, Me.Recursive.IsPresent)
                back = FileList
            ElseIf System.IO.File.Exists(fullPath) Then
                back = New List(Of String)(New String() {fullPath})
            Else
                Throw New ArgumentException("Filename does not match a directory or file", "Filename")
            End If
        End If
        Return back
    End Function

    Private Sub HandleFilter(ByVal TargetFiles As List(Of String))
        Dim options = WildcardOptions.Compiled Or WildcardOptions.IgnoreCase
        For Each file In TargetFiles.ToArray
            Dim cankeep As Boolean = True
            Try
                Dim tag = PowerTag.Tag.Create(file, Me.SessionPath)

                ' Handle Title filter
                If Not String.IsNullOrEmpty(Title) Then
                    Dim pattern = New WildcardPattern(Title, options)
                    Dim compareString = tag.Title
                    If compareString Is Nothing Then compareString = String.Empty
                    If Not pattern.IsMatch(compareString) Then
                        cankeep = False
                        WriteVerbose("Title filter: remove file '{0}'", file)
                    End If
                End If

                ' Handle Artist filter
                If cankeep AndAlso Not String.IsNullOrEmpty(Artist) Then
                    Dim pattern = New WildcardPattern(Artist, options)

                    Dim comparerArray As String() = tag.BaseTag.Artists
                    If comparerArray Is Nothing Then comparerArray = New String() {}
                    Dim keep As Boolean = False
                    For Each crrentArtist In comparerArray
                        If pattern.IsMatch(crrentArtist) Then
                            keep = True
                            Exit For
                        End If
                    Next
                    If Not keep Then
                        cankeep = False
                        WriteVerbose("Artist filter: remove file '{0}'", file)
                    End If
                End If

                ' Handle Album filter
                If cankeep AndAlso Not String.IsNullOrEmpty(Album) Then
                    Dim pattern = New WildcardPattern(Album, options)
                    Dim compareString = tag.BaseTag.Album
                    If compareString Is Nothing Then compareString = String.Empty
                    If Not pattern.IsMatch(compareString) Then
                        cankeep = False
                        WriteVerbose("Album filter: remove file '{0}'", file)
                    End If
                End If

                If Not cankeep Then TargetFiles.Remove(file)

            Catch ex As TagLib.UnsupportedFormatException
                Me.WriteVerbose(String.Format("unsupported Format: '{0}'", System.IO.Path.GetFileName(file)))
                TargetFiles.Remove(file)
            Catch ex As TagLib.CorruptFileException
                Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidData, file))
                TargetFiles.Remove(file)
            Catch ex As Exception
                Throw
            End Try
            If Not cankeep Then TargetFiles.Remove(file)
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
#End Region

#Region "private properties"
    Private ReadOnly Property HaveFilter() As Boolean
        Get
            Return Not (String.IsNullOrEmpty(Me.Title) _
                        AndAlso String.IsNullOrEmpty(Artist) _
                        AndAlso String.IsNullOrEmpty(Album))
        End Get
    End Property
#End Region

#Region "parameter"
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

    Private myFullName As String
    <Parameter(Position:=0, Mandatory:=False, ValueFromPipeline:=True, ValueFromPipelineByPropertyName:=True, HelpMessage:=HelpMessageFileName)> _
    Public Property FullName() As String
        Get
            Return myFullName
        End Get
        Set(ByVal value As String)
            myFullName = value
        End Set
    End Property

    Private myTitle As String
    <Parameter()> _
    Public Property Title() As String
        Get
            Return myTitle
        End Get
        Set(ByVal value As String)
            myTitle = value
        End Set
    End Property

    Private myArtist As String
    <Parameter()> _
    Public Property Artist() As String
        Get
            Return myArtist
        End Get
        Set(ByVal value As String)
            myArtist = value
        End Set
    End Property

    Private myAlbum As String
    <Parameter()> _
    Public Property Album() As String
        Get
            Return myAlbum
        End Get
        Set(ByVal value As String)
            myAlbum = value
        End Set
    End Property

    Private myForce As SwitchParameter
    <Parameter()> _
    Public Property Force() As SwitchParameter
        Get
            Return myForce
        End Get
        Set(ByVal value As SwitchParameter)
            myForce = value
        End Set
    End Property
#End Region

End Class
