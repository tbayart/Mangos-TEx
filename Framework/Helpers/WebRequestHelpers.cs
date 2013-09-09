using System;
using System.IO;
using System.Net;

namespace Framework.Helpers
{
    public static class WebRequestHelpers
    {
        public static string DownloadString(string address)
        {
            var request = WebRequest.Create(address);
            var response = request.GetResponse();
            byte[] buffer = new byte[response.ContentLength];
            int readTotal = 0;

            Stream responseStream = response.GetResponseStream();
            while (readTotal < response.ContentLength)
            {
                int remaining = (int)(response.ContentLength - readTotal);
                int read = responseStream.Read(buffer, readTotal, remaining);
                if (read == 0)
                    throw new Exception("Connection lost ?");
                readTotal += read;
            }

            return System.Text.Encoding.UTF8.GetString(buffer);
        }
    }
}
