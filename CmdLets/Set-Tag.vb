Imports System.Text.RegularExpressions
<Cmdlet(VerbsCommon.Set, TagNounes.Tag, _
        SupportsShouldProcess:=True, DefaultParameterSetName:=Set_Tag.DefaultParameterSetName)> _
Public Class Set_Tag : Inherits EditTagCmdLetBase
    Private Const NoProperryFoundError As String = "no property found for parameter '{0}'"
    Private Const Uint32ParseError As String = "parameter '{0}': cannot parse value '{1}' into UInt32"
    Private Const UnknownTypeError As String = "parameter '{0}': unknown type '{1}'"
    Private Const MultipleDefinitionError As String = "parameter '{0}' has multiple definitions (regex and directy)"
    Private Const MatchingPropertyNotFoundError As String = "parameter: '{0}': matching property '{1}' not found in '{2}'"
    Private Const SetValueError As String = "error setting taglib property '{0}' to value '{1}'"
    Private Const InvalidRegexGroupError As String = "Invalid regex group name '{0}'; no matching parameter found"
    Private Const MultipleRegexError As String = "Parameter '{0}' is defiened by more than one regex"

    Private Const NoMatchWarning As String = "regex does not match"
#Region "process record"
    Protected Overrides Sub ProcessEditTag(ByVal TargetTag As Tag)
        Try
            SetRegexParameter(TargetTag)
            SetTaglibProperties(TargetTag)
        Catch ex As ArgumentException
            Me.WriteError(New ErrorRecord(ex, Me.DefaultErrorId, ErrorCategory.InvalidData, TargetTag))
        End Try
    End Sub
#End Region

