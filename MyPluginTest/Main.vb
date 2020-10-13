Imports System.IO
Imports System.Net
Imports System.Numerics
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Web.Script.Serialization


Module Main
    Private TempDic As New Dictionary(Of Long, Long)

#Region "收到私聊消息"
    Public funRecvicePrivateMsg As RecvicePrivateMsg = New RecvicePrivateMsg(AddressOf RecvicetPrivateMessage)
    <UnmanagedFunctionPointer(CallingConvention.StdCall)>
    Public Delegate Function RecvicePrivateMsg(ByRef sMsg As PrivateMessageEvent) As Integer
    Public Function RecvicetPrivateMessage(ByRef sMsg As PrivateMessageEvent) As Integer
        Dim MessageRandom As New Long
        Dim MessageReq As New UInteger
        If sMsg.SenderQQ <> sMsg.ThisQQ Then

        End If
        Return 0
    End Function
#End Region

#Region "收到群聊消息"
    Public funRecviceGroupMsg As RecviceGroupMsg = New RecviceGroupMsg(AddressOf RecvicetGroupMessage)
    <UnmanagedFunctionPointer(CallingConvention.StdCall)>
    Public Delegate Function RecviceGroupMsg(ByRef sMsg As GroupMessageEvent) As Integer
    Public Function RecvicetGroupMessage(ByRef sMsg As GroupMessageEvent) As Integer

        If sMsg.SenderQQ <> sMsg.ThisQQ Then
            If TempDic.ContainsKey(sMsg.SenderQQ) AndAlso TempDic.ContainsValue(sMsg.MessageGroupQQ) Then
                TempDic.Remove(sMsg.SenderQQ)
                'Dim filepath As String = BaiduAPI.TtsSpeech(sMsg.MessageContent)
                Dim filepath As String = Environment.CurrentDirectory + "\main\data\voice\saved.mp3"
                If filepath <> "" AndAlso File.Exists(filepath) Then
                    Dim audiobyte As Byte() = SilkHelp.SilkEncoding(filepath)
                    Dim ret = API.UploadGroupAudio(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, 0, "", audiobyte, audiobyte.Length)
                End If
            ElseIf sMsg.MessageContent.Contains("[Audio,hash=") Then
                Dim match = New Regex("(?<=url=).*?(?=,type=0)").Match(sMsg.MessageContent)
                If match.Success Then
                    Dim audioByte() As Byte = SilkHelp.UrlSilkDecoding(match.Value) ' SilkHelp.GetByteFromAmr(match.Value) '
                    Dim ret = BaiduAPI.VoiceSpeech(audioByte)
                    API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "语音内容: " + ret, False)
                End If
            ElseIf sMsg.MessageContent.Contains("[pic,hash=") Then
                Dim matches As MatchCollection = Regex.Matches(sMsg.MessageContent, "\[pic,hash.*?\]", RegexOptions.Multiline Or RegexOptions.IgnoreCase)
                For Each match As Match In matches
                    Dim imageUrl As String = Marshal.PtrToStringAnsi(API.GetImageDownloadLink(plugin_key, match.Value, sMsg.ThisQQ, sMsg.MessageGroupQQ))
                    'Dim ret = BaiduAPI.BaiduOCR(imageUrl)
                    Dim ret = TencentAPI.TencentOcr(imageUrl)
                    API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "图片内容: " + vbNewLine + ret + vbNewLine, False)
                Next
            ElseIf sMsg.MessageContent.Contains("翻译英文") Then
                Dim szText = sMsg.MessageContent.Replace("翻译英文", "").Trim
                Dim ret = BaiduAPI.BaiduTranslation(szText, "auto", "en")
                API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "英文翻译文本: " + vbNewLine + ret, False)
            ElseIf sMsg.MessageContent.Contains("翻译中文") Then
                Dim szText = sMsg.MessageContent.Replace("翻译中文", "").Trim
                Dim ret = BaiduAPI.BaiduTranslation(szText, "auto", "zh")
                API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "中文翻译文本: " + vbNewLine + ret, False)
            ElseIf sMsg.MessageContent = "文字转语音" Then
                API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "]" + vbNewLine + "请输入要转语音的文本 ", False)
                    TempDic.Add(sMsg.SenderQQ, sMsg.MessageGroupQQ)
                End If
            End If
            Return 0
    End Function


#End Region

End Module
