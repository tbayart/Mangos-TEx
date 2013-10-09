using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using Framework.Helpers;
using System.Globalization;
using Framework.Commands;
using Framework.MVVM;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using Framework;
using WowheadApi;
using System.Reactive.Linq;
using MangosTEx.Events;
using Framework.Debug;
using Framework.Services;
using MangosTEx.Services.ViewModels;

namespace MangosTEx.ViewModels
{
    public class MainWindowViewModel : ObservableViewModel
    {
        #region Ctor
        public MainWindowViewModel()
        {
            //WowApi.WowApiClient c = new WowApi.WowApiClient(CultureInfo.CurrentCulture);
            //WowApi.Models.Character character;
            //character = c.GetCharacter("Elune", "Kerenn", WowApi.WowApiCharacterDataField.All);
            //character = c.GetCharacter("trollbane", "Hayase", WowApi.WowApiCharacterDataField.All);
            //character = c.GetCharacter("frostmane", "Galacta", WowApi.WowApiCharacterDataField.All);
            //System.Diagnostics.Debugger.Break();

            // wowhead tests
            // http://fr.wowhead.com/object=191656
            //var grabber = new WowheadClient(CultureInfo.CurrentCulture);
            //WowheadApi.Models.GameObject obj1 = grabber.GetGameObject(3714);
            //WowheadApi.Models.GameObject obj2 = grabber.GetGameObject(191656);

            SettingsViewModel.ApplyProxySettings();
            UpdateConnexionStatusExecute(true);
        }
        #endregion Ctor

        #region Properties
        private MangosTEx.Services.Models.ConnectionStatus _connectionStatus;
        public MangosTEx.Services.Models.ConnectionStatus ConnectionStatus
        {
            get { return _connectionStatus; }
            private set
            {
                _connectionStatus = value;
                RaisePropertyChanged(() => ConnectionStatus);
            }
        }

        private IEnumerable<string> _tabList = new List<string> { "Home", "Items", "Game objects" };
        public IEnumerable<string> TabList { get { return _tabList; } }

        public string CurrentTab
        {
            get { return _currentTab; }
            set
            {
                _currentTab = value;
                RaisePropertyChanged(() => CurrentTab);
            }
        }
        private string _currentTab;

        public IViewModel DataViewModel
        {
            get { return _dataViewModel; }
            private set
            {
                _dataViewModel = value;
                RaisePropertyChanged(() => DataViewModel);
            }
        }
        private IViewModel _dataViewModel;
        #endregion Properties

        #region Methods
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == GetPropertyName(() => CurrentTab))
            {
                if (CurrentTab == "Home")
                    DataViewModel = ViewModelProvider.GetInstance<HomeViewModel>();
                else if (CurrentTab == "Items")
                    DataViewModel = ViewModelProvider.GetInstance<ItemLocalizationViewModel>();
                else if (CurrentTab == "Game Objects")
                    DataViewModel = ViewModelProvider.GetInstance<GameObjectLocalizationViewModel>();
                else
                    DataViewModel = null;
            }
        }
        #endregion Methods

        #region Commands
        protected override void InitializeCommands()
        {
            base.InitializeCommands();
            UpdateConnexionStatusCommand = new DelegateCommand(() => UpdateConnexionStatusExecute());
            OpenSettingsCommand = new DelegateCommand(OpenSettingsExecute);
        }

        public ICommand UpdateConnexionStatusCommand { get; private set; }
        private void UpdateConnexionStatusExecute(bool forceUpdate = false)
        {
            if (ConnectionStatus == null && forceUpdate == false)
                return;

            ConnectionStatus = null;
            Observable.Start(() => MangosTEx.Services.MangosProvider.CheckDatabaseAccess())
                .ObserveOnDispatcher()
                .Subscribe(result => ConnectionStatus = result);
        }

        public ICommand OpenSettingsCommand { get; private set; }
        private void OpenSettingsExecute()
        {
            SettingsViewModel settings = ViewModelProvider.GetInstance<SettingsViewModel>();
            ServiceProvider.GetInstance<InteractionService>().ShowContent(settings);
            UpdateConnexionStatusCommand.Execute(null);
        }
        #endregion Commands
    }
}
