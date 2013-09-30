using System.Collections.Generic;
using WowheadApi.Models.Base;

namespace WowheadApi.Models
{
    public class GameObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<GameObjectData> RelatedData { get; set; }
    }
}
