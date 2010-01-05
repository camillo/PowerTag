Public MustInherit Class CmdLetBase : Inherits PSCmdlet
    Public Const DefaulExceptionMessage As String = "Unknown exeption in PowerTag. Please report this bug."
    Protected Const UnsupportedFormatMessage As String = "unsupported Format: '{0}'"

#Region "ctr / init"
    Public Sub New()
        Try
            InitReflectionProperties()
        Catch ex As Exception
            Throw New InternalException("error, creating CmdLet", ex)
        End Try
    End Sub

    Private Sub InitReflectionProperties()
        Dim attr = Me.GetType.GetCustomAttributes(GetType(CmdletAttribute), True)
        If attr.Length = 0 Then Throw New InternalException("no CmdletAttribute found for type '{0}'", Me.GetType.FullName)
        Dim cmdLetAttribute = DirectCast(attr(0), CmdletAttribute)
        Me.myVerb = cmdLetAttribute.VerbName
        Me.myNoun = cmdLetAttribute.NounName
    End Sub

#End Region

#Region "process record"
    Protected Overrides Sub ProcessRecord()
        Dim enterVerboseHandler = Tag.VerboseHandler
        Dim enterWarningHanlder = Tag.WarningHandler
        Try
            Tag.VerboseHandler = AddressOf WriteVerbose
            Tag.WarningHandler = AddressOf WriteWarning
            DoProcessRecord()
        Catch ex As ArgumentException
            Me.WriteError(New ErrorRecord(ex, Me.DefaultErrorId, ErrorCategory.InvalidArgument, ex.ParamName))
        Catch ex As PipelineStoppedException
            Me.Host.UI.WriteLine(ex.Message)
        Catch ex As InternalException
            Me.ThrowTerminatingError(New ErrorRecord(ex, Me.DefaultErrorId, ErrorCategory.InvalidOperation, Me))
        Catch ex As Exception
            Dim newEx = New InternalException(DefaulExceptionMessage, ex)
            Me.ThrowTerminatingError(New ErrorRecord(newEx, Me.DefaultErrorId, ErrorCategory.NotSpecified, Me))
        Finally
            Tag.VerboseHandler = enterVerboseHandler
            Tag.WarningHandler = enterWarningHanlder
        End Try
    End Sub

    Protected Overridable Sub DoProcessRecord()
    End Sub
#End Region

#Region "protected helper"
    Protected Overloads Sub WriteWarning(ByVal Message As String, ByVal arg0 As String, ByVal ParamArray Args() As Object)
        Me.WriteWarning(String.Format(Message, arg0, Args))
    End Sub

    Protected Overloads Sub WriteVerbose(ByVal Message As String, ByVal arg0 As String, ByVal ParamArray Args() As Object)
        Me.WriteVerbose(String.Format(Message, arg0, Args))
    End Sub

    Protected Function ExecuteNewPipeline(ByVal ComandKvp As KeyValuePair(Of String, IEnumerable)) As System.Collections.ObjectModel.Collection(Of PSObject)
        Return ExecuteNewPipeline(ComandKvp.Key, ComandKvp.Value)
    End Function

    Protected Function ExecuteNewPipeline(ByVal Command As String, ByVal Input As IEnumerable) As System.Collections.ObjectModel.Collection(Of PSObject)
        Me.WriteVerbose("executing '{0}'", Command)
        Dim pipe = Runspaces.Runspace.DefaultRunspace.CreateNestedPipeline(Command, False)
        Dim pipeResult = pipe.Invoke(Input)
        Return pipeResult
    End Function

    Protected ReadOnly Property Name() As String
        Get
            Dim back = String.Format("{0}-{1}", Me.Verb, Me.Noun)
            Return back
        End Get
    End Property

#End Region

#Region "Protected Properties"
    Protected ReadOnly Property SessionPath() As String
        Get
            Return Me.SessionState.Path.CurrentLocation.Path
        End Get
    End Property

    Private myVerb As String
    Protected ReadOnly Property Verb() As String
        Get
            Return myVerb
        End Get
    End Property

    Private myNoun As String
    Protected ReadOnly Property Noun() As String
        Get
            Return myNoun
        End Get
    End Property

    Protected ReadOnly Property DefaultErrorId() As String
        Get
            Return Me.GetType.Name
        End Get
    End Property
#End Region









End Class
