﻿<Cmdlet(VerbsCommon.Set, TagNounes.Tag, _
        SupportsShouldProcess:=True, DefaultParameterSetName:=Set_Tag.DefaultParameterSetName)> _
Public Class Set_Tag : Inherits EditTagBase
    Protected Overrides Sub DoEdit(ByVal TargetFile As TagLib.File)
        For Each prop In Me.GetType.GetProperties()
            If prop.DeclaringType.Equals(Me.GetType) Then
                If prop.GetCustomAttributes(GetType(ParameterAttribute), False).Length = 1 Then
                    Me.WriteVerbose("try property '{0}'", prop.Name)
                    ProcessProperty(TargetFile, prop)
                Else
                    Me.WriteVerbose("skipping property '{0}' (no parameter)", prop.Name)
                End If
            Else
                Me.WriteVerbose("skipping property '{0}' (not mine)", prop.Name)
            End If
        Next
    End Sub

    Private Sub ProcessProperty(ByVal TargetFile As TagLib.File, ByVal Prop As Reflection.PropertyInfo)
        Dim name = Prop.Name
        Dim tagProperty As Reflection.PropertyInfo = Nothing
        If TryGetTaglibProperty(name, tagProperty) Then
            Dim valueObj = Prop.GetValue(Me, Nothing)
            If Not valueObj Is Nothing Then
                If tagProperty.CanWrite Then
                    Me.WriteVerbose("set property '{0}' to '{1}'", name, valueObj)
                    SetProperty(tagProperty, valueObj, TargetFile)
                Else
                    Me.WriteWarning("property {0} is not writable.", name)
                End If
            Else
                Me.WriteVerbose("nothing to do '{0}' (no value set)", name)
            End If
        Else
            Me.WriteVerbose("nothing to do '{0}' (no matching taglib property", name)
        End If
    End Sub

    Private Sub SetProperty(ByVal TagProperty As Reflection.PropertyInfo, ByVal ValueObj As Object, ByVal TargetFile As TagLib.File)
        Dim TagPropertyType = TagProperty.PropertyType
        Dim realValue As Object
        If TagPropertyType.Equals(GetType(String)) Then
            realValue = ExtractValue(ValueObj.ToString, TargetFile)
        ElseIf TagPropertyType.Equals(GetType(String())) Then
            Dim valueList = New List(Of String)
            For Each value In ValueObj.ToString.Split(";"c)
                valueList.Add(ExtractValue(value, TargetFile))
            Next
            realValue = valueList.ToArray
        ElseIf TagPropertyType.Equals(GetType(UInteger)) Then
            Dim value As UInteger
            If Not UInteger.TryParse(ValueObj.ToString, value) Then Throw New ArgumentException(String.Format("cannot parse value '{0}' to UInteger", ValueObj))
            realValue = value
        Else
            Throw New InternalException(String.Concat("unknown tagtype ", TagPropertyType.Name))
        End If
        Dim tag = TargetFile.Tag
        TagProperty.SetValue(tag, realValue, Nothing)
    End Sub

    Private Function ExtractValue(ByVal Value As String, ByVal TargetFile As TagLib.File) As String
        Dim back As String = Value
        If back.Contains(FilePrefix) Then
            Dim replaceValue = System.IO.Path.GetFileNameWithoutExtension(TargetFile.Name)
            Dim pattern = Me.Match
            If Not String.IsNullOrEmpty(pattern) Then
                Dim match = System.Text.RegularExpressions.Regex.Match(replaceValue, pattern)
                If match.Success Then
                    If match.Groups.Count > 0 Then
                        replaceValue = match.Groups(match.Groups.Count - 1).Value
                    Else
                        replaceValue = match.Value
                    End If
                End If
            End If
            back = back.Replace(FilePrefix, replaceValue)
        End If
        Me.WriteVerbose("extracted value: '{0}'; input: '{1}'", back, Value)
        Return back
    End Function

    Private myMatch As String
    <Parameter()> _
    Public Property Match() As String
        Get
            Return myMatch
        End Get
        Set(ByVal value As String)
            myMatch = value
        End Set
    End Property

    Private Shared Function TryGetTaglibProperty(ByVal Name As String, ByRef TagLibProperty As Reflection.PropertyInfo) As Boolean
        Dim back As Boolean = False
        For Each prop In GetType(TagLib.Tag).GetProperties
            If prop.Name.ToLower.Equals(Name.ToLower) Then
                TagLibProperty = prop
                back = True
                Exit For
            End If
        Next
        Return back
    End Function

    Private myTitle As String
    <Parameter()> _
    Public Property Title() As String
        Get
            Return myTitle
        End Get
        Set(ByVal value As String)
            myTitle = value
        End Set
    End Property

End Class
