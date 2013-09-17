using System.Collections.Generic;

namespace WowApi.Models
{
    public class CharacterTalent
    {
        public string CalcGlyph { get; set; }
        public string CalcSpec { get; set; }
        public string CalcTalent { get; set; }
        public TalentGlyphs Glyphs { get; set; }
        public bool Selected { get; set; }
        public TalentSpec Spec { get; set; }
        public List<Talent> Talents { get; set; }

        public class TalentGlyphs
        {
            public List<TalentGlyph> Major { get; set; }
            public List<TalentGlyph> Minor { get; set; }
        }

        public class TalentGlyph
        {
            public int Glyph { get; set; }
            public string Icon { get; set; }
            public int Item { get; set; }
            public string Name { get; set; }
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

        public class Talent
        {
            public int Column { get; set; }
            public Spell Spell { get; set; }
            public int Tier { get; set; }
        }
    }
}
