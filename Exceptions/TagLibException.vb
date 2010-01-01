Public Class TagLibException : Inherits Exception
    Private Const DefaultMessage As String = "internal problem with TagLib"

    Public Sub New()
        Me.New(DefaultMessage)
    End Sub

    Public Sub New(ByVal Message As String)
        MyBase.New(Message)
    End Sub

End Class
