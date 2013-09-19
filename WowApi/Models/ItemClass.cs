using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowApi.Models
{
    public class ItemClassBase
    {
        public int Class { get; set; }
        public string Name { get; set; }
    }

    public class ItemClass : ItemClassBase
    {
        public List<ItemClassBase> Subclasses { get; set; }
    }
}
