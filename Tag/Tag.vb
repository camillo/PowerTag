Partial Public Class Tag

    Protected Friend Sub New(ByVal File As TagLib.File)
        If File Is Nothing Then Throw New InternalException("File must not be null")
        myFile = File
    End Sub

    Public Sub Reload()
        Dim path = Me.Path
        WriteVerbose("reloading '{0}'", path)
        myFile = TagLib.File.Create(path)
    End Sub

    Public Sub Reset()
        Dim path = Me.Path
        SyncLock Cache
            If Cache.ContainsKey(path) Then
                WriteVerbose("remove item from cache")
                Cache.Remove(path)
            Else
                WriteVerbose("item not in cache; no remove needed")
            End If
        End SyncLock
    End Sub

    Public ReadOnly Property Filename() As String
        Get
            Return System.IO.Path.GetFileName(Me.Path)
        End Get
    End Property

#Region "wrapped TagLib Methods"
    Public Sub Save()
        WriteVerbose("saving '{0}'", Me.Path)
        myFile.Save()
    End Sub

    Public Sub Clear()
        WriteVerbose("clearing tag")
        Me.BaseTag.Clear()
    End Sub
#End Region

#Region "Wrapped Taglib Properties"
    Public ReadOnly Property Path() As String
        Get
            Return myFile.Name
        End Get
    End Property

    Public Property Title() As String
        Get
            Return Me.BaseTag.Title
        End Get
        Set(ByVal value As String)
            Me.BaseTag.Title = value
        End Set
    End Property

    Public Property Year() As UInt32
        Get
            Return Me.BaseTag.Year
        End Get
        Set(ByVal value As UInt32)
            Me.BaseTag.Year = value
        End Set
    End Property

    Public Property Genres() As String()
        Get
            Return Me.BaseTag.Genres
        End Get
        Set(ByVal value As String())
            Me.BaseTag.Genres = value
        End Set
    End Property
#End Region

#Region "Taglib base Properties"
    Private myFile As TagLib.File
    Public ReadOnly Property File() As TagLib.File
        Get
            Return myFile
        End Get
    End Property

    Public ReadOnly Property BaseTag() As TagLib.Tag
        Get
            Return Me.File.Tag
        End Get
    End Property
#End Region

#Region "logging"
    Friend Delegate Sub WriteLog(ByVal Message As String)

    Private Shared WriteWarningHandler As New WriteLog(AddressOf DefaultLog)
    Friend Shared Property WarningHandler() As WriteLog
        Get
            Return WriteWarningHandler
        End Get
        Set(ByVal value As WriteLog)
            WriteWarningHandler = value
        End Set
    End Property

    Private Shared WriteVerboseHandler As New WriteLog(AddressOf DefaultLog)
    Friend Shared Property VerboseHandler() As WriteLog
        Get
            Return WriteVerboseHandler
        End Get
        Set(ByVal value As WriteLog)
            If value Is Nothing Then
                WriteVerboseHandler = New WriteLog(AddressOf DefaultLog)
            Else
                WriteVerboseHandler = value
            End If
        End Set
    End Property

    Private Shared Sub DefaultLog(ByVal Message As String)
    End Sub

    Protected Shared Sub WriteVerbose(ByVal Message As String)
        Try
            VerboseHandler.Invoke(Message)
        Catch ex As Exception
            Console.WriteLine("error, writing verbose: {0}", ex)
        End Try
    End Sub

    Protected Shared Sub WriteVerbose(ByVal Message As String, ByVal ParamArray Args() As Object)
        Try
            WriteVerbose(String.Format(Message, Args))
        Catch ex As Exception
            Console.WriteLine("error, writing verbose: {0}", ex)
        End Try
    End Sub

    Protected Shared Sub WriteWarning(ByVal Message As String)
        Try
            WarningHandler.Invoke(Message)
        Catch ex As Exception
            Console.WriteLine("error, writing warning: {0}", ex)
        End Try
    End Sub

    Protected Shared Sub WriteWarning(ByVal Message As String, ByVal ParamArray Args() As Object)
        Try
            WriteWarning(String.Format(Message, Args))
        Catch ex As Exception
            Console.WriteLine("error, writing warning: {0}", ex)
        End Try
    End Sub

#End Region 'logging

End Class
