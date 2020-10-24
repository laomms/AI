Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Text.RegularExpressions

Public Class HttpHelper
    ''' <summary>
    ''' Http请求
    ''' </summary>
    ''' <param name="url">请求网址</param>
    ''' <param name="Headerdics">头文件固定KEY值字典类型泛型集合</param>
    ''' <param name="heard">头文件集合</param>
    ''' <param name="cookieContainers">cookie容器</param>
    ''' <param name="redirecturl">头文件中的跳转链接</param>
    ''' <returns>返回请求字符串结果</returns>
    Public Shared Function RequestGet(ByVal url As String, Headerdics As Dictionary(Of String, String), ByVal heard As WebHeaderCollection, ByRef cookieContainers As CookieContainer, ByRef redirecturl As String) As String
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function

        'Dim domain = CStr(Regex.Match(url, "^(?:\w+://)?([^/?]*)").Groups(1).Value)

        'If domain.Contains("www.") = True Then
        '    domain = domain.Replace("www.", "")
        'Else
        '    domain = "." & domain
        'End If
        If url = "" Then Return ""
        Dim myRequest As HttpWebRequest = WebRequest.Create(url)
        myRequest.Headers = heard
        myRequest.Method = "GET"
        For Each pair In Headerdics
            GetType(HttpWebRequest).GetProperty(pair.Key).SetValue(myRequest, pair.Value, Nothing)
        Next
        myRequest.CookieContainer = cookieContainers
        Dim results As String = ""

        Try
            Using myResponse As HttpWebResponse = myRequest.GetResponse()
                If myResponse.ContentEncoding.ToLower().Contains("gzip") Then
                    Using stream As Stream = New System.IO.Compression.GZipStream(myResponse.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress)
                        Using reader = New StreamReader(stream)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                ElseIf myResponse.ContentEncoding.ToLower().Contains("deflate") Then
                    Using stream As Stream = New System.IO.Compression.DeflateStream(myResponse.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress)
                        Using reader = New StreamReader(stream)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                Else
                    Using stream As Stream = myResponse.GetResponseStream()
                        Using reader = New StreamReader(stream, Encoding.UTF8)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                End If
                If myResponse.Headers("Location") IsNot Nothing Then
                    redirecturl = myResponse.Headers("Location")
                End If
            End Using
        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        results = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try
        redirecturl = redirecturl
        Return results
    End Function
    ''' <summary>
    ''' Http响应
    ''' </summary>
    ''' <param name="url">请求网址</param>
    ''' <param name="Headerdics">头文件固定KEY值字典类型泛型集合</param>
    ''' <param name="heard">头文件集合</param>
    ''' <param name="postdata">提交的字符串型数据</param>
    ''' <param name="cookieContainers">cookie容器</param>
    ''' <param name="redirecturl">头文件中的跳转链接</param>
    ''' <returns>返回响应字符串结果</returns>
    Public Shared Function RequestPost(ByVal url As String, Headerdics As Dictionary(Of String, String), ByVal heard As WebHeaderCollection, ByVal postdata As String, ByRef cookieContainers As CookieContainer, ByRef ResponseHeaders As WebHeaderCollection, ByRef redirecturl As String) As String
        If url = "" Then Return ""
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function

        'Dim domain = CStr(Regex.Match(url, "^(?:\w+://)?([^/?]*)").Groups(1).Value)

        'If domain.Contains("www.") = True Then
        '    domain = domain.Replace("www.", "")
        'Else
        '    domain = domain
        'End If
        Dim results As String = ""
        Try

            Dim myRequest = CType(WebRequest.Create(url), HttpWebRequest)
            Dim data = Encoding.UTF8.GetBytes(postdata)
            myRequest.Headers = heard
            myRequest.Method = "POST"
            For Each pair In Headerdics
                GetType(HttpWebRequest).GetProperty(pair.Key).SetValue(myRequest, pair.Value, Nothing)
            Next
            myRequest.CookieContainer = cookieContainers
            myRequest.ContentLength = data.Length
            Using stream = myRequest.GetRequestStream()
                stream.Write(data, 0, data.Length)
            End Using

            Using myResponse As HttpWebResponse = myRequest.GetResponse()
                If myResponse.ContentEncoding.ToLower().Contains("gzip") Then
                    Using stream = myResponse.GetResponseStream()
                        Using reader As New StreamReader(New System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress), Encoding.UTF8)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                ElseIf myResponse.ContentEncoding.ToLower().Contains("deflate") Then
                    Using stream = myResponse.GetResponseStream()
                        Using reader As New StreamReader(New System.IO.Compression.DeflateStream(stream, System.IO.Compression.CompressionMode.Decompress), Encoding.UTF8)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                Else
                    Using stream As Stream = myResponse.GetResponseStream()
                        Using reader As New StreamReader(stream, Encoding.UTF8)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                End If
                If myResponse.Headers("Location") IsNot Nothing Then
                    redirecturl = myResponse.Headers("Location")
                End If
                ResponseHeaders = myResponse.Headers
            End Using
        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        results = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try
        Return results
    End Function


End Class
