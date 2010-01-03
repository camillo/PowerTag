Public MustInherit Class CmdLetBase : Inherits PSCmdlet
    Protected Const FilePrefix As String = "?:file"

    Protected Overrides Sub ProcessRecord()
        Try
            DoProcessRecord()
        Catch ex As ArgumentException
            Me.WriteError(New ErrorRecord(ex, "Argument", ErrorCategory.InvalidArgument, TargetObject))
        Catch ex As PipelineStoppedException
            Me.Host.UI.WriteLine(ex.Message)
        Catch ex As InternalException
            Me.WriteWarning("Internal error in PowerTag. Please report this bug.")
            Me.ThrowTerminatingError(New ErrorRecord(ex, Me.GetType.Name, ErrorCategory.InvalidOperation, Me.TargetObject))
        Catch ex As Exception
            Dim newEx = New InternalException("unknown exeption in PowerTag. Please report this bug", ex)
            Me.ThrowTerminatingError(New ErrorRecord(newEx, Me.GetType.Name, ErrorCategory.NotSpecified, Me.TargetObject))
        End Try
    End Sub

    Private myUltraVerbose As SwitchParameter
    <Parameter()> _
    Public Property UltraVerbose() As SwitchParameter
        Get
            Return myUltraVerbose
        End Get
        Set(ByVal value As SwitchParameter)
            myUltraVerbose = value
        End Set
    End Property

    Protected Overridable Sub DoProcessRecord()
    End Sub

    Protected Overloads Sub WriteUltraVerbose(ByVal Message As String, ByVal ParamArray Args() As Object)
        If Me.UltraVerbose.IsPresent Then
            Me.WriteVerbose(String.Format(Message, Args))
        End If
    End Sub

    Protected Overloads Sub WriteUltraVerbose(ByVal Message As String)
        If Me.UltraVerbose.IsPresent Then
            Me.WriteVerbose(Message)
        End If
    End Sub

    Protected Overloads Sub WriteVerbose(ByVal Message As String, ByVal ParamArray Args() As Object)
        Me.WriteVerbose(String.Format(Message, Args))
    End Sub

    Protected Overloads Sub WriteWarning(ByVal Message As String, ByVal ParamArray Args() As Object)
        Me.WriteWarning(String.Format(Message, Args))
    End Sub

    Private myTargetObject As Object = "-"
    Protected Property TargetObject() As Object
        Get
            Return myTargetObject
        End Get
        Set(ByVal value As Object)
            myTargetObject = value
        End Set
    End Property

End Class
