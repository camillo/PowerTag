Public MustInherit Class SetTagWrapperBase : Inherits EditTagBase
    Private myWrappedPropertyName As String
    Protected Sub New()
        Dim attr = Me.GetType.GetCustomAttributes(GetType(CmdletAttribute), True)
        If attr.Length = 0 Then Throw New InternalException("no CmdletAttribute found")
        Me.myWrappedPropertyName = DirectCast(attr(0), CmdletAttribute).NounName
    End Sub

    Protected Sub New(ByVal WrappedPropertyName As String)
        Me.myWrappedPropertyName = WrappedPropertyName
    End Sub

    Protected Overrides Function DoEdit(ByVal TargetFile As TagLib.File) As Boolean

        Dim input As Object
        Dim tmp As KeyValuePair(Of Reflection.PropertyInfo, TaglibParameterAttribute) = Nothing
        If Not Set_Tag.TryGetTaglibParemeter(myWrappedPropertyName, tmp) Then _
            Throw New InternalException("no property found for parameter '{0}'", myWrappedPropertyName)
        Dim targetType = tmp.Key.PropertyType
        Dim value = Me.Value
        If targetType.Equals(GetType(String)) Then
            input = New Object() {value}
        ElseIf targetType.Equals(GetType(String())) Then
            If TypeOf value Is String() Then
                input = value
            ElseIf TypeOf value Is IEnumerable(Of String) Then
                input = value
            ElseIf TypeOf value Is Array Then
                input = value
            Else
                input = New Object() {value.ToString}
            End If
        ElseIf targetType.Equals(GetType(Nullable(Of UInt32))) Then
            Dim uIntResult As UInt32
            If Not UInt32.TryParse(value.ToString, uIntResult) Then Throw New ArgumentException(String.Format("Cannot parse value '{0}' into UInt32", value), myWrappedPropertyName)
            input = New Object() {uIntResult}
        Else
            Throw New InternalException("unknown parameter type '{0}'", targetType.FullName)
        End If
        If ShouldProcess(TargetFile.Name, String.Format("set '{0}':'{1}'", myWrappedPropertyName, input)) Then
            WhatIfMode = WhatIfModes.false
        Else
            WhatIfMode = WhatIfModes.true
        End If
        Dim command = String.Format("Set-Tag -Filename ""{0}"" -{1} $input", TargetFile.Name.Replace("""", """"""), myWrappedPropertyName)
        Dim pipe = Runspaces.Runspace.DefaultRunspace.CreateNestedPipeline(command, False)
        Dim pipeResult = pipe.Invoke(input)
        Return True
    End Function

    Private myValue As Object
    <Parameter(Position:=0)> _
    Public Property Value() As Object
        Get
            Return myValue
        End Get
        Set(ByVal value As Object)
            myValue = value
        End Set
    End Property

    Protected Overrides Sub EndProcessing()
        WhatIfMode = WhatIfModes.none
    End Sub

End Class
