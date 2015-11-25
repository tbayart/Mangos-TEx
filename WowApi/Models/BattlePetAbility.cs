namespace WowApi.Models
{
    public class BattlePetAbility
    {
        public int Cooldown { get; set; }
        public string Icon { get; set; }
        public int Id { get; set; }
        public bool IsPassive { get; set; }
        public string Name { get; set; }
        public int PetTypeId { get; set; }
        public int Rounds { get; set; }
        public bool ShowHints { get; set; }
    }
}
