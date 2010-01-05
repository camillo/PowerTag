<Cmdlet(VerbsCommon.Get, TagNounes.MediaItem)> _
Public Class Get_MediaItem : Inherits CmdLetBase

#Region "process record"
    Protected Overrides Sub DoProcessRecord()
        Try
            Dim back = New List(Of System.IO.FileInfo)
            Dim fullName = Me.FullName
            If String.IsNullOrEmpty(fullName) Then
                ScanDirecectory(Me.SessionPath, back, Me.Recursive.IsPresent)
            ElseIf System.IO.File.Exists(fullName) Then
                If Tag.HasSupportedExtension(fullName) Then back.Add(New System.IO.FileInfo(fullName))
                If Me.Recursive.IsPresent Then WriteVerbose("FullName points to a file; recursive parameter is ignored")
            ElseIf System.IO.Directory.Exists(fullName) Then
                ScanDirecectory(fullName, back, Me.Recursive.IsPresent)
            Else
                Throw New IO.FileNotFoundException("Parameter FullName does not point to a file or directory", fullName)
            End If
            Me.WriteObject(back, True)
        Catch ex As IO.FileNotFoundException
            Me.WriteError(New ErrorRecord(ex, "MediaFile", ErrorCategory.ObjectNotFound, ex.FileName))
        End Try
    End Sub
#End Region

#Region "private helper"
    Private Sub ScanDirecectory(ByVal Path As String, ByVal Result As List(Of System.IO.FileInfo), ByVal Recursive As Boolean)
        Try
            For Each file In System.IO.Directory.GetFiles(Path)
                If Tag.HasSupportedExtension(file) Then Result.Add(New System.IO.FileInfo(file))
            Next
            If Recursive Then
                For Each directory In System.IO.Directory.GetDirectories(Path)
                    ScanDirecectory(directory, Result, True)
                Next
            End If
        Catch ex As Exception
            Me.WriteWarning("cannot scan '{0}'; {1}", Path, ex.Message)
        End Try
    End Sub
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

    Private myFullname As String
    <Parameter(ValueFromPipelineByPropertyName:=True, position:=0)> _
    Public Property FullName() As String
        Get
            Return myFullname
        End Get
        Set(ByVal value As String)
            myFullname = value
        End Set
    End Property
#End Region

End Class
