using ApiCommon.Interfaces;
using Framework.Helpers;

namespace ApiCommon.Impl
{
    internal class SimpleHttpProvider : IDataProvider
    {
        public string ProvideData(string source)
        {
            return WebRequestHelpers.DownloadString(source);
        }
    }
}
