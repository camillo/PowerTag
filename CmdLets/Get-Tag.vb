<Cmdlet(VerbsCommon.Get, TagNounes.Tag, _
        SupportsShouldProcess:=False)> _
Public Class Get_Tag : Inherits CmdLetBase
    Private Const HelpMessageFileName As String = "path to mediafile (mp3,ogg...)"
    Private myFileName As String

    Protected Overrides Sub DoProcessRecord()
        Dim TargetFiles = GetFileList()
        If HaveFilter Then HandleFilter(TargetFiles)
        ProcessFiles(TargetFiles)
    End Sub

    Private Function GetFileList() As List(Of String)
        Dim back As List(Of String)
        Dim parameterFileName = Me.FileName
        If String.IsNullOrEmpty(parameterFileName) Then
            Dim sessionPath = Me.SessionPath
            Me.WriteVerbose("No filename given. Use files in '{0}'", sessionPath)
            Dim FileList = New List(Of String)
            FillFileList(sessionPath, FileList, Me.Recursive.IsPresent)
            back = FileList
        Else
            Dim fullPath As String
            If System.IO.Path.IsPathRooted(parameterFileName) Then
                fullPath = parameterFileName
            Else
                fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Me.SessionPath, parameterFileName))
            End If
            If System.IO.Directory.Exists(fullPath) Then
                Dim FileList = New List(Of String)
                FillFileList(fullPath, FileList, Me.Recursive.IsPresent)
                back = FileList
            ElseIf System.IO.File.Exists(parameterFileName) Then
                back = New List(Of String)(New String() {fullPath})
            Else
                Throw New ArgumentException("Filename does not match a directory or file", "Filename")
            End If
        End If
        Return back
    End Function

    Private Sub ProcessFiles(ByVal TargetFiles As List(Of String))
        For Each currentFileName In TargetFiles
            Me.WriteVerbose("processing file '{0}'", currentFileName)
            Me.TargetObject = currentFileName
            Try
                Dim mediaTag = Tag.Create(currentFileName, Me.Force.IsPresent)
                If mediaTag Is Nothing Then
                    Me.WriteWarning(String.Format("File '{0}' does not have a tag", currentFileName))
                Else
                    Me.WriteObject(mediaTag)
                End If
            Catch ex As System.IO.FileNotFoundException
                Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.ObjectNotFound, currentFileName))
            Catch ex As TagLibException
                Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidResult, currentFileName))
            Catch ex As TagLib.UnsupportedFormatException
                Me.WriteVerbose(String.Format("unsupported Format: '{0}'", currentFileName))
            Catch ex As TagLib.CorruptFileException
                Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidData, currentFileName))
            End Try
        Next
    End Sub


    Private Sub HandleFilter(ByVal TargetFiles As List(Of String))
        Dim options = WildcardOptions.Compiled Or WildcardOptions.IgnoreCase
        For Each file In TargetFiles.ToArray
            Dim cankeep As Boolean = True
            Try
                Dim tag = PowerTag.Tag.Create(file)

                ' Handle Title filter
                If Not String.IsNullOrEmpty(Title) Then
                    Dim pattern = New WildcardPattern(Title, options)
                    Dim compareString = tag.Title
                    If compareString Is Nothing Then compareString = String.Empty
                    If Not pattern.IsMatch(compareString) Then
                        cankeep = False
                        WriteUltraVerbose("Title filter: remove file '{0}'", file)
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
                        WriteUltraVerbose("Artist filter: remove file '{0}'", file)
                    End If
                End If

                ' Handle Album filter
                If cankeep AndAlso Not String.IsNullOrEmpty(Album) Then
                    Dim pattern = New WildcardPattern(Album, options)
                    Dim compareString = tag.BaseTag.Album
                    If compareString Is Nothing Then compareString = String.Empty
                    If Not pattern.IsMatch(compareString) Then
                        cankeep = False
                        WriteUltraVerbose("Album filter: remove file '{0}'", file)
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

    Private ReadOnly Property HaveFilter() As Boolean
        Get
            Return Not (String.IsNullOrEmpty(Me.Title) _
                        AndAlso String.IsNullOrEmpty(Artist) _
                        AndAlso String.IsNullOrEmpty(Album))
        End Get
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

End Class
