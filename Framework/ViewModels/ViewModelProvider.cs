using System;
using Framework.InstanceProvider;

namespace Framework.MVVM
{
    public class ViewModelProvider : InstanceProviderBase<IViewModel>
    {
        #region Ctor
        public ViewModelProvider(Type type)
            : base(type)
        {
        }
        public ViewModelProvider(string typeName)
            : base(typeName)
        {
        }
        #endregion Ctor
    }
}
