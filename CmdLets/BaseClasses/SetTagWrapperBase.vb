Public MustInherit Class SetTagWrapperBase : Inherits EditTagCmdLetBase
    Protected Const CommandTemplate As String = "Set-Tag -FullName ""{0}"" -{1} $input"
    Protected Const VirtualParameter As String = " -Virtual"

    Protected Const PropertyNotFoundMessage As String = "No property found for parameter '{0}'."
    Protected Const UintParseErrorMessage As String = "Cannot parse value '{0}' into UInt32."
    Protected Const UnknownParameterErrorMessage As String = "Unknown parameter type '{0}'."

#Region "process record"
    Protected Overrides Sub ProcessEditTag(ByVal TargetTag As Tag)

        Dim commandKvp = CreateCommandKvp(TargetTag)
        Dim pipeResult = ExecuteNewPipeline(commandKvp)

    End Sub
#End Region

#Region "private helper"
    Private Function CreateCommandKvp(ByVal TargetTag As Tag) As KeyValuePair(Of String, IEnumerable)
        Dim back As KeyValuePair(Of String, IEnumerable)
        Dim wrappedPropertyName = Me.Noun

        Dim taglibKvp As KeyValuePair(Of Reflection.PropertyInfo, TaglibParameterAttribute) = Nothing
        If Not Edit_Tag.TryGetTaglibParemeter(wrappedPropertyName, taglibKvp) Then _
            Throw New InternalException(PropertyNotFoundMessage, wrappedPropertyName)

        Dim input As IEnumerable = CreateInputObject(taglibKvp.Key.PropertyType, wrappedPropertyName)

        Dim FullnameParameterText = Util.ConvertToParameterText(TargetTag.Path)
        Dim command = String.Format(CommandTemplate, FullnameParameterText, wrappedPropertyName)
        If Me.Virtual.IsPresent Then command = String.Concat(command, VirtualParameter)

        back = New KeyValuePair(Of String, IEnumerable)(command, input)
        Return back
    End Function

    Private Function CreateInputObject(ByVal TargetType As Type, ByVal WrappedPropertyName As String) As IEnumerable
        Dim back As IEnumerable
        If TargetType.Equals(GetType(String)) Then
            back = New Object() {Value}
        ElseIf TargetType.Equals(GetType(String())) Then
            If TypeOf Value Is String() Then
                back = New Object() {Value}
            ElseIf TypeOf Value Is IEnumerable(Of String) Then
                back = New Object() {Value}
            ElseIf TypeOf Value Is Array Then
                back = Util.ConvertToStringEnumerable(DirectCast(Value, Array))
            Else
                back = New String() {Value.ToString}
            End If
        ElseIf TargetType.Equals(GetType(Nullable(Of UInt32))) Then
            Dim uIntResult As UInt32
            If Not UInt32.TryParse(Value.ToString, uIntResult) Then _
                Throw New ArgumentException(String.Format(UintParseErrorMessage, Value), WrappedPropertyName)
            back = New Object() {uIntResult}
        Else
            Throw New InternalException(UnknownParameterErrorMessage, TargetType.FullName)
        End If
        Return back
    End Function
#End Region

#Region "parameter"
    Private myValue As Object
    <Parameter()> _
    Public Property Value() As Object
        Get
            Return myValue
        End Get
        Set(ByVal value As Object)
            myValue = value
        End Set
    End Property
#End Region

End Class
