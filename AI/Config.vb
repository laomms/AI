Imports System.Runtime.InteropServices
Module Config
    Public Declare Function GetPrivateProfileSection Lib "kernel32.dll" Alias "GetPrivateProfileSectionA" (ByVal lpAppName As String, ByVal lpReturnedBuffer As IntPtr, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    Public Declare Function GetPrivateProfileSectionNames Lib "kernel32.dll" Alias "GetPrivateProfileSectionNamesA" (ByVal lpszReturnBuffer As IntPtr, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    Public Declare Function GetPrivateProfileString Lib "kernel32.dll" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedBuffer As IntPtr, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    Public Declare Function WritePrivateProfileSection Lib "kernel32.dll" Alias "WritePrivateProfileSectionA" (ByVal lpAppName As String, ByVal lpString As String, ByVal lpFileName As String) As Long
    Public Declare Function WritePrivateProfileString Lib "kernel32.dll" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Long

    Public Function ExisteSection(ByVal File As String, ByVal Section As String) As Boolean
        Dim PtrBuffer As IntPtr
        Dim ReturnValue As Integer

        PtrBuffer = Marshal.StringToHGlobalAnsi(New String(vbNullChar, 1024))
        ReturnValue = GetPrivateProfileSection(Section, PtrBuffer, 1024, File)

        Return ReturnValue
    End Function
    Public Function GetAllSections(ByVal File As String) As Collection
        Dim PtrBuffer As IntPtr
        Dim Sections As Collection
        Dim I, ReturnValue As Integer
        Dim AllSection, SectionItem As String

        PtrBuffer = Marshal.StringToHGlobalAnsi(New String(vbNullChar, 1024))
        ReturnValue = GetPrivateProfileSectionNames(PtrBuffer, 1024, File)
        AllSection = Marshal.PtrToStringAnsi(PtrBuffer, ReturnValue)
        Marshal.FreeHGlobal(PtrBuffer)

        Sections = New Collection
        SectionItem = ""
        For I = 0 To ReturnValue - 1
            If AllSection.Chars(I) = vbNullChar Then
                Sections.Add(SectionItem)
                SectionItem = ""
            Else
                SectionItem = SectionItem & AllSection.Chars(I)
            End If
        Next
        GetAllSections = Sections
        Sections = Nothing
    End Function
    Public Function GetSectionKeyNames(ByVal File As String, ByVal Section As String) As Collection
        Dim PtrBuffer As IntPtr
        Dim KeyNames As Collection
        Dim I, ReturnValue As Integer
        Dim AllKeyName, KeyContent As String

        PtrBuffer = Marshal.StringToHGlobalAnsi(New String(vbNullChar, 1024))
        ReturnValue = GetPrivateProfileSection(Section, PtrBuffer, 1024, File)
        AllKeyName = Marshal.PtrToStringAnsi(PtrBuffer, ReturnValue)
        Marshal.FreeHGlobal(PtrBuffer)
        KeyNames = New Collection
        KeyContent = ""
        Try
            For I = 0 To ReturnValue - 1
                If AllKeyName.Chars(I) = vbNullChar Then
                    KeyNames.Add(KeyContent)
                    KeyContent = ""
                Else
                    KeyContent = KeyContent & AllKeyName.Chars(I)
                End If
            Next
        Catch ex As Exception

        End Try

        GetSectionKeyNames = KeyNames
        KeyNames = Nothing
    End Function
    Public Function GetValueName(ByVal File As String, ByVal Section As String, ByVal KeyName As String) As String
        Dim PtrBuffer As IntPtr
        Dim ReturnValue As Integer
        Dim AllValue As String
        PtrBuffer = Marshal.StringToHGlobalAnsi(New String(vbNullChar, 1024))
        ReturnValue = GetPrivateProfileString(Section, KeyName, "", PtrBuffer, 255, File)
        AllValue = Marshal.PtrToStringAnsi(PtrBuffer, ReturnValue)
        Marshal.FreeHGlobal(PtrBuffer)
        GetValueName = AllValue
    End Function

    Public Function SetSection(ByVal File As String, ByVal Section As String, ByVal KeyValue As String) As Boolean
        SetSection = WritePrivateProfileSection(Section, KeyValue, File)
    End Function

    Public Function SetKeyName(ByVal File As String, ByVal Section As String, ByVal KeyName As String, ByVal KeyValue As String) As Boolean
        SetKeyName = WritePrivateProfileString(Section, KeyName, KeyValue, File)
    End Function

    Public Function DelSection(ByVal File As String, ByVal Section As String) As Boolean
        DelSection = WritePrivateProfileSection(Section, "", File)
    End Function

    Public Function DelKeyName(ByVal File As String, ByVal Section As String, ByVal KeyName As String) As Boolean
        DelKeyName = WritePrivateProfileString(Section, KeyName, "", File)
    End Function

    Public Sub IniWriteValue(Section As String, Key As String, Value As String, IniPath As String)
        Dim Values = WritePrivateProfileString(Section, Key, Value, IniPath)
    End Sub
    Public Function IniReadValue(Section As String, Key As String, IniPath As String, Optional defaultValue As String = "") As String
        Dim Ptrs As IntPtr = Marshal.StringToHGlobalAnsi(New String(vbNullChar, 1024))
        Dim Values = GetPrivateProfileString(Section, Key, defaultValue, Ptrs, 255, IniPath)
        If Values > 0 Then
            Return Marshal.PtrToStringAnsi(Ptrs, Values)
        Else
            Return defaultValue
        End If
    End Function
End Module
