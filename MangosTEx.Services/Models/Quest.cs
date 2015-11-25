using Framework.MVVM;

namespace MangosTEx.Services.Models
{
    public class Quest : ObservableObject
    {
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(() => Id);
            }
        }
        private int _id;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }
        private string _title;

        public string Details
        {
            get { return _details; }
            set
            {
                _details = value;
                RaisePropertyChanged(() => Details);
            }
        }
        private string _details;

        public string Objectives
        {
            get { return _objectives; }
            set
            {
                _objectives = value;
                RaisePropertyChanged(() => Objectives);
            }
        }
        private string _objectives;

        public string OfferRewardText
        {
            get { return _offerRewardText; }
            set
            {
                _offerRewardText = value;
                RaisePropertyChanged(() => OfferRewardText);
            }
        }
        private string _offerRewardText;

        public string RequestItemsText
        {
            get { return _requestItemsText; }
            set
            {
                _requestItemsText = value;
                RaisePropertyChanged(() => RequestItemsText);
            }
        }
        private string _requestItemsText;

        public string EndText
        {
            get { return _endText; }
            set
            {
                _endText = value;
                RaisePropertyChanged(() => EndText);
            }
        }
        private string _endText;

        public string CompletedText
        {
            get { return _completedText; }
            set
            {
                _completedText = value;
                RaisePropertyChanged(() => CompletedText);
            }
        }
        private string _completedText;

        public string ObjectiveText1
        {
            get { return _objectiveText1; }
            set
            {
                _objectiveText1 = value;
                RaisePropertyChanged(() => ObjectiveText1);
            }
        }
        private string _objectiveText1;

        public string ObjectiveText2
        {
            get { return _objectiveText2; }
            set
            {
                _objectiveText2 = value;
                RaisePropertyChanged(() => ObjectiveText2);
            }
        }
        private string _objectiveText2;

        public string ObjectiveText3
        {
            get { return _objectiveText3; }
            set
            {
                _objectiveText3 = value;
                RaisePropertyChanged(() => ObjectiveText3);
            }
        }
        private string _objectiveText3;

        public string ObjectiveText4
        {
            get { return _objectiveText4; }
            set
            {
                _objectiveText4 = value;
                RaisePropertyChanged(() => ObjectiveText4);
            }
        }
        private string _objectiveText4;

    }
}
