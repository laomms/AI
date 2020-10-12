Imports System.IO

Public Class BaiduAPI
    Public Shared Function BaiduOCR(imagePath As String) As String
        Dim client = New Baidu.Aip.Ocr.Ocr(Baidu_APPKey, Baidu_SecretKey)
        client.Timeout = 60000 ' 修改超时时间
        Dim image = File.ReadAllBytes(imagePath)
        Dim url = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1564654456007&di=7832dd6f515e654bdf5074e47b6803b1&imgtype=0&src=http%3A%2F%2Fpic.962.net%2Fup%2F2018-5%2F2018527102938219310.jpg"
        Dim result = client.GeneralBasic(image)
        Console.WriteLine(result)
        Return result
    End Function
    Public Shared Function RegisterOCR(imagePath As String) As String '注册人脸
        Dim client = New Baidu.Aip.Face.Face(Baidu_APPKey, Baidu_SecretKey)
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
        Console.WriteLine(result)
        Return result
    End Function
    Public Shared Function FaceOCR(imagePath As String) As String '识别人脸
        Dim client = New Baidu.Aip.Face.Face(Baidu_APPKey, Baidu_SecretKey)
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
    End Function
    Public Shared Function VoiceSpeech(data() As Byte) As String '语音识别
        Dim client = New Baidu.Aip.Speech.Asr(Baidu_APPID, Baidu_APPKey, Baidu_SecretKey)
        client.Timeout = 60000 ' 修改超时时间
        'Dim data = File.ReadAllBytes("E:\Work Demo\语音技术\Voice\Voice\Image\16k.wav")
        ' 可选参数
        Dim options = New Dictionary(Of String, Object) From {
                {"dev_pid", 1536}
            }
        client.Timeout = 120000 ' 若语音较长，建议设置更大的超时时间. ms
        Dim result = client.Recognize(data, "wav", 16000, options)
        Console.Write(result)
        Return result
    End Function
    Public Shared Sub TtsSpeech(filepath As String) '语音合成
        Dim _ttsClient = New Baidu.Aip.Speech.Tts(Baidu_APPKey, Baidu_SecretKey)
        _ttsClient.Timeout = 60000
        Dim options = New Dictionary(Of String, Object) From
        {
         {"spd", 5},
         {"vol", 7},
         {"per", 4}
        }
        Dim result = _ttsClient.Synthesis("听说关注博主不迷路", options)
        If result.ErrorCode = 0 Then ' 或 result.Success
            'File.WriteAllBytes("E:\Work Demo\语音技术\Voice\Voice\Image\aaa.mp3", result.Data)
            File.WriteAllBytes(filepath, result.Data)
        End If
    End Sub

End Class
