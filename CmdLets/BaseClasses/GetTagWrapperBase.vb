Public MustInherit Class GetTagWrapperBase : Inherits TagCmdLetBase

    Private myWrappedPropertyName As String
    Protected Sub New()
        Dim attr = Me.GetType.GetCustomAttributes(GetType(CmdletAttribute), True)
        If attr.Length = 0 Then Throw New InternalException("no CmdletAttribute found")
        Me.myWrappedPropertyName = DirectCast(attr(0), CmdletAttribute).NounName
    End Sub

    Protected Sub New(ByVal WrappedPropertyName As String)
        Me.myWrappedPropertyName = WrappedPropertyName
    End Sub

    Protected Overrides Sub ProcessTag(ByVal TargetTag As Tag)
        Dim command = String.Format("Get-Tag -Path ""{0}"" ", TargetTag.Path.Replace("""", """"""))
        Dim pipe = Runspaces.Runspace.DefaultRunspace.CreateNestedPipeline(command, False)
        Dim pipeResult = pipe.Invoke()
        For Each result In pipeResult
            Dim baseObject = result.BaseObject
            If baseObject Is Nothing Then Continue For
            Dim baseType = baseObject.GetType
            'If Not baseType.Equals(GetType(TagLib.Tag)) Then Throw New InternalException(String.Concat("unexpected type ", baseObject.GetType.FullName))
            Dim targetProperty = baseType.GetProperty(myWrappedPropertyName)
            If targetProperty Is Nothing Then Throw New InternalException("cannot find tag property '{0}'", myWrappedPropertyName)
            Dim back = targetProperty.GetValue(baseObject, Nothing)
            Me.WriteObject(back)
        Next
    End Sub

    Protected Overrides Sub DoProcessRecord()
        Select Case Me.ParameterSetName
            Case NoParameterParameterSetName
                Dim myCmdLets = Me.GetType.GetCustomAttributes(GetType(CmdletAttribute), False)
                If myCmdLets.Length = 0 Then Throw New InternalException("unable to get cmdlet attribute.")
                Dim myCmdLet = DirectCast(myCmdLets(0), CmdletAttribute)
                Dim command = String.Format("Get-ChildItem | {0}-{1}", myCmdLet.VerbName, myCmdLet.NounName)
                Me.WriteVerbose("executing '{0}'", command)
                Dim pipe = Runspaces.Runspace.DefaultRunspace.CreateNestedPipeline(command, False)
                Dim back = pipe.Invoke()
                Me.WriteObject(back, True)
            Case Else
                MyBase.DoProcessRecord()
        End Select

    End Sub
End Class
