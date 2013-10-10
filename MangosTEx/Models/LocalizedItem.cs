using Framework.Interfaces;
using Framework.MVVM;

namespace MangosTEx.Models
{
    public class LocalizedItem : ObservableObject, ISelectable
    {
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

        #region ISelectable
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }
        private bool _isSelected;
        #endregion ISelectable
    }
}
