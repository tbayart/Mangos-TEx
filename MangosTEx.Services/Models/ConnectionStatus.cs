namespace MangosTEx.Services.Models
{
    public class ConnectionStatus
    {
        public ConnectionStatus(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        public bool IsValid { get; private set; }
        public string Message { get; private set; }

        public override string ToString() { return Message; }
    }
}
