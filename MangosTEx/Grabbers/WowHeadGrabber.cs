using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MangosTEx.Grabbers
{
    public class WowHeadGrabber
    {
        private WebClient _client;
        private string _baseUrl;
        private string _baseQuery;

        public WowHeadGrabber(string langage, IWebProxy proxy = null)
        {
            _client = new WebClient();
            _client.Proxy = proxy;
            _baseUrl = string.Format("http://{0}.wowhead.com/", langage);
            _baseQuery = string.Concat(_baseUrl, "{0}={1}");
        }

        public string GetItem(int id)
        {
            string address = string.Format(_baseQuery, "item", id);
            return _client.DownloadString(address);
        }
    }
}
