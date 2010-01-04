Public Class AudioTag : Inherits Tag
    Protected Friend Sub New(ByVal File As TagLib.File)
        MyBase.New(File)
    End Sub

#Region "Wrapped Taglib Properties"
    Public Property Album() As String
        Get
            Return Me.BaseTag.Album
        End Get
        Set(ByVal value As String)
            Me.BaseTag.Album = value
        End Set
    End Property

    Public Property Artists() As String()
        Get
            Return BaseTag.Artists
        End Get
        Set(ByVal value As String())
            BaseTag.Artists = value
        End Set
    End Property

    Public Property Track() As UInt32
        Get
            Return BaseTag.Track
        End Get
        Set(ByVal value As UInt32)
            BaseTag.Track = value
        End Set
    End Property
#End Region

End Class
