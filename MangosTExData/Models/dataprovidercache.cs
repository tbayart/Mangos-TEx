using System;

namespace MangosTExData.Context
{
    public partial class dataprovidercache
    {
        public dataprovidercache()
        {
        }

        public dataprovidercache(string source, string data)
        {
            this.source = source;
            this.data = data;
            this.date = DateTime.Now;
        }
    }
}
