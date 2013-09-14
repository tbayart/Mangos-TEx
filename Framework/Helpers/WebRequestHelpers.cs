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
            var request = WebRequest.Create(address);
            var response = request.GetResponse();
            byte[] buffer = new byte[1024];
            StringBuilder sb = new StringBuilder();

            using (Stream responseStream = response.GetResponseStream())
            {
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
}
