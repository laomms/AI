Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Public Class Form1
    Friend Shared MyInstance As Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MyInstance = Me
        If File.Exists(IniFilePath) Then
            Dim Sections = GetAllSections(IniFilePath)
            If Sections.Count = 0 Then Return
            For Each Section In Sections
                If Section = "Baidu" Then
                    Dim KeysList = GetSectionKeyNames(IniFilePath, Section)
                    For Each Key In KeysList
                        If Key.ToString.StartsWith("APPID") Then
                            TextBox1.Text = Key.ToString.Replace("APPID=", "")
                        ElseIf Key.ToString.StartsWith("APPKEY") Then
                            TextBox2.Text = Key.ToString.Replace("APPKEY=", "")
                        ElseIf Key.ToString.StartsWith("SecretKey") Then
                            TextBox3.Text = Key.ToString.Replace("SecretKey=", "")
                        ElseIf Key.ToString.StartsWith("APP_ID") Then
                            TextBox8.Text = Key.ToString.Replace("APP_ID=", "")
                        ElseIf Key.ToString.StartsWith("APP_KEY") Then
                            TextBox9.Text = Key.ToString.Replace("APP_KEY=", "")
                        End If
                    Next
                ElseIf Section = "Tencent" Then
                    Dim KeysList = GetSectionKeyNames(IniFilePath, Section)
                    For Each Key In KeysList
                        If Key.ToString.StartsWith("APPID") Then
                            TextBox4.Text = Key.ToString.Replace("APPID=", "")
                        ElseIf Key.ToString.StartsWith("SecretId") Then
                            TextBox6.Text = Key.ToString.Replace("SecretId=", "")
                        ElseIf Key.ToString.StartsWith("SecretKey") Then
                            TextBox7.Text = Key.ToString.Replace("SecretKey=", "")
                        ElseIf Key.ToString.StartsWith("APP_ID") Then
                            TextBox5.Text = Key.ToString.Replace("APP_ID=", "")
                        ElseIf Key.ToString.StartsWith("APP_KEY") Then
                            TextBox10.Text = Key.ToString.Replace("APP_KEY=", "")
                        End If
                    Next
                End If
            Next
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim path = Environment.CurrentDirectory & "\main\data\config\AI.ini"
        If Not System.IO.Directory.Exists(Environment.CurrentDirectory + "\main\data\config\") Then
            System.IO.Directory.CreateDirectory(Environment.CurrentDirectory + "\main\data\config\")
            WritePrivateProfileString("Baidu", "APPID", TextBox1.Text, IniFilePath)
            WritePrivateProfileString("Baidu", "APPKEY", TextBox2.Text, IniFilePath)
            WritePrivateProfileString("Baidu", "SecretKey", TextBox3.Text, IniFilePath)
            WritePrivateProfileString("Baidu", "APP_ID", TextBox8.Text, IniFilePath)
            WritePrivateProfileString("Baidu", "APP_KEY", TextBox9.Text, IniFilePath)
            MessageBox.Show("设置成功.")
        Else
            If Not File.Exists(IniFilePath) Then
                WritePrivateProfileString("Baidu", "APPID", TextBox1.Text, IniFilePath)
                WritePrivateProfileString("Baidu", "APPKEY", TextBox2.Text, IniFilePath)
                WritePrivateProfileString("Baidu", "SecretKey", TextBox3.Text, IniFilePath)
                WritePrivateProfileString("Baidu", "APP_ID", TextBox8.Text, IniFilePath)
                WritePrivateProfileString("Baidu", "APP_KEY", TextBox9.Text, IniFilePath)
                MessageBox.Show("设置成功.")
            Else
                Dim Sections = GetAllSections(IniFilePath)
                For Each Section In Sections
                    If Section = "Baidu" Then
                        WritePrivateProfileString("Baidu", "APPID", TextBox1.Text, IniFilePath)
                        WritePrivateProfileString("Baidu", "APPKEY", TextBox2.Text, IniFilePath)
                        WritePrivateProfileString("Baidu", "SecretKey", TextBox3.Text, IniFilePath)
                        WritePrivateProfileString("Baidu", "APP_ID", TextBox8.Text, IniFilePath)
                        WritePrivateProfileString("Baidu", "APP_KEY", TextBox9.Text, IniFilePath)
                        MessageBox.Show("设置成功.")
                        Exit For
                    End If
                Next
            End If
        End If
        Baidu_APPID = TextBox1.Text
        Baidu_APPKEY = TextBox2.Text
        Baidu_SecretKey = TextBox3.Text
        Baidu_APP_ID = TextBox8.Text
        Baidu_APP_KEY = TextBox9.Text
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim path = Environment.CurrentDirectory & "\main\data\config\AI.ini"
        If Not System.IO.Directory.Exists(Environment.CurrentDirectory + "\main\data\config\") Then
            System.IO.Directory.CreateDirectory(Environment.CurrentDirectory + "\main\data\config\")
            WritePrivateProfileString("Tencent", "APPID", TextBox4.Text, IniFilePath)
            WritePrivateProfileString("Tencent", "SecretId", TextBox6.Text, IniFilePath)
            WritePrivateProfileString("Tencent", "SecretKey", TextBox7.Text, IniFilePath)
            WritePrivateProfileString("Tencent", "APP_ID", TextBox5.Text, IniFilePath)
            WritePrivateProfileString("Tencent", "APP_KEY", TextBox10.Text, IniFilePath)
            MessageBox.Show("设置成功.")
        Else
            If Not File.Exists(IniFilePath) Then
                WritePrivateProfileString("Tencent", "APPID", TextBox4.Text, IniFilePath)
                WritePrivateProfileString("Tencent", "SecretId", TextBox6.Text, IniFilePath)
                WritePrivateProfileString("Tencent", "SecretKey", TextBox7.Text, IniFilePath)
                WritePrivateProfileString("Tencent", "APP_ID", TextBox5.Text, IniFilePath)
                WritePrivateProfileString("Tencent", "APP_KEY", TextBox10.Text, IniFilePath)
                MessageBox.Show("设置成功.")
            Else
                Dim Sections = GetAllSections(IniFilePath)
                For Each Section In Sections
                    If Section = "Tencent" Then
                        WritePrivateProfileString("Tencent", "APPID", TextBox4.Text, IniFilePath)
                        WritePrivateProfileString("Tencent", "SecretId", TextBox6.Text, IniFilePath)
                        WritePrivateProfileString("Tencent", "SecretKey", TextBox7.Text, IniFilePath)
                        WritePrivateProfileString("Tencent", "APP_ID", TextBox5.Text, IniFilePath)
                        WritePrivateProfileString("Tencent", "APP_KEY", TextBox10.Text, IniFilePath)
                        MessageBox.Show("设置成功.")
                        Exit For
                    End If
                Next
            End If

        End If
        Tencent_APPID = TextBox4.Text
        Tencent_SecretId = TextBox6.Text
        Tencent_SecretKey = TextBox7.Text
        Tencent_APP_ID = TextBox5.Text
        Tencent_APP_KEY = TextBox10.Text
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        System.Diagnostics.Process.Start(LinkLabel1.Text) '利用Process.Start来打开
        LinkLabel1.LinkVisited = True
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        System.Diagnostics.Process.Start(LinkLabel2.Text) '利用Process.Start来打开
        LinkLabel2.LinkVisited = True
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        System.Diagnostics.Process.Start(LinkLabel3.Text) '利用Process.Start来打开
        LinkLabel3.LinkVisited = True
    End Sub

    Private Sub LinkLabel4_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel4.LinkClicked
        System.Diagnostics.Process.Start(LinkLabel4.Text) '利用Process.Start来打开
        LinkLabel4.LinkVisited = True
    End Sub
End Class