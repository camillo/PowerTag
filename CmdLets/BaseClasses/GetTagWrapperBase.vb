Public MustInherit Class GetTagWrapperBase : Inherits TagCmdLetBase
    Protected Const GetChildrenCommandPattern As String = "Get-ChildItem {0}| {1}"
    Protected Const CommandPattern As String = "Get-Tag -Fullname ""{0}"" "
    Protected Const RecursiveParameter As String = "-Recurse"
    Protected Const UniqueModifier As String = "| Select-Object -Unique"

    Protected Const TagLibPropertyNotFoundMessage As String = "cannot find tag property '{0}'"

#Region "process record"
    Protected Overrides Sub DoProcessRecord()
        Select Case Me.ParameterSetName
            Case NoParameterParameterSetName
                Dim command = String.Format(GetChildrenCommandPattern, Me.GetRecursivString, Me.Name)
                If Me.Unique.IsPresent Then command = String.Concat(command, UniqueModifier)
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

    Private Function GetRecursivString() As String
        Dim back As String
        If Me.Recurse.IsPresent Then
            back = RecursiveParameter
        Else
            back = String.Empty
        End If
        Return back
    End Function
#End Region

#Region "parameter"

    Private myRecurse As SwitchParameter
    <Parameter()> _
    Public Property Recurse() As SwitchParameter
        Get
            Return myRecurse
        End Get
        Set(ByVal value As SwitchParameter)
            myRecurse = value
        End Set
    End Property

    Private myUnique As SwitchParameter
    <Parameter()> _
    Public Property Unique() As SwitchParameter
        Get
            Return myUnique
        End Get
        Set(ByVal value As SwitchParameter)
            myUnique = value
        End Set
    End Property

#End Region
End Class
