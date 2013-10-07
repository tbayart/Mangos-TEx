using MangosTEx.Services.ModelBase;

namespace MangosTEx.Services.Models
{
    public class PageText : GameObjectData
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

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(() => Text);
            }
        }
        private string _text;

        public string LocalizedText
        {
            get { return _localizedText; }
            set
            {
                _localizedText = value;
                RaisePropertyChanged(() => LocalizedText);
            }
        }
        private string _localizedText;

        public int NextPage
        {
            get { return _nextPage; }
            set
            {
                _nextPage = value;
                RaisePropertyChanged(() => NextPage);
            }
        }
        private int _nextPage;
    }
}
