using System.Collections.Generic;

namespace WowApi.Models
{
    public class Profession
    {
        public string Icon { get; set; }
        public int Id { get; set; }
        public int Max { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public List<int> Recipes { get; set; }
    }
}
