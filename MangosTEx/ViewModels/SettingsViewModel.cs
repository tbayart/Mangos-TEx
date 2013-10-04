using System;
using System.Security;
using System.Security.Cryptography;
using System.Windows.Input;
using Framework.Commands;
using Framework.Helpers;
using Framework.MVVM;
using Framework.Services.Interfaces;
using MangosTEx.Models;

namespace MangosTEx.ViewModels
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
                switch (Properties.Settings.Default.ProxyAuth)
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
            DatabaseHost = Properties.Settings.Default.DatabaseHost;
            DatabaseName = Properties.Settings.Default.DatabaseName;
            UseProxy = Properties.Settings.Default.UseProxy;
            ProxyAddress = Properties.Settings.Default.ProxyAddress;
            ProxyPort = Properties.Settings.Default.ProxyPort;
            ProxyAuth = Properties.Settings.Default.ProxyAuth;
            ProxyUsername = Properties.Settings.Default.ProxyUsername;
            ProxyPassword = GetProxyPassword();
        }

        private void SaveApplicationSettings()
        {
            Properties.Settings.Default.DatabaseHost = DatabaseHost;
            Properties.Settings.Default.DatabaseName = DatabaseName;
            Properties.Settings.Default.UseProxy = UseProxy;
            Properties.Settings.Default.ProxyAddress = ProxyAddress;
            Properties.Settings.Default.ProxyPort = ProxyPort;
            Properties.Settings.Default.ProxyAuth = ProxyAuth;
            Properties.Settings.Default.ProxyUsername = ProxyUsername;
            Properties.Settings.Default.ProxyPassword = GetAlgorithm().CreateEncryptor().Encrypt(ProxyPassword);
            Properties.Settings.Default.Save();
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
