using System;
using System.Runtime.Serialization;

namespace Framework.InstanceProvider
{
    public class InstanceProviderException : Exception
    {
        public InstanceProviderException()
            : base()
        {
        }

        public InstanceProviderException(string message)
            : base(message)
        {
        }

        protected InstanceProviderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public InstanceProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
