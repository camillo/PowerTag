<AttributeUsage(AttributeTargets.Property, AllowMultiple:=False)> _
Public Class TaglibParameterAttribute : Inherits Attribute

    Public Sub New(ByVal TaglibName As String)
        Me.myTaglibName = TaglibName
    End Sub

    Private myTaglibName As String
    Public Property TaglibName() As String
        Get
            Return myTaglibName
        End Get
        Set(ByVal value As String)
            myTaglibName = value
        End Set
    End Property

End Class
