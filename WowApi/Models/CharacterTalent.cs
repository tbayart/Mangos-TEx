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
    }
}
