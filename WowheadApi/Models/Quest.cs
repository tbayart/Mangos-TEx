namespace WowheadApi.Models
{
    public class Quest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string Objectives { get; set; }
        public string OfferRewardText { get; set; }
        public string RequestItemsText { get; set; }
        public string EndText { get; set; }
        public string CompletedText { get; set; }
        public string ObjectiveText1 { get; set; }
        public string ObjectiveText2 { get; set; }
        public string ObjectiveText3 { get; set; }
        public string ObjectiveText4 { get; set; }
    }
}
