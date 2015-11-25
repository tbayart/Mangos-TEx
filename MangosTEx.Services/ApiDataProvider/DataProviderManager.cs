using ApiCommon;
using Framework.Services;

namespace MangosTEx.Services.ApiDataProvider
{
    public static class DataProviderManager
    {
        public static IDataProvider GetSimpleHttpProvider()
        {
            return new SimpleHttpProvider();
        }

        public static IDataProvider GetHttpCachedProvider()
        {
            var provider = new HttpCachedProvider();
            provider.InitProvider();
            return provider;
        }
    }
}
