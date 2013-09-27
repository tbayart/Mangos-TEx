using System.Collections.Generic;
using Framework;

namespace MangosTEx.Events
{
    public class MangosDataGameObjectEventArgs : EventArgs<IEnumerable<MangosData.Models.GameObject>>
    {
        public MangosDataGameObjectEventArgs(IEnumerable<MangosData.Models.GameObject> go) : base(go) { }
    }

    public class LocaleGameObjectEventArgs : EventArgs<WowheadApi.Models.GameObject>
    {
        public LocaleGameObjectEventArgs(WowheadApi.Models.GameObject go) : base(go) { }
    }
}
