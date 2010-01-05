<Cmdlet(VerbsCommon.Get, TagNounes.GenreList)> _
Public Class Get_GenreList : Inherits CmdLetBase

#Region "process record"
    Protected Overrides Sub DoProcessRecord()
        Dim GenreList = GetGenreList()
        FilterGenereList(GenreList)
        Me.WriteObject(GenreList, True)
    End Sub
#End Region

#Region "private helper"
    Private Function GetGenreList() As List(Of String)
        Dim back = New List(Of String)
        If myVideo.IsPresent Then back.AddRange(TagLib.Genres.Video)
        If myAudio.IsPresent Then back.AddRange(TagLib.Genres.Audio)
        If back.Count = 0 Then
            back.AddRange(TagLib.Genres.Video)
            back.AddRange(TagLib.Genres.Audio)
        End If
        Return back
    End Function

    Private Sub FilterGenereList(ByVal GenreList As List(Of String))
        Dim filter = Me.Filter
        If Not String.IsNullOrEmpty(filter) Then
            Dim options = WildcardOptions.IgnoreCase Or WildcardOptions.IgnoreCase
            Dim wildcard = New WildcardPattern(filter, options)
            For Each item In GenreList.ToArray
                If Not wildcard.IsMatch(item) Then GenreList.Remove(item)
            Next
        End If
    End Sub
#End Region

#Region "parameter"
    Private myAudio As SwitchParameter
    <Parameter()> _
    Public Property Audio() As SwitchParameter
        Get
            Return myAudio
        End Get
        Set(ByVal value As SwitchParameter)
            myAudio = value
        End Set
    End Property

    Private myVideo As SwitchParameter
    <Parameter()> _
    Public Property Video() As SwitchParameter
        Get
            Return myVideo
        End Get
        Set(ByVal value As SwitchParameter)
            myVideo = value
        End Set
    End Property

    Private myFilter As String
    <Parameter(Position:=0)> _
    Public Property Filter() As String
        Get
            Return myFilter
        End Get
        Set(ByVal value As String)
            myFilter = value
        End Set
    End Property
#End Region

End Class
