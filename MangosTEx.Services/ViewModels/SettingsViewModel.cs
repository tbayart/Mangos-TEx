using System;
using System.Security;
using System.Security.Cryptography;
using System.Windows.Input;
using MangosTEx.Services.DataTypes;
using Framework.Commands;
using Framework.Helpers;
using Framework.MVVM;
using Framework.Services.Interfaces;

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

        private Properties.Settings Settings { get { return Properties.Settings.Default; } }
        #endregion Properties

        #region Methods
        private SecureString GetProxyPassword()
        {
            return GetAlgorithm().CreateDecryptor().Decrypt(Properties.Settings.Default.ProxyPassword);
        }

        public void ApplySettings()
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
                        proxy.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.ProxyUsername, GetProxyPassword());
                        break;
                    case ProxyAuthEnum.NoAuthentification:
                    default:
                        break;
                }
            }
            System.Net.WebRequest.DefaultWebProxy = proxy;
        }

        private void LoadApplicationSettings()
        {
            DatabaseHost = Settings.DatabaseHost;
            DatabaseName = Settings.DatabaseName;
            UseProxy = Settings.UseProxy;
            ProxyAddress = Settings.ProxyAddress;
            ProxyPort = Settings.ProxyPort;
            ProxyAuth = Settings.ProxyAuth;
            ProxyUsername = Settings.ProxyUsername;
            ProxyPassword = GetProxyPassword();
        }

        private void SaveApplicationSettings()
        {
            Settings.DatabaseHost = DatabaseHost;
            Settings.DatabaseName = DatabaseName;
            Settings.UseProxy = UseProxy;
            Settings.ProxyAddress = ProxyAddress;
            Settings.ProxyPort = ProxyPort;
            Settings.ProxyAuth = ProxyAuth;
            Settings.ProxyUsername = ProxyUsername;
            Settings.ProxyPassword = GetAlgorithm().CreateEncryptor().Encrypt(ProxyPassword);
            Settings.Save();
        }

        private SymmetricAlgorithm GetAlgorithm()
        {
            var algo = Aes.Create();
            algo.Key = Convert.FromBase64String("6EvipoVj2gGtKUuIVHC5Tm3UIyBEmraD3UcJcrTArCc=");
            algo.IV = Convert.FromBase64String("RoDnj57f/XJS/7klvBEAPQ==");
            return algo;
        }
        #endregion Methods

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
            ApplySettings();
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
