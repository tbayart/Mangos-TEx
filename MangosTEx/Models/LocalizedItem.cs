using Framework.MVVM;

namespace MangosTEx.Models
{
    public class LocalizedItem : ObservableObject
    {
        #region Ctor
        public LocalizedItem(MangosTEx.Services.Models.Item databaseItem)
        {
            DatabaseItem = databaseItem;
            TranslatedItem = new WowheadApi.Models.Item { Id = databaseItem.Id };
        }
        #endregion Ctor

        #region Properties
        public MangosTEx.Services.Models.Item DatabaseItem
        {
            get { return _databaseItem; }
            set
            {
                _databaseItem = value;
                RaisePropertyChanged(() => DatabaseItem);
            }
        }
        private MangosTEx.Services.Models.Item _databaseItem;

        public WowheadApi.Models.Item TranslatedItem
        {
            get { return _translatedItem; }
            set
            {
                _translatedItem = value;
                RaisePropertyChanged(() => TranslatedItem);
            }
        }
        private WowheadApi.Models.Item _translatedItem;

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                RaisePropertyChanged(() => Error);
            }
        }
        private string _error;
        #endregion Properties
    }
}
