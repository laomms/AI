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
            If sMsg.MessageContent.Contains("[Audio,hash=") Then
                Dim match = New Regex("(?<=url=).*?(?=,type=0)").Match(sMsg.MessageContent)
                If match.Success Then
                    API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "语音地址地址:" + vbNewLine + match.Value, False)
                End If
            ElseIf sMsg.MessageContent.Contains("[pic,hash=") Then
                Dim matches As MatchCollection = Regex.Matches(sMsg.MessageContent, "\[pic,hash.*?\]", RegexOptions.Multiline Or RegexOptions.IgnoreCase)
                For Each match As Match In matches
                    Dim imageUrl As String = Marshal.PtrToStringAnsi(API.GetImageDownloadLink(plugin_key, match.Value, sMsg.ThisQQ, sMsg.MessageGroupQQ))
                    API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "图片地址:" + vbNewLine + imageUrl, False)
                Next
            End If
        End If
        Return 0
    End Function


#End Region

End Module
