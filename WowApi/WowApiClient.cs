using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using WowApi.Models;

namespace WowApi
{
    public class WowApiClient
    {
        public Item GetItem(int id)
        {
            string address = string.Format("http://eu.battle.net/api/wow/item/{0}", id);

            WebRequest request = WebRequest.Create(address);
            using(WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    int progress = 0;
                    int remaining = (int)response.ContentLength;
                    byte[] buffer = new byte[response.ContentLength];
                    while (progress < response.ContentLength)
                    {
                        int read = stream.Read(buffer, progress, remaining);
                        if (read == 0)
                            throw new Exception("");
                        progress += read;
                        remaining -= read;
                    }

                    string result = System.Text.Encoding.UTF8.GetString(buffer);
                    return JsonConvert.DeserializeObject<Item>(result);
                }
            }
        }
    }
}
