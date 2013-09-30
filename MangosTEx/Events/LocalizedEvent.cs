using System.Collections.Generic;
using Framework;

namespace MangosTEx.Events
{
    public class LocaleItemEventArgs : EventArgs<WowheadApi.Models.Item>
    {
        public LocaleItemEventArgs(WowheadApi.Models.Item item) : base(item) { }
    }
}
