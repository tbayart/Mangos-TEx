using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowheadApi.Models
{
    public class GameObject
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public string Error { get; set; }
    }
}
