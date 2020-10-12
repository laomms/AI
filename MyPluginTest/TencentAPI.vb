Imports TencentCloud.Common
Imports TencentCloud.Common.Profile
Imports TencentCloud.Ocr.V20181119
Imports TencentCloud.Ocr.V20181119.Models
Imports System.Runtime.InteropServices
Imports System.Globalization
Imports TencentCloud.Aai.V20180522
Imports TencentCloud.Aai.V20180522.Models
Imports Newtonsoft.Json.Linq

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
        Catch e As Exception
            Console.WriteLine(e.ToString())
        End Try
        Return ""
    End Function
End Class
