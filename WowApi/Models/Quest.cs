namespace WowApi.Models
{
    public class Quest
    {
        public string Category { get; set; }
        public int Id { get; set; }
        public int Level { get; set; }
        public int ReqLevel { get; set; }
        public int SuggestedPartyMembers { get; set; }
        public string Title { get; set; }
    }
}
