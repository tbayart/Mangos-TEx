using System;
using Framework.InstanceProvider;

namespace Framework.Services
{
    public class ServiceProvider : InstanceProviderBase<IService>
    {
        #region Ctor
        public ServiceProvider(Type type)
            : base(type)
        {
        }
        public ServiceProvider(string typeName)
            : base(typeName)
        {
        }
        #endregion Ctor
    }
}
