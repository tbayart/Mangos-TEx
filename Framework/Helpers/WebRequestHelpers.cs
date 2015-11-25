using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Framework.Helpers
{
    public static class WebRequestHelpers
    {
        public static string DownloadString(string address)
        {
            try
            {
                var request = WebRequest.Create(address);
                if (request is HttpWebRequest)
                    ((HttpWebRequest)request).UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";

                using (var response = request.GetResponse())
                {
                    StringBuilder sb = new StringBuilder();
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        byte[] buffer = new byte[1024];
                        for (; ; )
                        {
                            int read = responseStream.Read(buffer, 0, buffer.Length);
                            if (read == 0)
                                break;
                            sb.Append(System.Text.Encoding.UTF8.GetString(buffer, 0, read));
                        }
                    }
                    return sb.ToString();
                }
            }
            catch (WebException ex)
            {
                // when error 404, return empty data
                if(ex.Message.Contains("(404)"))
                    return string.Empty;
                // else, leave the exception to somebody else
                throw;
            }
        }
    }
}
