
<Serializable()> _
Public Class InternalException
    Inherits System.Exception

    Public Sub New(ByVal message As String, ByVal ParamArray Args() As Object)
        Me.New(String.Format(message, Args))
    End Sub


    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub

    Public Sub New( _
        ByVal info As System.Runtime.Serialization.SerializationInfo, _
        ByVal context As System.Runtime.Serialization.StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class
