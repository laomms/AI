﻿Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class BaiduAPI
    Public Shared Function BaiduOCR(imageUrl As String) As String
        Dim client = New Baidu.Aip.Ocr.Ocr(Baidu_APPKEY, Baidu_SecretKey)
        client.Timeout = 60000 ' 修改超时时间
        'Dim image = File.ReadAllBytes(imageUrl)
        'Dim result = client.GeneralBasic(image)
        ' 如果有可选参数
        'Dim options = New Dictionary(Of String, Object) From {
        '            {"language_type", "CHN_ENG"},
        '            {"detect_direction", "true"},
        '            {"detect_language", "true"},
        '            {"probability", "true"}
        '        }
        ' 带参数调用通用文字识别, 图片参数为本地图片
        'Dim result = client.GeneralBasic(image, options)
        ' 调用通用文字识别, 图片参数为本地图片， 可能会抛出网络等异常， 请使用try / catch捕获 用户向服务请求识别某张图中的所有文字 var result = client.GeneralBasic(image) '本地图图片
        Dim result = client.GeneralBasicUrl(imageUrl) '网络图片
        '      Dim result = client.Accurate(image) '本地图片：相对于通用文字识别该产品精度更高，但是识别耗时会稍长。

        '      Dim result = client.General(image) '本地图片：通用文字识别（含位置信息版）
        '      Dim result = client.GeneralUrl(url) '网络图片：通用文字识别（含位置信息版）

        '      Dim result = client.GeneralEnhanced(image) '本地图片：调用通用文字识别（含生僻字版）
        '      Dim result = client.GeneralEnhancedUrl(url) '网络图片：调用通用文字识别（含生僻字版）

        '      Dim result = client.WebImage(image) '本地图片:用户向服务请求识别一些背景复杂，特殊字体的文字。
        '      Dim result = client.WebImageUrl(url) '网络图片:用户向服务请求识别一些背景复杂，特殊字体的文字。
        Dim szResult As String = ""
        Try
            Dim n = CInt(result("words_result_num"))
            For i As Integer = 0 To n - 1
                szResult = szResult + vbNewLine + result("words_result")(i)("words").ToString
            Next
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
        Return szResult
    End Function

    Public Shared Function BaiduOCR2(imageUrl As String) As String
        Dim encoding As New UTF8Encoding
        Dim szUrl1 = "https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=" + Baidu_APPKEY + "&client_secret=" + Baidu_SecretKey
        Dim szToken = GetBaiduToken(szUrl1)
        Dim szUrl2 = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic?access_token=" & szToken
        '本地或者网络
        'Dim szBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(picpath))
        'Dim PostData As String = HttpUtility.UrlEncode(szBase64)
        'Dim Data As Byte() = encoding.GetBytes("image=" & PostData & "&language_type=CHN_ENG")
        Dim Data As Byte() = encoding.GetBytes("url=" & imageUrl)
        Dim httpWebRequest As HttpWebRequest = WebRequest.Create(szUrl2)
        httpWebRequest.Method = "POST"
        httpWebRequest.Timeout = 10000
        httpWebRequest.Accept = "*/*"
        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
        httpWebRequest.ServicePoint.Expect100Continue = True
        httpWebRequest.ProtocolVersion = New Version(1, 1)
        httpWebRequest.ContentLength = Data.Length
        Dim requestStream = httpWebRequest.GetRequestStream()
        requestStream.Write(Data, 0, Data.Length)
        requestStream.Close()
        Dim result As String = ""
        Dim szRes As String = ""
        Try
            Using myResponse As HttpWebResponse = httpWebRequest.GetResponse()
                If myResponse.ContentEncoding.ToLower().Contains("gzip") Then
                    Using stream As Stream = New System.IO.Compression.GZipStream(myResponse.GetResponseStream, IO.Compression.CompressionMode.Decompress)
                        Using reader As New StreamReader(stream)
                            result = reader.ReadToEnd()
                            reader.Close()
                        End Using
                        stream.Close()
                    End Using
                Else
                    Using stream As Stream = httpWebRequest.GetResponse().GetResponseStream()
                        Using reader As New StreamReader(stream)
                            result = reader.ReadToEnd()
                            reader.Close()
                        End Using
                        stream.Close()
                    End Using
                End If
            End Using
            Dim jsons As Object = New JavaScriptSerializer().Deserialize(Of Object)(result)
            Dim n = CInt(jsons("words_result_num"))
            For i As Integer = 0 To n - 1
                szRes = szRes + vbNewLine + jsons("words_result")(i)("words").ToString
            Next
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try

        Return szRes
    End Function
    Public Shared Function GetBaiduToken(url As String) As String
        Dim szToken As String = ""
        Dim szRequest As HttpWebRequest = WebRequest.Create(url)
        szRequest.Method = "POST"
        Try
            Using myResponse As HttpWebResponse = szRequest.GetResponse()
                Using stream As Stream = szRequest.GetResponse().GetResponseStream()
                    Using szReader As New StreamReader(stream)
                        Dim szResult = szReader.ReadToEnd()
                        Dim jsons As Object = New JavaScriptSerializer().DeserializeObject(szResult)
                        szToken = jsons("access_token")
                        szReader.Close()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
        Return szToken
    End Function
    Public Shared Function BaiduTranslation(ByVal szText As String, languageFrom As String, languageTo As String) As String
        Dim appId As String = Baidu_APP_ID
        Dim password As String = Baidu_APP_KEY
        Dim jsonResult As String = String.Empty
        Dim randomNum As String = DateTime.UtcNow.Subtract(New DateTime(1970, 1, 1)).TotalMilliseconds.ToString.Substring(0, 5)
        Dim sourceMd5Byte() As Byte = Encoding.UTF8.GetBytes(appId & szText & randomNum & password)
        Dim md5 As MD5 = System.Security.Cryptography.MD5.Create()
        Dim destMd5Byte() As Byte = md5.ComputeHash(sourceMd5Byte)
        Dim md5Sign As String = BitConverter.ToString(destMd5Byte).Replace("-", "")
        md5Sign = md5Sign.ToLower()
        Dim szResult As String = ""
        Dim url As String = String.Format("http://api.fanyi.baidu.com/api/trans/vip/translate?q={0}&from={1}&to={2}&appid={3}&salt={4}&sign={5}", HttpUtility.UrlEncode(szText, Encoding.UTF8), languageFrom, languageTo, appId, randomNum, md5Sign)
        Dim wc As New WebClient()
        wc.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)")
        Try
            jsonResult = wc.DownloadString(url)
            Dim jsons As Object = New JavaScriptSerializer().DeserializeObject(jsonResult)
            If jsonResult.Contains("error_code") Then
                szResult = "调用失败: " + jsons("error_msg")
            Else
                For Each item As Dictionary(Of String, Object) In jsons("trans_result")
                    szResult = szResult + item.Values(1).ToString() + vbNewLine
                Next
            End If
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
        Return szResult
    End Function
    Public Shared Function RegisterOCR(imagePath As String) As String '注册人脸
        Try
            Dim client = New Baidu.Aip.Face.Face(Baidu_APPKEY, Baidu_SecretKey)
            client.Timeout = 60000 ' 修改超时时间
            '取决于image_type参数，传入BASE64字符串或URL字符串或FACE_TOKEN字符串
            '你共享的图片路径（点击路径可直接查看图片）
            Dim image = "https://thumbnail0.baidupcs.com/thumbnail/32f3cc8f022839a4dbf6b9f9cca76ce8?fid=3145591938-250528-218900036170682&time=1550282400&rt=sh&sign=FDTAER-DCb740ccc5511e5e8fedcff06b081203-sTBqvQbbBy3n5SDQfbtjNwjlSHg%3D&expires=8h&chkv=0&chkbd=0&chkpc=&dp-logid=1077356968076791248&dp-callid=0&size=c710_u400&quality=100&vuk=-&ft=video"
            Dim imageType = "URL"
            '注册人脸
            Dim groupId = "group1"
            Dim userId = "user1"
            ' 调用人脸注册，可能会抛出网络等异常，请使用try/catch捕获
            Dim result = client.UserAdd(image, imageType, groupId, userId)
            Console.WriteLine(result)
            ' 如果有可选参数
            Dim options = New Dictionary(Of String, Object) From {
                    {"user_info", "user's info"},
                    {"quality_control", "NORMAL"},
                    {"liveness_control", "LOW"}
                }
            ' 带参数调用人脸注册
            result = client.UserAdd(image, imageType, groupId, userId, options)
            Return result
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try

    End Function
    Public Shared Function FaceOCR(imagePath As String) As String '识别人脸
        Try
            Dim client = New Baidu.Aip.Face.Face(Baidu_APPKEY, Baidu_SecretKey)
            client.Timeout = 60000 ' 修改超时时间
            '取决于image_type参数，传入BASE64字符串或URL字符串或FACE_TOKEN字符串
            '你共享的图片路径（点击路径可直接查看图片）
            Dim image = "https://thumbnail0.baidupcs.com/thumbnail/32f3cc8f022839a4dbf6b9f9cca76ce8?fid=3145591938-250528-218900036170682&time=1550282400&rt=sh&sign=FDTAER-DCb740ccc5511e5e8fedcff06b081203-sTBqvQbbBy3n5SDQfbtjNwjlSHg%3D&expires=8h&chkv=0&chkbd=0&chkpc=&dp-logid=1077356968076791248&dp-callid=0&size=c710_u400&quality=100&vuk=-&ft=video"
            Dim imageType = "URL"

            '人脸识别（在注册的人脸库里面进行识别）
            '调用人脸检测，可能会抛出网络等异常，请使用try / catch捕获
            Dim result = client.Detect(image, imageType)
            Console.WriteLine(result)
            ' 如果有可选参数
            Dim options = New Dictionary(Of String, Object) From {
                    {"face_field", "age"},
                    {"max_face_num", 2},
                    {"face_type", "LIVE"}
                }
            ' 带参数调用人脸检测
            result = client.Detect(image, imageType, options)
            Console.WriteLine(result)
            Return result
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try

    End Function
    Public Shared Function VoiceSpeech(data() As Byte) As String '语音识别,第一次使用记得领取免费试用额
        Try
            Dim client = New Baidu.Aip.Speech.Asr(Baidu_APPID, Baidu_APPKEY, Baidu_SecretKey)
            client.Timeout = 60000 ' 修改超时时间
            'Dim data = File.ReadAllBytes("E:\Work Demo\语音技术\Voice\Voice\Image\16k.wav")
            ' 可选参数
            Dim options = New Dictionary(Of String, Object) From {
                    {"dev_pid", 1537}
                }
            client.Timeout = 120000 ' 若语音较长，建议设置更大的超时时间. ms
            Dim res = client.Recognize(data, "pcm", 16000, options)
            Return res("result")(0)
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try

    End Function
    Public Shared Function TtsSpeech(szContent As String) As String '语音合成,第一次使用记得领取免费试用额
        Try
            Dim _ttsClient = New Baidu.Aip.Speech.Tts(Baidu_APPKEY, Baidu_SecretKey)
            _ttsClient.Timeout = 60000
            Dim options = New Dictionary(Of String, Object) From
            {
             {"spd", 2},
             {"vol", 7},
             {"per", 4}
            }
            Dim folderpath As String = Environment.CurrentDirectory & "\main\data\voice"
            If Not Directory.Exists(folderpath) Then
                Dim dic As DirectoryInfo = Directory.CreateDirectory(folderpath)
            End If
            Dim result = _ttsClient.Synthesis(szContent, options)
            If result.ErrorCode = 0 Then
                If File.Exists(folderpath + "\saved.mp3") Then
                    File.Delete(folderpath + "\saved.mp3")
                End If
                File.WriteAllBytes(folderpath + "\saved.mp3", result.Data)
                Return folderpath + "\saved.mp3"
            Else
                Return "调用失败: " + result.ErrorMsg.ToString
            End If
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
    End Function
End Class
