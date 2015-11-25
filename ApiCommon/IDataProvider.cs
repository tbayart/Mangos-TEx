using Framework.Interfaces;

namespace ApiCommon
{
    public interface IDataProvider
    {
        string GetProviderName();
        IStatusMessage GetStatus();
        string ProvideData(string source);
    }
}
