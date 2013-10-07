namespace MangosTEx.Models
{
    public class ConnectionStatus
    {
        public ConnectionStatus(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool StatusOk { get { return string.IsNullOrEmpty(ErrorMessage); } }
        public string ErrorMessage { get; set; }

        public override string ToString() { return ErrorMessage; }
    }
}
