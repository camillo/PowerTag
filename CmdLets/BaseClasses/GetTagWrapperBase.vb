Public MustInherit Class GetTagWrapperBase : Inherits TagCmdLetBase
    Protected Const GetChildrenCommandPattern As String = "Get-ChildItem | {0}"
    Protected Const CommandPattern As String = "Get-Tag -Fullname ""{0}"" "

    Protected Const TagLibPropertyNotFoundMessage As String = "cannot find tag property '{0}'"

#Region "process record"
    Protected Overrides Sub DoProcessRecord()
        Select Case Me.ParameterSetName
            Case NoParameterParameterSetName
                Dim command = String.Format(GetChildrenCommandPattern, Me.Name)
                Dim back = ExecuteNewPipeline(command, Nothing)
                Me.WriteObject(back, True)
            Case Else
                MyBase.DoProcessRecord()
        End Select
    End Sub

    Protected Overrides Sub ProcessTag(ByVal TargetTag As Tag)
        Dim wrappedPropertyName = Me.Noun
        Dim fullNameParameterText = Util.ConvertToParameterText(TargetTag.Path)
        Dim command = String.Format(CommandPattern, fullNameParameterText)

        For Each result In ExecuteNewPipeline(command, Nothing)
            Dim baseObject = result.BaseObject
            Dim targetProperty = GetTargetProperty(baseObject, wrappedPropertyName)
            If targetProperty Is Nothing Then Continue For

            Dim back = targetProperty.GetValue(baseObject, Nothing)
            Me.WriteObject(back)
        Next
    End Sub
#End Region

#Region "private helper"
    Private Shared Function GetTargetProperty(ByVal BaseObject As Object, ByVal WrappedPropertyName As String) As Reflection.PropertyInfo
        Dim back As Reflection.PropertyInfo
        If BaseObject Is Nothing Then
            back = Nothing
        Else
            Dim baseType = BaseObject.GetType
            back = baseType.GetProperty(WrappedPropertyName)
            If back Is Nothing Then Throw New InternalException(TagLibPropertyNotFoundMessage, WrappedPropertyName)
        End If
        Return back
    End Function
#End Region
End Class
