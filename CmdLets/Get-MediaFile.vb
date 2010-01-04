<Cmdlet(VerbsCommon.Get, TagNounes.MediaFile)> _
Public Class Get_MediaFile : Inherits CmdLetBase
    Protected Overrides Sub DoProcessRecord()
        Dim back = New List(Of System.IO.FileInfo)
        ScanDirecectory(Me.SessionPath, back, Me.Recursive.IsPresent)
        Me.WriteObject(back, True)
    End Sub

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

    Private myRecursive As SwitchParameter
    Public Property Recursive() As SwitchParameter
        Get
            Return myRecursive
        End Get
        Set(ByVal value As SwitchParameter)
            myRecursive = value
        End Set
    End Property

End Class
