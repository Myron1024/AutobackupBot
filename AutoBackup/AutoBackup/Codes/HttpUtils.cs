using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace AutoBackup.Codes
{
    public class HttpUtils
    {

        public static string HttpPost(string url, string data = "", string contentType = "application/x-www-form-urlencoded")
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };    // 忽略证书错误
            byte[] buffer = Encoding.Default.GetBytes(data);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "post";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.ContentType = contentType;
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = 30000;
                request.ContentLength = buffer.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.Error("POST [" + url + "] error:" + ex.Message, ex);
                return "";
            }
        }

        public static string HttpPost(string url, string data = "", string contentType = "application/x-www-form-urlencoded", string origin = "", string referrer = "")
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };    // 忽略证书错误
            byte[] buffer = Encoding.Default.GetBytes(data);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "post";
                request.Accept = "application/json, text/javascript, */*; q=0.01";
                request.ContentType = contentType;
                request.Referer = referrer;
                request.Headers.Add("Origin", origin);
                request.Host = "www.jockeyclub.ph";
                request.Timeout = 30000;
                request.ContentLength = buffer.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.Error("POST [" + url + "] error:" + ex.Message, ex);
                return "";
            }
        }

        public static string HttpGet(string url)
        {
            return HttpGet(url, Encoding.UTF8);
        }

        public static string HttpGet(string url, Encoding encoding)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Timeout = 30000;
                var response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.Error("HttpGet error:" + ex.Message, ex);
                return "";
            }
        }

        public static string HttpPostFile(string url, NameValueCollection values, NameValueCollection files = null)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            // The first boundary
            byte[] boundaryBytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            // The last boundary
            byte[] trailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            // The first time it itereates, we need to make sure it doesn't put too many new paragraphs down or it completely messes up poor webbrick
            byte[] boundaryBytesF = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

            // Create the request and set parameters
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // Get request stream
            Stream requestStream = request.GetRequestStream();

            foreach (string key in values.Keys)
            {
                // Write item to stream
                byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", key, values[key]));
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                requestStream.Write(formItemBytes, 0, formItemBytes.Length);
            }

            if (files != null)
            {
                foreach (string key in files.Keys)
                {
                    if (File.Exists(files[key]))
                    {
                        string fileName = Path.GetFileName(files[key]);

                        int bytesRead = 0;
                        byte[] buffer = new byte[2048];
                        byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n", key, fileName));
                        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                        requestStream.Write(formItemBytes, 0, formItemBytes.Length);

                        using (FileStream fileStream = new FileStream(files[key], FileMode.Open, FileAccess.Read))
                        {
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                // Write file content to stream, byte by byte
                                requestStream.Write(buffer, 0, bytesRead);
                            }

                            fileStream.Close();
                        }
                    }
                }
            }

            // Write trailer and close stream
            requestStream.Write(trailer, 0, trailer.Length);
            requestStream.Close();

            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return reader.ReadToEnd();
            };
        }
    }
}
