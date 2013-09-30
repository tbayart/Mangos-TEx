using System.Collections.Generic;
using Framework;

namespace MangosTEx.Events
{
    public class LocaleGameObjectEventArgs : EventArgs<WowheadApi.Models.GameObject>
    {
        public LocaleGameObjectEventArgs(WowheadApi.Models.GameObject go) : base(go) { }
    }
}