#Region "private helper"
    Private Sub SetRegexParameter(ByVal TargetTag As Tag)
        Dim myType = Me.GetType
        For Each regexParam In GetRegexParameter(TargetTag)
            Dim name = regexParam.Key
            Dim stringValue = regexParam.Value
            Dim prop = myType.GetProperty(name)
            If prop Is Nothing Then Throw New InternalException(NoProperryFoundError, name)
            Dim targetValue As Object
            Dim targetType = prop.PropertyType
            If targetType.Equals(GetType(String)) Then
                targetValue = stringValue
            ElseIf targetType.Equals(GetType(String())) Then
                targetValue = New String() {stringValue}
            ElseIf targetType.Equals(GetType(Nullable(Of UInt32))) Then
                Dim uint32Value As UInt32
                If Not UInt32.TryParse(stringValue, uint32Value) Then _
                    Throw New ArgumentException(String.Format(Uint32ParseError, name, stringValue), name)
                targetValue = uint32Value
            Else
                Throw New InternalException(UnknownTypeError, name, targetType.FullName)
            End If
            Dim oldValue = prop.GetValue(Me, Nothing)
            If Not oldValue Is Nothing Then Throw New ArgumentException(String.Format(MultipleDefinitionError, name), name)
            prop.SetValue(Me, targetValue, Nothing)
        Next
    End Sub

    Private Sub SetTaglibProperties(ByVal TargetTag As Tag)

        Dim tagType = TargetTag.BaseTag.GetType

        For Each kvp In Me.GetTaglibParameterProperties
            Dim myProp = kvp.Key
            Dim myPropName = myProp.Name
            Dim value = myProp.GetValue(Me, Nothing)
            If Not value Is Nothing Then
                Dim taglibPropertyName As String = kvp.Value.TaglibName
                Dim taglibProp = tagType.GetProperty(taglibPropertyName)
                If taglibProp Is Nothing Then Throw New InternalException(MatchingPropertyNotFoundError, myProp.Name, taglibPropertyName, tagType.FullName)
                Try
                    taglibProp.SetValue(TargetTag.BaseTag, value, Nothing)
                Catch ex As Exception
                    Throw New InternalException(String.Format(SetValueError, taglibPropertyName, value), ex)
                End Try
            End If
        Next
    End Sub

    Private Function GetRegexParameter(ByVal TargetTag As Tag) As Dictionary(Of String, String)
        Dim back = New Dictionary(Of String, String)
        Dim filename = System.IO.Path.GetFullPath(TargetTag.Path)
        Dim properties = GetTaglibParameterProperties()

        For Each currentPair In New KeyValuePair(Of Regex, String)() { _
        New KeyValuePair(Of Regex, String)(myFilenameRegex, System.IO.Path.GetFileNameWithoutExtension(filename)), _
        New KeyValuePair(Of Regex, String)(myParentRegex, System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetDirectoryName(filename))), _
        New KeyValuePair(Of Regex, String)(myRootRegex, Me.SessionState.Path.CurrentLocation.Path)}
            Dim currentRegex = currentPair.Key
            If currentRegex Is Nothing Then Continue For
            Dim match = currentRegex.Match(currentPair.Value)
            If Not match.Success Then
                Me.WriteWarning(NoMatchWarning)
                Continue For
            End If
            For Each groupName In currentRegex.GetGroupNames
                If IsNumeric(groupName) Then Continue For
                Dim tmp As KeyValuePair(Of Reflection.PropertyInfo, TaglibParameterAttribute) = Nothing

                If Not TryGetTaglibParemeter(groupName, tmp) Then Throw New  _
                ArgumentException(String.Format(InvalidRegexGroupError, groupName), groupName)

                Dim group = match.Groups(groupName)
                If group.Success Then
                    Dim parameterName = tmp.Key.Name
                    If back.ContainsKey(parameterName) Then Throw New ArgumentException(String.Format(MultipleRegexError, parameterName), groupName)
                    Dim value = group.Value
                    back.Add(parameterName, value)
                    Me.WriteVerbose("regex value '{0}': '{1}'", groupName, value)
                Else
                    Me.WriteVerbose("group '{0}' is denined, but does not match", groupName)
                End If
            Next
        Next

        Return back
    End Function

    Private Function GetTaglibParameterProperties() As Dictionary(Of Reflection.PropertyInfo, TaglibParameterAttribute)
        Dim back = New Dictionary(Of Reflection.PropertyInfo, TaglibParameterAttribute)
        Dim myType = Me.GetType
        For Each prop In myType.GetProperties
            If prop.DeclaringType.Equals(myType) Then
                Dim attributes = prop.GetCustomAttributes(GetType(TaglibParameterAttribute), False)
                If attributes.Length = 1 Then
                    back.Add(prop, DirectCast(attributes(0), TaglibParameterAttribute))
                Else
                    Me.WriteVerbose("skipping property '{0}' (no parameter)", prop.Name)
                End If
            Else
                Me.WriteVerbose("skipping property '{0}' (not mine)", prop.Name)
            End If
        Next
        Return back
    End Function
#End Region

#Region "shared helper"
    Friend Shared Function TryGetTaglibParemeter(ByVal Name As String, ByRef Result As KeyValuePair(Of Reflection.PropertyInfo, TaglibParameterAttribute)) As Boolean
        Dim back As Boolean = False
        Dim foundMore As Boolean = False
        Dim nameLower = Name.ToLower.Trim
        For Each prop In GetType(Set_Tag).GetProperties
            Dim attr = prop.GetCustomAttributes(GetType(TaglibParameterAttribute), False)
            If attr.Length = 0 Then Continue For
            Dim propName = prop.Name.ToLower
            If propName.Equals(nameLower) Then
                back = True
                foundMore = False
                Result = New KeyValuePair(Of Reflection.PropertyInfo, TaglibParameterAttribute)(prop, DirectCast(attr(0), TaglibParameterAttribute))
                Exit For
            ElseIf propName.StartsWith(nameLower) Then
                If Not back Then
                    Result = New KeyValuePair(Of Reflection.PropertyInfo, TaglibParameterAttribute)(prop, DirectCast(attr(0), TaglibParameterAttribute))
                    back = True
                Else
                    foundMore = True
                End If
            End If
        Next
        If foundMore Then Throw New ArgumentException(String.Format("more than one parameter could match name '{0}'", Name), Name)
        Return back
    End Function
