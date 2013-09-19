using System.Collections.Generic;
using Newtonsoft.Json;

namespace WowApi.Models
{
    public class Talent
    {
        public List<TalentGlyph> Glyphs { get; set; }
        [JsonIgnore]
        public List<TalentTier> Talents { get; set; }
        public string Class { get; set; }
        public List<TalentSpec> Specs { get; set; }
        public List<TalentSpec> PetSpecs { get; set; }
    }

    public class TalentGlyph
    {
        public int Glyph { get; set; }
        public int Item { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int TypeId { get; set; }
    }

    public class TalentTier
    {
        public int Tier { get; set; }
        public int Column { get; set; }
        public Spell Spell { get; set; }
    }

    public class TalentSpec
    {
        public string BackgroundImage { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string Role { get; set; }
    }
}
