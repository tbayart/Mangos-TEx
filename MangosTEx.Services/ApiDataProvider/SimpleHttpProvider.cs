using System;
using ApiCommon;
using Framework.Helpers;
using Framework.Interfaces;
using MangosTEx.Services.Models;

namespace MangosTEx.Services.ApiDataProvider
{
    internal class SimpleHttpProvider : IDataProvider
    {
        public string GetProviderName()
        {
            return "Simple HTTP Provider";
        }

        public IStatusMessage GetStatus()
        {
            try
            {
                WebRequestHelpers.DownloadString("http://www.perdu.com");
            }
            catch (Exception ex)
            {
                return new ConnectionStatus(false, ex.Message);
            }
            return new ConnectionStatus(true, "Connection OK");
        }

        public string ProvideData(string source)
        {
            return WebRequestHelpers.DownloadString(source);
        }
    }
}
