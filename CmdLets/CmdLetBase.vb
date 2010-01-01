Public MustInherit Class CmdLetBase : Inherits PSCmdlet
    Protected Const FilePrefix As String = "?:file"

    Protected Overrides Sub ProcessRecord()
        Try
            DoProcessRecord()
        Catch ex As InternalException
            Me.WriteWarning("Internal error in powertag. Please report this bug")
            Me.ThrowTerminatingError(New ErrorRecord(ex, Me.GetType.Name, ErrorCategory.InvalidOperation, Me.TargetObject))
        Catch ex As Exception
            Me.ThrowTerminatingError(New ErrorRecord(ex, Me.GetType.Name, ErrorCategory.NotSpecified, Me.TargetObject))
        End Try
    End Sub

    Protected Overridable Sub DoProcessRecord()
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
