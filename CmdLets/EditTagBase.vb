Public MustInherit Class EditTagBase : Inherits CmdLetBase
    Public Const DefaultParameterSetName As String = "byFilenName"
    Public Const TagParaneterSetName As String = "byTag"
    Private Const HelpMessageFileName As String = "path to mediafile (mp3,ogg...)"
    Private Const HelpMessageTag As String = "target tag for operation"

    Protected Overrides Sub DoProcessRecord()
        Try
            Dim targetFile As TagLib.File

            Select Case Me.ParameterSetName
                Case TagParaneterSetName
                    targetFile = FileCache.GetFile(Me.Tag)
                Case DefaultParameterSetName
                    targetFile = FileCache.GetFile(Me.FileName)
                Case Else
                    Throw New InternalException(String.Format("unknown Parametersetname {0}", Me.ParameterSetName))
            End Select

            DoEdit(targetFile)

            If Me.NeedSave Then
                targetFile.Save()
            End If

            If Me.PassThru.IsPresent Then
                Me.WriteObject(targetFile.Tag)
            End If

        Catch ex As System.IO.FileNotFoundException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.ObjectNotFound, FileName))
        Catch ex As TagLibException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidResult, FileName))
        Catch ex As TagLib.UnsupportedFormatException
            Me.WriteWarning(String.Format("unsupported Format: '{0}", FileName))
        Catch ex As TagLib.CorruptFileException
            Me.WriteError(New ErrorRecord(ex, "GetTag", ErrorCategory.InvalidData, FileName))
        End Try

    End Sub

    Protected Overridable Sub DoEdit(ByVal TargetFile As TagLib.File)

    End Sub

#Region "Parameter"
    Private myFileName As String
    Private myTag As TagLib.Tag

    <Parameter(Position:=0, Mandatory:=True, ParameterSetName:=DefaultParameterSetName, ValueFromPipeline:=True, HelpMessage:=HelpMessageFileName)> _
    Public Property FileName() As String
        Get
            Return myFileName
        End Get
        Set(ByVal value As String)
            myFileName = value
        End Set
    End Property

    <Parameter(Position:=0, Mandatory:=True, ParameterSetName:=TagParaneterSetName, ValueFromPipeline:=True, HelpMessage:=HelpMessageTag)> _
    Public Property Tag() As TagLib.Tag
        Get
            Return myTag
        End Get
        Set(ByVal value As TagLib.Tag)
            myTag = value
        End Set
    End Property

    Private myPassThru As SwitchParameter
    <Parameter()> _
    Public Property PassThru() As SwitchParameter
        Get
            Return myPassThru
        End Get
        Set(ByVal value As SwitchParameter)
            myPassThru = value
        End Set
    End Property

    Private myVirtual As SwitchParameter
    <Parameter()> _
    Public Property Virtual() As SwitchParameter
        Get
            Return myVirtual
        End Get
        Set(ByVal value As SwitchParameter)
            myVirtual = value
        End Set
    End Property

    Protected ReadOnly Property NeedSave() As Boolean
        Get
            Return Not Me.Virtual.IsPresent
        End Get
    End Property
#End Region

End Class
