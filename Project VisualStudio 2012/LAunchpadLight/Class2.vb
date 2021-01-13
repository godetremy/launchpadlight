Imports System.Linq.Expressions


Public Class Class2

    Private atoto As String
    Private atoto2 As String

    Public Property toto As String
        Get
            Return atoto
        End Get
        Set(value As String)
            SetValeur(atoto, value)
        End Set
    End Property

    Public Property toto2 As String
        Get
            Return atoto2
        End Get
        Set(value As String)
            SetValeur(atoto2, value)
        End Set
    End Property
    Public Sub SetValeur(ByRef propriete As String, ByVal val As String)


    End Sub

End Class
