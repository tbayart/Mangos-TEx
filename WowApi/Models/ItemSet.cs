using System.Collections.Generic;

namespace WowApi.Models
{
    public class ItemSet
    {
        public int Id { get; set; }
        public List<int> Items { get; set; }
        public string Name { get; set; }
        public List<SetBonus> SetBonuses { get; set; }

        public class SetBonus
        {
            public string Description { get; set; }
            public int Threshold { get; set; }
        }
    }
}
