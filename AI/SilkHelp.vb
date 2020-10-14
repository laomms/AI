Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Threading

Public Class SilkHelp

    Private Shared finished As Boolean = False

    Public Shared Function GetByteFromamr(ByVal audio_url As String) As Byte()
        If audio_url = "" Then Return Nothing
        Dim rootdicpath As String = Environment.CurrentDirectory
        Dim audioslik As String = rootdicpath & "\main\data\voice"
        If Not Directory.Exists(audioslik) Then
            Dim dic As DirectoryInfo = Directory.CreateDirectory(audioslik)
        End If
        If File.Exists(audioslik + "\temp_audio.mp3") Then
            File.Delete(audioslik + "\temp_audio.mp3")
        End If
        If File.Exists(audioslik + "\temp_audio.silk") Then
            File.Delete(audioslik + "\temp_audio.silk")
        End If
        If File.Exists(audioslik + "\temp_audio.pcm") Then
            File.Delete(audioslik + "\temp_audio.pcm")
        End If
        Using client = New WebClient()
            finished = False
            client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)")
            AddHandler client.DownloadFileCompleted, AddressOf OnDownloadComplete
            client.DownloadFileAsync(New System.Uri(audio_url), audioslik + "\temp_audio.amr")
        End Using
        Do Until finished = True
            Thread.Sleep(1)
        Loop
        Return File.ReadAllBytes(audioslik + "\temp_audio.silk")
    End Function
    Private Shared Sub OnDownloadComplete(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs)
        If Not e.Cancelled AndAlso e.Error Is Nothing Then
            finished = True
        End If
    End Sub
    '1.转换silk到mp3：
    'ffmpeg -i test.silk test.mp3
    '2.转换silk到wav：
    'ffmpeg  -i test.silk test.wav
    '3.转换mp3到wav：
    'ffmpeg -i test.mp3 -f wav test.wav

    ''' <summary>
    ''' Silk解码
    ''' </summary>
    ''' <param name="audio_url"></param>
    ''' <returns></returns>
    Public Shared Function UrlSilkDecoding(ByVal audio_url As String) As Byte()
        If audio_url = "" Then Return Nothing
        Dim rootdicpath As String = Environment.CurrentDirectory
        Dim ffmpge As String = rootdicpath & "\main\corn\ffmpeg.exe"
        Dim silkdecode As String = rootdicpath & "\main\corn\silkdecode.exe" '解码
        Dim silkencode As String = rootdicpath & "\main\corn\silkencode.exe" '编码
        Dim audioslik As String = rootdicpath & "\main\data\voice"
        If Not Directory.Exists(audioslik) Then
            Dim dic As DirectoryInfo = Directory.CreateDirectory(audioslik)
        End If
        If File.Exists(audioslik + "\temp_audio.silk") Then
            File.Delete(audioslik + "\temp_audio.silk")
        End If
        If File.Exists(audioslik + "\temp_audio.mp3") Then
            File.Delete(audioslik + "\temp_audio.mp3")
        End If
        If File.Exists(audioslik + "\temp_audio.pcm") Then
            File.Delete(audioslik + "\temp_audio.pcm")
        End If
        Using client = New WebClient()
            finished = False
            client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)")
            AddHandler client.DownloadFileCompleted, AddressOf OnDownloadComplete
            client.DownloadFileAsync(New System.Uri(audio_url), audioslik + "\temp_audio.silk")
        End Using
        Do Until finished = True
            Thread.Sleep(1)
        Loop
        Dim audio_path = audioslik + "\temp_audio.silk"
        If File.Exists(ffmpge) AndAlso File.Exists(silkdecode) AndAlso File.Exists(silkencode) Then
            Dim name As String = audio_path.Substring(audio_path.LastIndexOf("\") + 1)
            Dim tempname As String = audioslik & "\" & name.Substring(0, name.LastIndexOf("."))
            Runcmd(silkdecode, audioslik, $"""{audio_path}"" ""{tempname}.pcm""")  'silk转pcm:   silkdecode "name.silk" "name2.pcm"
            Return GetByte($"{tempname}.pcm")
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Silk解码
    ''' </summary>
    ''' <param name="audio_path"></param>
    ''' <returns></returns>
    Public Shared Function SilkDecoding(ByVal audio_path As String) As Byte()
        If Not File.Exists(audio_path) Then
            Return Nothing
        End If
        Dim rootdicpath As String = Environment.CurrentDirectory
        Dim ffmpge As String = rootdicpath & "\main\corn\ffmpeg.exe"
        Dim silkdecode As String = rootdicpath & "\main\corn\silkdecode.exe" '解码
        Dim silkencode As String = rootdicpath & "\main\corn\silkencode.exe" '编码
        If File.Exists(ffmpge) AndAlso File.Exists(silkdecode) AndAlso File.Exists(silkencode) Then
            Dim name As String = audio_path.Substring(audio_path.LastIndexOf("\") + 1)
            Dim audioslik As String = rootdicpath & "\main\data\voice"
            If Not Directory.Exists(audioslik) Then
                Dim dic As DirectoryInfo = Directory.CreateDirectory(audioslik)
            End If
            Dim tempname As String = audioslik & "\" & name.Substring(0, name.LastIndexOf("."))
            Dim arg As String = $" -i ""{name}"" ""{tempname}.mp3"""
            'ffmpeg -i "name.silk" "name1.mp3"
            Try
                Runcmd(ffmpge, audioslik, arg)
                Return GetByte($"{tempname}.mp3")
            Catch e1 As Exception
                'silkdecode "name.silk" "name2.pcm"
                arg = $"""{audio_path}"" ""{tempname}.pcm"""
                Runcmd(silkdecode, audioslik, arg)
                'ffmpeg -f s16le -ar 24000 -ac 1 -i "name2.pcm" "name2.mp3"
                arg = $" -f s16le -ar 24000 -ac 1 -i ""{tempname}.pcm"" ""{tempname}.mp3"""
                Runcmd(ffmpge, audioslik, arg)
                Return GetByte($"{tempname}.mp3")
            End Try
        End If
        Return Nothing
    End Function
    ''' <summary>
    ''' Silk编码
    ''' </summary>
    ''' <param name="audio_path"></param>
    ''' <returns></returns>
    Public Shared Function SilkEncoding(ByVal audio_path As String) As Byte()
        If Not File.Exists(audio_path) Then
            Return Nothing
        End If
        Dim rootdicpath As String = Environment.CurrentDirectory
        Dim ffmpge As String = rootdicpath & "\main\corn\ffmpeg.exe"
        Dim silkdecode As String = rootdicpath & "\main\corn\silkdecode.exe" '解码
        Dim silkencode As String = rootdicpath & "\main\corn\silkencode.exe" '编码
        If File.Exists(ffmpge) AndAlso File.Exists(silkdecode) AndAlso File.Exists(silkencode) Then
            Dim name As String = audio_path.Substring(audio_path.LastIndexOf("\") + 1)
            Dim audioslik As String = rootdicpath & "\main\data\voice"
            If Not Directory.Exists(audioslik) Then
                Dim dic As DirectoryInfo = Directory.CreateDirectory(audioslik)
            End If
            Dim tempname As String = audioslik & "\" & name.Substring(0, name.LastIndexOf("."))
            Dim arg As String = $"-y -i ""{name}"" -f s16le -ar 16000 -ac 1 -acodec pcm_s16le ""{tempname}.pcm"""    'mp3转pcm  ffmpeg -y -i test.mp3 -f s16le -ar 16000 -ac 1 -acodec pcm_s16le pcm16k.pcm
            Runcmd(ffmpge, audioslik, arg)
            arg = $"""{tempname}.pcm"" ""{tempname}.silk"" -tencent"     'pcm转silk  'silkencode "name.pcm" "name.silk" -tencent 
            Runcmd(silkencode, audioslik, arg)
            Return GetByte($"{tempname}.silk")
        End If
        Return Nothing
    End Function
    ''' <summary>
    ''' 将文件转换为byte[]
    ''' </summary>
    ''' <param name="filepath"></param>
    ''' <returns></returns>
    Private Shared Function GetByte(ByVal filepath As String) As Byte()
        Using fs As New FileStream(filepath, FileMode.Open, FileAccess.Read)
            Dim by(fs.Length - 1) As Byte
            fs.Read(by, 0, CInt(fs.Length))
            fs.Close()
            Return by
        End Using
    End Function
    ''' <summary>
    ''' 调用外部程序
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <param name="arg"></param>
    Private Shared Sub Runcmd(ByVal filename As String, WorkingDirectory As String, ByVal arg As String)
        Using p1 As New Process
            p1.StartInfo.CreateNoWindow = True
            'p1.StartInfo.Verb = "runas"
            p1.StartInfo.WorkingDirectory = WorkingDirectory
            p1.StartInfo.FileName = filename
            p1.StartInfo.Arguments = arg
            p1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            p1.StartInfo.UseShellExecute = False
            p1.StartInfo.RedirectStandardOutput = True
            p1.Start()
            p1.WaitForExit()
            Dim output As String
            Using streamreader As System.IO.StreamReader = p1.StandardOutput
                output = streamreader.ReadToEnd()
                Debug.Print(output)
            End Using
        End Using
    End Sub
End Class