#End Region

#Region "Taglib Parameter"
    Private myTitle As String
    <Parameter(Mandatory:=False), TaglibParameter("Title")> _
    Public Property Title() As String
        Get
            Return myTitle
        End Get
        Set(ByVal value As String)
            myTitle = value
        End Set
    End Property

    Private myTitleSort As String
    <Parameter(Mandatory:=False), TaglibParameter("TitleSort")> _
    Public Property TitleSort() As String
        Get
            Return myTitleSort
        End Get
        Set(ByVal value As String)
            myTitleSort = value
        End Set
    End Property

    Private myPerformers As String()
    <Parameter(Mandatory:=False), TaglibParameter("Performers")> _
    Public Property Performers() As String()
        Get
            Return myPerformers
        End Get
        Set(ByVal value As String())
            myPerformers = value
        End Set
    End Property

    Private myPerformersSort As String()
    <Parameter(Mandatory:=False), TaglibParameter("PerformersSort")> _
    Public Property PerformersSort() As String()
        Get
            Return myPerformersSort
        End Get
        Set(ByVal value As String())
            myPerformersSort = value
        End Set
    End Property

    Private myAlbumArtists As String()
    <Parameter(Mandatory:=False), TaglibParameter("AlbumArtists")> _
    Public Property AlbumArtists() As String()
        Get
            Return myAlbumArtists
        End Get
        Set(ByVal value As String())
            myAlbumArtists = value
        End Set
    End Property

    Private myAlbumArtistsSort As String()
    <Parameter(Mandatory:=False), TaglibParameter("AlbumArtistsSort")> _
    Public Property AlbumArtistsSort() As String()
        Get
            Return myAlbumArtistsSort
        End Get
        Set(ByVal value As String())
            myAlbumArtistsSort = value
        End Set
    End Property

    Private myComposers As String()
    <Parameter(Mandatory:=False), TaglibParameter("Composers")> _
    Public Property Composers() As String()
        Get
            Return myComposers
        End Get
        Set(ByVal value As String())
            myComposers = value
        End Set
    End Property

    Private myComposersSort As String()
    <Parameter(Mandatory:=False), TaglibParameter("ComposersSort")> _
    Public Property ComposersSort() As String()
        Get
            Return myComposersSort
        End Get
        Set(ByVal value As String())
            myComposersSort = value
        End Set
    End Property

    Private myAlbum As String
    <Parameter(Mandatory:=False), TaglibParameter("Album")> _
    Public Property Album() As String
        Get
            Return myAlbum
        End Get
        Set(ByVal value As String)
            myAlbum = value
        End Set
    End Property

    Private myAlbumSort As String
    <Parameter(Mandatory:=False), TaglibParameter("AlbumSort")> _
    Public Property AlbumSort() As String
        Get
            Return myAlbumSort
        End Get
        Set(ByVal value As String)
            myAlbumSort = value
        End Set
    End Property

    Private myComment As String
    <Parameter(Mandatory:=False), TaglibParameter("Comment")> _
    Public Property Comment() As String
        Get
            Return myComment
        End Get
        Set(ByVal value As String)
            myComment = value
        End Set
    End Property

    Private myGenres As String()
    <Parameter(Mandatory:=False), TaglibParameter("Genres")> _
    Public Property Genres() As String()
        Get
            Return myGenres
        End Get
        Set(ByVal value As String())
            myGenres = value
        End Set
    End Property

    Private myYear As Nullable(Of UInt32)
    <Parameter(Mandatory:=False), TaglibParameter("Year")> _
    Public Property Year() As Nullable(Of UInt32)
        Get
            Return myYear
        End Get
        Set(ByVal value As Nullable(Of UInt32))
            myYear = value
        End Set
    End Property

    Private myTrack As Nullable(Of UInt32)
    <Parameter(Mandatory:=False), TaglibParameter("Track")> _
    Public Property Track() As Nullable(Of UInt32)
        Get
            Return myTrack
        End Get
        Set(ByVal value As Nullable(Of UInt32))
            myTrack = value
        End Set
    End Property

    Private myTrackCount As Nullable(Of UInt32)
    <Parameter(Mandatory:=False), TaglibParameter("TrackCount")> _
    Public Property TrackCount() As Nullable(Of UInt32)
        Get
            Return myTrackCount
        End Get
        Set(ByVal value As Nullable(Of UInt32))
            myTrackCount = value
        End Set
    End Property

    Private myDisc As Nullable(Of UInt32)
    <Parameter(Mandatory:=False), TaglibParameter("Disc")> _
    Public Property Disc() As Nullable(Of UInt32)
        Get
            Return myDisc
        End Get
        Set(ByVal value As Nullable(Of UInt32))
            myDisc = value
        End Set
    End Property

    Private myDiscCount As Nullable(Of UInt32)
    <Parameter(Mandatory:=False), TaglibParameter("DiscCount")> _
    Public Property DiscCount() As Nullable(Of UInt32)
        Get
            Return myDiscCount
        End Get
        Set(ByVal value As Nullable(Of UInt32))
            myDiscCount = value
        End Set
    End Property

    Private myLyrics As String
    <Parameter(Mandatory:=False), TaglibParameter("Lyrics")> _
    Public Property Lyrics() As String
        Get
            Return myLyrics
        End Get
        Set(ByVal value As String)
            myLyrics = value
        End Set
    End Property

    Private myGrouping As String
    <Parameter(Mandatory:=False), TaglibParameter("Grouping")> _
    Public Property Grouping() As String
        Get
            Return myGrouping
        End Get
        Set(ByVal value As String)
            myGrouping = value
        End Set
    End Property

    Private myBeatsPerMinute As Nullable(Of UInt32)
    <Parameter(Mandatory:=False), TaglibParameter("BeatsPerMinute")> _
    Public Property BeatsPerMinute() As Nullable(Of UInt32)
        Get
            Return myBeatsPerMinute
        End Get
        Set(ByVal value As Nullable(Of UInt32))
            myBeatsPerMinute = value
        End Set
    End Property

    Private myConductor As String
    <Parameter(Mandatory:=False), TaglibParameter("Conductor")> _
    Public Property Conductor() As String
        Get
            Return myConductor
        End Get
        Set(ByVal value As String)
            myConductor = value
        End Set
    End Property

    Private myCopyright As String
    <Parameter(Mandatory:=False), TaglibParameter("Copyright")> _
    Public Property Copyright() As String
        Get
            Return myCopyright
        End Get
        Set(ByVal value As String)
            myCopyright = value
        End Set
    End Property

    Private myMusicBrainzArtistId As String
    <Parameter(Mandatory:=False), TaglibParameter("MusicBrainzArtistId")> _
    Public Property MusicBrainzArtistId() As String
        Get
            Return myMusicBrainzArtistId
        End Get
        Set(ByVal value As String)
            myMusicBrainzArtistId = value
        End Set
    End Property

    Private myMusicBrainzReleaseId As String
    <Parameter(Mandatory:=False), TaglibParameter("MusicBrainzReleaseId")> _
    Public Property MusicBrainzReleaseId() As String
        Get
            Return myMusicBrainzReleaseId
        End Get
        Set(ByVal value As String)
            myMusicBrainzReleaseId = value
        End Set
    End Property

    Private myMusicBrainzReleaseArtistId As String
    <Parameter(Mandatory:=False), TaglibParameter("MusicBrainzReleaseArtistId")> _
    Public Property MusicBrainzReleaseArtistId() As String
        Get
            Return myMusicBrainzReleaseArtistId
        End Get
        Set(ByVal value As String)
            myMusicBrainzReleaseArtistId = value
        End Set
    End Property

    Private myMusicBrainzTrackId As String
    <Parameter(Mandatory:=False), TaglibParameter("MusicBrainzTrackId")> _
    Public Property MusicBrainzTrackId() As String
        Get
            Return myMusicBrainzTrackId
        End Get
        Set(ByVal value As String)
            myMusicBrainzTrackId = value
        End Set
    End Property

    Private myMusicBrainzDiscId As String
    <Parameter(Mandatory:=False), TaglibParameter("MusicBrainzDiscId")> _
    Public Property MusicBrainzDiscId() As String
        Get
            Return myMusicBrainzDiscId
        End Get
        Set(ByVal value As String)
            myMusicBrainzDiscId = value
        End Set
    End Property

    Private myMusicIpId As String
    <Parameter(Mandatory:=False), TaglibParameter("MusicIpId")> _
    Public Property MusicIpId() As String
        Get
            Return myMusicIpId
        End Get
        Set(ByVal value As String)
            myMusicIpId = value
        End Set
    End Property

    Private myAmazonId As String
    <Parameter(Mandatory:=False), TaglibParameter("AmazonId")> _
    Public Property AmazonId() As String
        Get
            Return myAmazonId
        End Get
        Set(ByVal value As String)
            myAmazonId = value
        End Set
    End Property

    Private myMusicBrainzReleaseStatus As String
    <Parameter(Mandatory:=False), TaglibParameter("MusicBrainzReleaseStatus")> _
    Public Property MusicBrainzReleaseStatus() As String
        Get
            Return myMusicBrainzReleaseStatus
        End Get
        Set(ByVal value As String)
            myMusicBrainzReleaseStatus = value
        End Set
    End Property

    Private myMusicBrainzReleaseType As String
    <Parameter(Mandatory:=False), TaglibParameter("MusicBrainzReleaseType")> _
    Public Property MusicBrainzReleaseType() As String
        Get
            Return myMusicBrainzReleaseType
        End Get
        Set(ByVal value As String)
            myMusicBrainzReleaseType = value
        End Set
    End Property

    Private myMusicBrainzReleaseCountry As String
    <Parameter(Mandatory:=False), TaglibParameter("MusicBrainzReleaseCountry")> _
    Public Property MusicBrainzReleaseCountry() As String
        Get
            Return myMusicBrainzReleaseCountry
        End Get
        Set(ByVal value As String)
            myMusicBrainzReleaseCountry = value
        End Set
    End Property

    Private myArtists As String()
    <Parameter(Mandatory:=False), TaglibParameter("Artists")> _
    Public Property Artists() As String()
        Get
            Return myArtists
        End Get
        Set(ByVal value As String())
            myArtists = value
        End Set
    End Property
#End Region

#Region "Regex Parameter"

    Private myFilenamePattern As String
    Private myFilenameRegex As System.Text.RegularExpressions.Regex
    <Parameter()> _
    Public Property FilenamePattern() As String
        Get
            Return myFilenamePattern
        End Get
        Set(ByVal value As String)
            myFilenamePattern = value
            myFilenameRegex = New System.Text.RegularExpressions.Regex(value)
        End Set
    End Property

    Private myParentPattern As String
    Private myParentRegex As System.Text.RegularExpressions.Regex
    <Parameter()> _
    Public Property ParentPattern() As String
        Get
            Return myParentPattern
        End Get
        Set(ByVal value As String)
            myParentPattern = value
            myParentRegex = New System.Text.RegularExpressions.Regex(value)
        End Set
    End Property

    Private myRootPattern As String
    Private myRootRegex As System.Text.RegularExpressions.Regex
    <Parameter()> _
    Public Property RootPattern() As String
        Get
            Return myRootPattern
        End Get
        Set(ByVal value As String)
            myRootPattern = value
            myRootRegex = New System.Text.RegularExpressions.Regex(value)
        End Set
    End Property

#End Region


End Class
