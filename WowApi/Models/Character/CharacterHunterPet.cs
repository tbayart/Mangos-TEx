namespace WowApi.Models
{
    public class CharacterHunterPet
    {
        public string CalcSpec { get; set; }
        public int Creature { get; set; }
        public int FamilyId { get; set; }
        public string FamilyName { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public int Slot { get; set; }
        public HunterPetSpec Spec { get; set; }

        public class HunterPetSpec
        {
            public string BackgroundImage { get; set; }
            public string Description { get; set; }
            public string Icon { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public string Role { get; set; }
        }
    }
}
