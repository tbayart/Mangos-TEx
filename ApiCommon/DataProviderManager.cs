using ApiCommon.Impl;
using ApiCommon.Interfaces;

namespace ApiCommon
{
    public static class DataProviderManager
    {
        public static IDataProvider GetSimpleHttpProvider()
        {
            return new SimpleHttpProvider();
        }
    }
}
