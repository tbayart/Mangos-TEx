using System.Security;
using System.Windows.Input;
using Framework.Commands;
using Framework.Helpers;
using Framework.MVVM;
using Framework.Services.Interfaces;
using MangosTEx.Services.DataTypes;

namespace MangosTEx.Services.ViewModels
{
    public class SettingsViewModel : ObservableViewModel, ICloseable
    {
        #region Ctor
        public SettingsViewModel()
        {
            LoadApplicationSettings();
        }
        #endregion Ctor

        #region Properties
        public string DisplayName { get { return "Connection settings"; } }

        public string DatabaseHost
        {
            get { return _databaseHost; }
            set
            {
                _databaseHost = value;
                RaisePropertyChanged(() => DatabaseHost);
            }
        }
        private string _databaseHost;

        public uint DatabasePort
        {
            get { return _databasePort; }
            set
            {
                _databasePort = value;
                RaisePropertyChanged(() => DatabasePort);
            }
        }
        private uint _databasePort;

        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                _databaseName = value;
                RaisePropertyChanged(() => DatabaseName);
            }
        }
        private string _databaseName;

        public string DatabaseUsername
        {
            get { return _databaseUsername; }
            set
            {
                _databaseUsername = value;
                RaisePropertyChanged(() => DatabaseUsername);
            }
        }
        private string _databaseUsername;

        public SecureString DatabasePassword
        {
            get { return _databasePassword; }
            set
            {
                _databasePassword = value;
                RaisePropertyChanged(() => DatabasePassword);
            }
        }
        private SecureString _databasePassword;

        public bool UseProxy
        {
            get { return _useProxy; }
            set
            {
                _useProxy = value;
                RaisePropertyChanged(() => UseProxy);
            }
        }
        private bool _useProxy;

        public string ProxyAddress
        {
            get { return _proxyAddress; }
            set
            {
                _proxyAddress = value;
                RaisePropertyChanged(() => ProxyAddress);
            }
        }
        private string _proxyAddress;

        public int ProxyPort
        {
            get { return _proxyPort; }
            set
            {
                _proxyPort = value;
                RaisePropertyChanged(() => ProxyPort);
            }
        }
        private int _proxyPort;

        public ProxyAuthEnum ProxyAuth
        {
            get { return _proxyAuth; }
            set
            {
                _proxyAuth = value;
                RaisePropertyChanged(() => ProxyAuth);
            }
        }
        private ProxyAuthEnum _proxyAuth;

        public string ProxyUsername
        {
            get { return _proxyUsername; }
            set
            {
                _proxyUsername = value;
                RaisePropertyChanged(() => ProxyUsername);
            }
        }
        private string _proxyUsername;

        public SecureString ProxyPassword
        {
            get { return _proxyPassword; }
            set
            {
                _proxyPassword = value;
                RaisePropertyChanged(() => ProxyPassword);
            }
        }
        private SecureString _proxyPassword;

        private static Properties.Settings Settings { get { return Properties.Settings.Default; } }
        #endregion Properties

        #region Methods
        private void LoadApplicationSettings()
        {
            DatabaseHost = Settings.DatabaseHost;
            DatabasePort = Settings.DatabasePort;
            DatabaseName = Settings.DatabaseName;
            DatabaseUsername = Settings.DatabaseUsername;
            DatabasePassword = MangosProvider.Decrypt(Settings.DatabasePassword);
            UseProxy = Settings.UseProxy;
            ProxyAddress = Settings.ProxyAddress;
            ProxyPort = Settings.ProxyPort;
            ProxyAuth = Settings.ProxyAuth;
            ProxyUsername = Settings.ProxyUsername;
            ProxyPassword = MangosProvider.Decrypt(Settings.ProxyPassword);
        }

        private void SaveApplicationSettings()
        {
            Settings.DatabaseHost = DatabaseHost;
            Settings.DatabasePort = DatabasePort;
            Settings.DatabaseName = DatabaseName;
            Settings.DatabaseUsername = DatabaseUsername;
            Settings.DatabasePassword = MangosProvider.Encrypt(DatabasePassword);
            Settings.UseProxy = UseProxy;
            Settings.ProxyAddress = ProxyAddress;
            Settings.ProxyPort = ProxyPort;
            Settings.ProxyAuth = ProxyAuth;
            Settings.ProxyUsername = ProxyUsername;
            Settings.ProxyPassword = MangosProvider.Encrypt(ProxyPassword);
            Settings.Save();
        }
        #endregion Methods

        #region Static Methods
        public static void ApplyProxySettings()
        {
            System.Net.WebProxy proxy = null;
            if (Properties.Settings.Default.UseProxy == true)
            {
                proxy = new System.Net.WebProxy(Properties.Settings.Default.ProxyAddress, Properties.Settings.Default.ProxyPort);
                switch (Settings.ProxyAuth)
                {
                    case ProxyAuthEnum.DefaultAuthentification:
                        proxy.UseDefaultCredentials = true;
                        break;
                    case ProxyAuthEnum.CustomAuthentification:
                        proxy.Credentials = new System.Net.NetworkCredential(
                            Settings.ProxyUsername,
                            MangosProvider.Decrypt(Settings.ProxyPassword));
                        break;
                    case ProxyAuthEnum.NoAuthentification:
                    default:
                        break;
                }
            }
            System.Net.WebRequest.DefaultWebProxy = proxy;
        }
        #endregion Static Methods

        #region Commands
        protected override void InitializeCommands()
        {
            base.InitializeCommands();
            SaveCommand = new DelegateCommand(SaveExecute, SaveCanExecute);
        }

        public ICommand SaveCommand { get; private set; }
        private void SaveExecute()
        {
            SaveApplicationSettings();
            CloseCommand.Execute(null);
            ApplyProxySettings();
        }

        private bool SaveCanExecute()
        {
            return string.IsNullOrEmpty(DatabaseHost) == false
                && string.IsNullOrEmpty(DatabaseName) == false;
        }
        #endregion Commands

        #region ObservableViewModel
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            SaveCommand.RaiseCanExecuteChanged();
        }
        #endregion ObservableViewModel

        #region ICloseable
        public ICommand CloseCommand { get; set; }
        #endregion ICloseable
    }
}
