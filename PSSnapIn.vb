Imports System.Configuration.Install
Imports System.Management.Automation
Imports System.ComponentModel

<RunInstaller(True)> _
Public Class PowerTagSnapIn
    Inherits PSSnapIn
    Private Const FormatsName As String = "PowerTagFormats.ps1xml"
    Private Const TypesName As String = "PowerTagTypes.ps1xml"

    Overrides ReadOnly Property Name() As String
        Get
            Return "PowerTag"
        End Get
    End Property
    Overrides ReadOnly Property Vendor() As String
        Get
            Return "Daniel Marohn"
        End Get
    End Property
    Overrides ReadOnly Property VendorResource() As String
        Get
            Return "PowerTag,"
        End Get
    End Property
    Overrides ReadOnly Property DescriptionResource() As String
        Get
            Return "PowerTag,Registers the Cmdlets and Providers in this assembly "
        End Get

    End Property
    Overrides ReadOnly Property Description() As String
        Get
            Return "allows to view and edit mediatags"
        End Get
    End Property

    Public Overrides ReadOnly Property Formats() As String()
        Get
            Return New String() {FormatsName}
        End Get
    End Property

    Public Overrides ReadOnly Property Types() As String()
        Get
            Return New String() {TypesName}
        End Get
    End Property

End Class
