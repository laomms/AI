Imports TencentCloud.Common
Imports TencentCloud.Common.Profile
Imports TencentCloud.Ocr.V20181119
Imports TencentCloud.Aai.V20180522
Imports TencentCloud.Aai.V20180522.Models
Imports TencentCloud.Cvm.V20170312
Imports TencentCloud.Cvm.V20170312.Models
Imports TencentCloud.Iai.V20180301
Imports TencentCloud.Iai.V20180301.Models
Imports TencentCloud.Tmt.V20180321
Imports TencentCloud.Tmt.V20180321.Models
Imports TencentCloud.Tcr.V20190924
Imports TencentCloud.Sms.V20190711
Imports TencentCloud.Cms.V20190321.Models
Imports TencentCloud.Ocr.V20181119.Models
Imports TencentCloud.Cim.V20190318
Imports TencentCloud.Tiia.V20190529
Imports TencentCloud.Tiia.V20190529.Models
Imports TencentCloud.Sms.V20190711.Models
Imports TencentCloud.Ticm.V20181127
Imports TencentCloud.Ticm.V20181127.Models
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Web
Imports System.IO
Imports System.Windows.Forms
Imports System.Text
Imports System.Security.Cryptography
Imports System.Net
Imports System.Web.Script.Serialization

Public Class TencentAPI
    Public Shared Function TecentChat2(szContents As String) As String
        Try
            Dim cred As Credential = New Credential With {
                    .SecretId = Tencent_SecretId,
                    .SecretKey = Tencent_SecretKey
                }
            Dim clientProfile As New ClientProfile()
            Dim httpProfile As New HttpProfile()
            httpProfile.Endpoint = "aai.tencentcloudapi.com"
            clientProfile.HttpProfile = httpProfile

            Dim client As New AaiClient(cred, "ap-hongkong", clientProfile)
            Dim req As New ChatRequest()
            Dim strParams As String = "{""Text"":""" & szContents & """,""ProjectId"":0}"
            req = ChatRequest.FromJsonString(Of ChatRequest)(strParams)
            Dim resp As ChatResponse = client.ChatSync(req)
            Console.WriteLine(AbstractModel.ToJsonString(resp))
            Dim jsons As JObject = JObject.Parse(AbstractModel.ToJsonString(resp))

            Return jsons.SelectToken("Response").SelectToken("Answer").ToString
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
        Return ""
    End Function
    Public Shared Function TecentChat1(szContents As String) As String
        Dim params As New List(Of String)
        params.Add("app_id:" + Tencent_APP_ID)
        params.Add("session:10000")
        params.Add("question:" + szContents)
        params.Add("time_stamp:" + CLng((DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString)
        params.Add("nonce_str:" + New Random(System.Environment.TickCount).Next(10000000, 99999999).ToString)
        Dim sign As String = getReqSign(params, Tencent_APP_KEY)
        params.Add("sign:" + sign)
        Dim str As String = ""
        For Each item As String In params
            If item <> "" Then
                str &= item.Split(":")(0) & "=" & UCase(HttpUtility.UrlEncode(item.Split(":")(1))) & "&"
            End If
        Next
        Dim url = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_textchat"
        Dim szRes As String = ""
        Using wc = New WebClient()
            wc.Encoding = Encoding.UTF8
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded")
            wc.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8")
            szRes = wc.UploadString(url, str)
            Try
                Dim jsons = New JavaScriptSerializer().DeserializeObject(szRes)
                If jsons("msg").ToString <> "ok" Then
                    szRes = "调用接口失败: " + jsons("msg").ToString
                Else
                    szRes = jsons("data")("answer").ToString
                End If

            Catch ex As Exception
                If Not ex.InnerException Is Nothing Then
                    Return "调用失败: " + ex.GetBaseException.Message.ToString
                Else
                    Return "调用失败: " + ex.Message.ToString
                End If
            End Try

        End Using
        Return szRes
    End Function
    Private Shared Function getReqSign(params As List(Of String), appkey As String) As String
        params.Sort()
        Dim str As String = ""
        For Each item As String In params
            If item <> "" Then
                str += item.Split(":")(0) & "=" & UCase(HttpUtility.UrlEncode(item.Split(":")(1))) + "&"
            End If
        Next
        str = str & "app_key=" & appkey
        Dim sBuilder As New StringBuilder()
        Using hasher As MD5 = MD5.Create()
            Dim dbytes As Byte() = hasher.ComputeHash(Encoding.UTF8.GetBytes(str))
            For n As Integer = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("X2"))
            Next n
        End Using
        Return UCase(sBuilder.ToString)
    End Function
    Public Shared Function GetZones() As String
        Try
            Dim cred As New Credential With {
                    .SecretId = Tencent_SecretId,
                    .SecretKey = Tencent_SecretKey
                }

            ' 实例化一个client, 不必要的选项都屏蔽
            Dim clientProfile As New ClientProfile()
            Dim httpProfile As New HttpProfile()
            httpProfile.WebProxy = Environment.GetEnvironmentVariable("HTTPS_PROXY")
            clientProfile.HttpProfile = httpProfile
            Dim client As New CvmClient(cred, "ap-guangzhou", clientProfile)
            Dim req As New DescribeZonesRequest()
            Dim resp As DescribeZonesResponse = client.DescribeZonesSync(req)
            Return AbstractModel.ToJsonString(resp)
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
    End Function
    Public Shared Function VoiceRecognition(strFilepath As String) As String
        Try
            Dim cred As New Credential With {
                .SecretId = Tencent_SecretId,
                .SecretKey = Tencent_SecretKey
            }
            Dim clientProfile As New ClientProfile()
            Dim httpProfile As New HttpProfile()
            clientProfile.HttpProfile = httpProfile
            Dim client As New AaiClient(cred, "ap-guangzhou", clientProfile)

            '开始语音mp3转文字, 成对使用 SentenceRecognitionRequest/ SentenceRecognitionResponse
            Dim req As New SentenceRecognitionRequest()
            Dim fs As New FileStream(strFilepath, FileMode.Open)
            Dim raw(fs.Length - 1) As Byte
            fs.Read(raw, 0, raw.Length)
            Dim base64data As String = Convert.ToBase64String(raw) 'mp3文件的数据用Base64编码
            fs.Close()
            ' 语音识别request的设置。
            req.ProjectId = 0
            req.SubServiceType = 2
            '必须8或16k否则返回不接受,但实际文件32k/64k也能接受,但会识别错误
            req.EngSerViceType = "16k"
            req.SourceType = 1
            req.VoiceFormat = "mp3"
            '随便输,系统其实没返回这个ID,和文档不一样
            req.UsrAudioKey = "my_usrid_5c79510a12da-whatever"
            'base64编码数据设置为该Request的Data    
            req.Data = base64data
            req.DataLen = base64data.Length
            ' 异步提交语言识别Request
            Dim resp As SentenceRecognitionResponse = client.SentenceRecognitionSync(req)
            ' 输出json格式的字符串回包
            Return AbstractModel.ToJsonString(resp)
        Catch e As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
    End Function
    Public Shared Function TestToVoice(szText As String) As Byte()
        Try
            Dim cred As New Credential With {
                .SecretId = Tencent_SecretId,
                .SecretKey = Tencent_SecretKey
            }
            Dim aaiClient As New AaiClient(cred, "ap-beijing")
            Dim req As New TextToVoiceRequest()
            req.ProjectId = 10144947
            req.ModelType = 1
            req.PrimaryLanguage = 1
            req.SampleRate = 5
            req.SessionId = "testsessionid"
            req.Speed = 1.0F
            req.Text = szText
            req.VoiceType = 1
            req.Volume = 1.0F
            Dim textToVoiceResponse As TextToVoiceResponse = aaiClient.TextToVoiceSync(req)
            Dim audio As String = textToVoiceResponse.Audio()
            If Not audio Is String.Empty Then
                Return System.Convert.FromBase64String(audio)
            End If
        Catch ex As Exception

        End Try
        Return Nothing
    End Function
    Public Shared Function TextDetect(szText As String) As String
        Try
            Dim cred As New Credential With {
              .SecretId = Tencent_SecretId,
              .SecretKey = Tencent_SecretKey
          }
            Dim httpProfile As New HttpProfile()
            httpProfile.Endpoint = "ocr.tencentcloudapi.com"

            Dim clientProfile As New ClientProfile()
            clientProfile.SignMethod = "TC3-HMAC-SHA256"
            clientProfile.HttpProfile = httpProfile

            Dim client As New TmtClient(cred, "ap-guangzhou", clientProfile)
            Dim req As New Models.LanguageDetectRequest
            req.Text = szText
            Dim resp = client.LanguageDetectSync(req)
            Return resp.ToString
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try

        Return Nothing
    End Function
    Private Function GetFaceMarks(txtPicUrl As String, ByVal strFullPathPicFileName As String) As String
        Try
            Dim cred As New Credential With {
                    .SecretId = Tencent_SecretId,
                .SecretKey = Tencent_SecretKey
                }
            Dim clientProfile As New ClientProfile()
            Dim httpProfile As New HttpProfile()
            clientProfile.HttpProfile = httpProfile
            Dim client As New IaiClient(cred, "ap-guangzhou", clientProfile)

            '这里开始 anlyzeFace ,五官定位, 成对使用 AnalyzeFaceRequest/ AnalyzeFaceResponse
            Dim faceReq As New AnalyzeFaceRequest()
            If txtPicUrl.Length > 0 Then ' 若faceReq同时有url和Image, 腾讯云优先用url.
                faceReq.Url = txtPicUrl
            Else
                If strFullPathPicFileName.Length > 0 Then ' 用filestream 读取本地图片文件
                    Dim strFilepath As String = strFullPathPicFileName
                    Dim fs As New FileStream(strFilepath, FileMode.Open)
                    Dim raw(fs.Length - 1) As Byte
                    fs.Read(raw, 0, raw.Length)
                    Dim base64data As String = Convert.ToBase64String(raw) '图片文件的数据用Base64编码
                    fs.Close()
                    faceReq.Image = base64data ' base64编码后的字符串设置为Request的Image
                Else
                    MessageBox.Show("请选择一个本地图片或在下框输入图片网址")
                    Return ""
                End If
            End If
            Dim resp As AnalyzeFaceResponse = client.AnalyzeFaceSync(faceReq)
            Return AbstractModel.ToJsonString(resp)
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
    End Function
    Public Shared Function translate(szText As String) As String
        Try
            Dim cred As New Credential With {
                    .SecretId = Tencent_SecretId,
                .SecretKey = Tencent_SecretKey
                }
            Dim httpProfile As New HttpProfile()
            httpProfile.ReqMethod = "GET"
            httpProfile.Endpoint = "tmt.tencentcloudapi.com"
            Dim clientProfile As New ClientProfile()
            clientProfile.SignMethod = "HmacSHA1"
            clientProfile.HttpProfile = httpProfile
            Dim client As New TmtClient(cred, "ap-guangzhou", clientProfile)
            Dim req As New TextTranslateRequest()
            req.ProjectId = 0
            req.Source = "en"
            req.SourceText = szText
            req.Target = "zh"
            Dim resp = client.TextTranslateSync(req)
            Return AbstractModel.ToJsonString(resp)
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
    End Function
    Public Shared Function TencentOcr(img_url As String) As String
        Try
            Dim cred As New Credential With {
               .SecretId = Tencent_SecretId,
               .SecretKey = Tencent_SecretKey
               }
            Dim httpProfile As New HttpProfile()
            httpProfile.Endpoint = "ocr.tencentcloudapi.com"

            Dim clientProfile As New ClientProfile()
            clientProfile.SignMethod = "TC3-HMAC-SHA256"

            clientProfile.HttpProfile = httpProfile
            Dim client As New OcrClient(cred, "ap-guangzhou", clientProfile)
            Dim req As New Models.GeneralBasicOCRRequest()
            req.ImageUrl = img_url
            Dim resp = client.GeneralBasicOCRSync(req)
            Dim jsons As JObject = JObject.Parse(AbstractModel.ToJsonString(resp))
            Dim szResult As String = ""
            Dim count = jsons("TextDetections").Count
            For i = 0 To count - 1
                szResult = szResult + vbNewLine + jsons("TextDetections")(i)("DetectedText").ToString
            Next
            Return szResult
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
    End Function
    Public Shared Function ImageDetect(img_url As String) As String
        Try
            Dim cred As New Credential With {
              .SecretId = Tencent_SecretId,
              .SecretKey = Tencent_SecretKey
              }
            Dim clientProfile As New ClientProfile()
            Dim httpProfile As New HttpProfile()
            httpProfile.Endpoint = ("tiia.tencentcloudapi.com")
            clientProfile.HttpProfile = httpProfile

            Dim client As New TiiaClient(cred, "ap-guangzhou", clientProfile)
            Dim req As New DetectMisbehaviorRequest()
            req.ImageUrl = img_url
            Dim resp As DetectMisbehaviorResponse = client.DetectMisbehaviorSync(req)
            Return AbstractModel.ToJsonString(resp)
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try



    End Function
    Public Shared Function ImagePornDetect(imageUrl As String) As String
        Try
            Dim cred As New Credential With {
               .SecretId = Tencent_SecretId,
               .SecretKey = Tencent_SecretKey
               }
            Dim clientProfile As New ClientProfile()
            Dim httpProfile As New HttpProfile()
            httpProfile.Endpoint = ("ticm.tencentcloudapi.com")
            clientProfile.HttpProfile = httpProfile

            Dim client As New TicmClient(cred, "ap-guangzhou", clientProfile)
            Dim req As New TencentCloud.Ticm.V20181127.Models.ImageModerationRequest()
            req.ImageUrl = imageUrl
            Dim resp As TencentCloud.Ticm.V20181127.Models.ImageModerationResponse = client.ImageModerationSync(req)
            Return AbstractModel.ToJsonString(resp)
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try

    End Function

    'Dim aaa() As String = New String() {"+8613711112222", "+8613711112222"}
    'SendMessageAsync(“短信发送测试！”, aaa)
    Sub SendMessageAsync(ByVal sContent As String, ByVal sPhone As String())
        Try
            '* CAM 密匙查询：https :       //console.cloud.tencent.com/cam/capi*/
            Dim cred As New Credential With {
                .SecretId = Tencent_SecretId,
                .SecretKey = Tencent_SecretKey
                }
            Dim ClientProfile As New ClientProfile()
            ClientProfile.SignMethod = ClientProfile.SIGN_TC3SHA256
            Dim HttpProfile As New HttpProfile()
            HttpProfile.ReqMethod = "POST"
            HttpProfile.Timeout = 10
            HttpProfile.Endpoint = "sms.tencentcloudapi.com"
            HttpProfile.WebProxy = Environment.GetEnvironmentVariable("HTTPS_PROXY")
            ClientProfile.HttpProfile = HttpProfile
            Dim client As SmsClient = New SmsClient(cred, "ap-guangzhou", ClientProfile)
            Dim req As New SendSmsRequest()
            req.Sign = "环球科技"
            req.SmsSdkAppid = "…"   'req.SmsSdkAppid = "1257310899"
            req.PhoneNumberSet = sPhone
            req.TemplateID = "644040" ' 模板参数: 若无模板参数， 则设置为空
            req.TemplateParamSet = New String() {sContent}
            Dim resp As SendSmsResponse = client.SendSmsSync(req)
            Dim sA As String = AbstractModel.ToJsonString(resp)
            If InStr(sA, "send success", CompareMethod.Text) > 0 Then
                MessageBox.Show("短信发送成功。")
            Else
                MessageBox.Show("短信发送失败。错误信息：" & sA)
            End If
        Catch e As Exception
            MessageBox.Show("短信发送失败！")
        End Try
    End Sub
    Private Sub IDCardVerificationBySDK(ByVal context As HttpContext)
        Dim imgStr As String = context.Request("ImageBase64")
        Try
            If Not String.IsNullOrEmpty(imgStr) Then
                Dim res As String = String.Empty
                Dim action As Action(Of String) = Sub(t)
                                                      res = GetOCRMsg(imgStr)
                                                  End Sub
                Dim asyncResult As IAsyncResult = action.BeginInvoke("调用腾讯云身份证识别", Nothing, Nothing)
                asyncResult.AsyncWaitHandle.WaitOne()
                If res.Contains("message") Then
                    context.Response.Write("{""Status"":""fail"",""errorMsg"":""" & res.Split(New String() {"message:"}, StringSplitOptions.None)(1) & """}")
                Else
                    Dim resp As IDCardOCRResponse = JsonConvert.DeserializeObject(Of IDCardOCRResponse)(res)
                    Dim result = New With {
                        Key .Status = "success",
                        Key .data = resp
                    }
                    context.Response.Write(JsonConvert.SerializeObject(result))
                End If
            Else
                context.Response.Write("{""Status"":""fail"",""errorMsg"":""请选择上传的图片！""}")
            End If
        Catch ex As Exception
            Debug.Print(ex.ToString())
        End Try
    End Sub
    Private Function GetOCRMsg(ByVal imgStr As String) As String
        Try
            Dim cred As New Credential With {
                .SecretId = Tencent_SecretId,
                .SecretKey = Tencent_SecretKey
            }
            Dim clientProfile As New ClientProfile With {.SignMethod = ClientProfile.SIGN_TC3SHA256}
            Dim httpProfile As New HttpProfile()
            httpProfile.Endpoint = "ocr.tencentcloudapi.com"
            httpProfile.ReqMethod = "POST"
            httpProfile.Timeout = 10 ' 请求连接超时时间，单位为秒(默认60秒)
            clientProfile.HttpProfile = httpProfile
            Dim client As New OcrClient(cred, "ap-guangzhou", clientProfile)
            Dim req As New IDCardOCRRequest()
            Dim strParams As String = "{""ImageBase64"":""" & imgStr & """,""CardSide"":""FRONT"",""ImageUrl"":"""",""Config"":""""}"
            req = JsonConvert.DeserializeObject(Of IDCardOCRRequest)(strParams)
            Dim resp As IDCardOCRResponse = client.IDCardOCR(req).ConfigureAwait(False).GetAwaiter().GetResult()
            Return AbstractModel.ToJsonString(resp)
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Return "调用失败: " + ex.GetBaseException.Message.ToString
            Else
                Return "调用失败: " + ex.Message.ToString
            End If
        End Try
    End Function

End Class
