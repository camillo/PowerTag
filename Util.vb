Public Class Util

    Public Shared Function GetFullPath(ByVal Filename As String, ByVal WorkPath As String) As String
        Dim back As String
        If IO.Path.IsPathRooted(Filename) Then
            back = Filename
        Else
            back = IO.Path.GetFullPath(IO.Path.Combine(WorkPath, Filename))
        End If
        Return back
    End Function

    Public Shared Function ConvertToStringEnumerable(ByVal Items As Array) As IEnumerable
        Dim back = New List(Of String)
        For Each item In Items
            back.Add(item.ToString)
        Next
        Return back.ToArray
    End Function

    Public Shared Function ConvertToParameterText(ByVal Item As String) As String
        Return Item.Replace("""", """""")
    End Function

    Public Shared Function ToArray(Of T)(ByVal Items As Collections.ObjectModel.Collection(Of PSObject)) As IEnumerable(Of T)
        Dim back = New List(Of T)
        For Each item In Items
            Dim obj = item.BaseObject
            If Not TypeOf obj Is T Then Throw New InternalException("unable to cast object {0} from type {1} to {2}", obj, obj.GetType.FullName, GetType(T).FullName)
            back.Add(DirectCast(obj, T))
        Next
        Return back.ToArray
    End Function
#Region "debug only"
#If DEBUG Then
    ''' <summary>this is a helperclass to generate code. No need to compile it into relase binaries. </summary>
    Friend Shared Function GenerateWrapper() As String
        Dim back = New System.Text.StringBuilder
        back.AppendLine("#Region ""Taglib Parameter""")
        For Each prop In GetType(TagLib.Tag).GetProperties
            If prop.CanWrite Then
                Dim propName = prop.Name
                Dim propType = prop.PropertyType.Name.Replace("[]", "()")
                If propType.ToLower.Contains("picture") Then Continue For
                propType = propType.Replace("UInt32", "Nullable(Of UInt32)")
                back.AppendFormat("Private my{0} as {1}{2}", propName, propType, vbNewLine)
                back.AppendFormat("<Parameter(Mandatory:=False), TaglibParameter(""{0}"")> _{1}", propName, vbNewLine)
                back.AppendFormat("Public Property {0}() As {1}{2}", propName, propType, vbNewLine)
                back.AppendLine("Get")
                back.AppendFormat("Return my{0}{1}", propName, vbNewLine)
                back.AppendLine("End Get")
                back.AppendFormat("Set(ByVal value As {0}){1}", propType, vbNewLine)
                back.AppendFormat("my{0} = value{1}", propName, vbNewLine)
                back.AppendLine("End Set")
                back.AppendLine("End Property")
                back.AppendLine()
            End If
        Next
        back.AppendLine("#End Region")
        Return back.ToString
    End Function
#End If
#End Region

End Class
