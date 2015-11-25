using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Framework.Commands;
using Framework.MVVM;

namespace MangosTEx.ViewModels
{
    public class HomeViewModel : ObservableViewModel
    {
        #region Ctor
        public HomeViewModel()
        {
            DatabaseCultureList = MangosTEx.Services.MangosProvider.SupportedCultures;
            LocalizationCultureList = WowheadApi.LocalizationHelper.SupportedCultures;
            CheckSettings();
        }
        #endregion Ctor

        #region Properties
        public CultureInfo[] DatabaseCultureList
        {
            get { return _databaseCultureList; }
            private set
            {
                _databaseCultureList = value;
                RaisePropertyChanged(() => DatabaseCultureList);
            }
        }
        private CultureInfo[] _databaseCultureList;

        public CultureInfo[] LocalizationCultureList
        {
            get { return _localizationCultureList; }
            private set
            {
                _localizationCultureList = value;
                RaisePropertyChanged(() => LocalizationCultureList);
            }
        }
        private CultureInfo[] _localizationCultureList;
        #endregion Properties

        #region Methods
        private void CheckSettings()
        {
            var settings = Properties.Settings.Default;
            if (DatabaseCultureList.Contains(settings.DatabaseCulture) == false)
                settings.DatabaseCulture = MangosTEx.Services.MangosProvider.DefaultCulture;
            if (LocalizationCultureList.Contains(settings.LocalizationCulture) == false)
                settings.LocalizationCulture = WowheadApi.LocalizationHelper.DefaultCulture;
        }

        private void settingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
        #endregion Methods

        #region Commands
        protected override void InitializeCommands()
        {
            base.InitializeCommands();
            SaveSettingsCommand = new DelegateCommand(SaveSettingsExecute);
        }

        public ICommand SaveSettingsCommand { get; private set; }
        private void SaveSettingsExecute()
        {
            Properties.Settings.Default.Save();
        }
        #endregion Commands
    }
}
