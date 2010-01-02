<Cmdlet("Do", "Test")> _
Public Class Do_Test : Inherits PSCmdlet


    Protected Overrides Sub BeginProcessing()
        MyBase.BeginProcessing()
    End Sub

    Protected Overrides Sub ProcessRecord()
        Dim s = New System.Text.StringBuilder
        s.AppendLine("#Region ""Taglib Parameter""")
        For Each prop In GetType(TagLib.Tag).GetProperties
            If prop.CanWrite Then
                Dim propName = prop.Name
                Dim propType = prop.PropertyType.Name.Replace("[]", "()")
                If propType.ToLower.Contains("picture") Then Continue For
                propType = propType.Replace("UInt32", "Nullable(Of UInt32)")
                s.AppendFormat("Private my{0} as {1}{2}", propName, propType, vbNewLine)
                s.AppendFormat("<Parameter(Mandatory:=False), TaglibParameter(""{0}"")> _{1}", propName, vbNewLine)
                s.AppendFormat("Public Property {0}() As {1}{2}", propName, propType, vbNewLine)
                s.AppendLine("Get")
                s.AppendFormat("Return my{0}{1}", propName, vbNewLine)
                s.AppendLine("End Get")
                s.AppendFormat("Set(ByVal value As {0}){1}", propType, vbNewLine)
                s.AppendFormat("my{0} = value{1}", propName, vbNewLine)
                s.AppendLine("End Set")
                s.AppendLine("End Property")
                s.AppendLine()
            End If
        Next
        s.AppendLine("#End Region")
        Me.WriteObject(s.ToString)
    End Sub

    Private myArrayTest As String()
    <Parameter(Mandatory:=False)> _
    Public Property ArrayTest() As String()
        Get
            Return myArrayTest
        End Get
        Set(ByVal value As String())
            myArrayTest = value
        End Set
    End Property

End Class
