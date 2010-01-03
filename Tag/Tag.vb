Partial Public Class Tag
    Private myInnerTag As TagLib.Tag
    Private myFile As TagLib.File

    Private Sub New(ByVal Filename As String)
        myFile = TagLib.File.Create(Filename)
        myInnerTag = myFile.Tag
    End Sub

    Public ReadOnly Property Path() As String
        Get
            Return myFile.Name
        End Get
    End Property

    Public ReadOnly Property BaseTag() As TagLib.Tag
        Get
            Return myInnerTag
        End Get
    End Property
End Class
