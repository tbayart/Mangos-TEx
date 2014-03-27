using Framework.Interfaces;

namespace MangosTEx.Services.Models
{
    public class ConnectionStatus : IStatusMessage
    {
        public ConnectionStatus(bool isOk, string message)
        {
            IsOk = isOk;
            Message = message;
        }

        public bool IsOk { get; private set; }
        public string Message { get; private set; }
    }
}
