using System.Collections.Generic;
using Framework;

namespace MangosTEx.Events
{
    public class MangosDataItemEventArgs : EventArgs<IEnumerable<MangosData.Models.Item>>
    {
        public MangosDataItemEventArgs(IEnumerable<MangosData.Models.Item> items) : base(items) { }
    }

    public class LocaleItemEventArgs : EventArgs<WowheadApi.Models.Item>
    {
        public LocaleItemEventArgs(WowheadApi.Models.Item item) : base(item) { }
    }
}
