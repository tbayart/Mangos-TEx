namespace Framework.Interfaces
{
    public interface IStatusMessage
    {
        bool IsOk { get; }
        string Message { get; }
    }
}
