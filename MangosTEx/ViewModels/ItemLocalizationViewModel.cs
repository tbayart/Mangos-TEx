using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ApiCommon;
using Framework.Commands;
using Framework.Debug;
using Framework.MVVM;
using MangosTEx.Models;
using MangosTEx.Services;
using MangosTEx.Services.ApiDataProvider;
using dbItem = MangosTEx.Services.Models.Item;
using Framework.Services;

namespace MangosTEx.ViewModels
{
    public class ItemLocalizationViewModel : ObservableViewModel
    {
        #region Ctor
        public ItemLocalizationViewModel()
        {
            SelectedLocalizationSource = LocalizationSourceList.First();
            SetCollectionView(null);
            LoadItemsAsync();
        }
        #endregion Ctor

        #region Properties
        public ICollectionView Items { get; private set; }

        public bool HideTranslated
        {
            get { return _hideTranslated; }
            set
            {
                _hideTranslated = value;
                RaisePropertyChanged(() => HideTranslated);
                RefreshCollectionView();
            }
        }
        private bool _hideTranslated;

        public bool HideMatchingTranslation
        {
            get { return _hideMatchingTranslation; }
            set
            {
                _hideMatchingTranslation = value;
                RaisePropertyChanged(() => HideMatchingTranslation);
                RefreshCollectionView();
            }
        }
        private bool _hideMatchingTranslation;

        public bool ShowErrorOnly
        {
            get { return _showErrorOnly; }
            set
            {
                _showErrorOnly = value;
                RaisePropertyChanged(() => ShowErrorOnly);
                RefreshCollectionView();
            }
        }
        private bool _showErrorOnly;

        public IEnumerable<string> LocalizationSourceList { get { return _localizationSourceList; } }
        private const string _localizationSourceBattleNet = "BattleNet";
        private const string _localizationSourceWowhead = "Wowhead";
        private IEnumerable<string> _localizationSourceList = new string[] { _localizationSourceWowhead, _localizationSourceBattleNet };

        public string SelectedLocalizationSource
        {
            get { return _selectedLocalizationSource; }
            set
            {
                _selectedLocalizationSource = value;
                RaisePropertyChanged(nameof(SelectedLocalizationSource));
            }
        }
        private string _selectedLocalizationSource;

        private Properties.Settings Settings { get { return Properties.Settings.Default; } }
        #endregion Properties

        #region Methods
        private Action RefreshCollectionView;
        private void SetCollectionView(IEnumerable<LocalizedItem> source)
        {
            Items = CollectionViewSource.GetDefaultView(source);
            if (Items != null)
            {
                Items.Filter = ItemFilter;
                RaisePropertyChanged(() => Items);
                RefreshCollectionView = Items.Refresh;
            }
            else
            {
                RefreshCollectionView = () => { };
            }
        }

        // filter untranslated items
        private bool ItemFilter(object obj)
        {
            LocalizedItem item = (LocalizedItem)obj;
            return ShowErrorOnly ? item.Status == LocalizationStatus.Error
                : FilterHideUntranslated(item) && FilterHideMatchingTranslation(item);
        }
        private bool FilterHideUntranslated(LocalizedItem item)
        {
            return HideTranslated == false || string.IsNullOrEmpty(item.DatabaseEntity.Name);
        }
        private bool FilterHideMatchingTranslation(LocalizedItem item)
        {
            return HideMatchingTranslation == false
                || item.TranslatedEntity == null
                || item.Status != LocalizationStatus.Equal;
        }

        private int _loadProcessId;
        private void LoadItemsAsync()
        {
            CultureInfo culture = Settings.DatabaseCulture;
            // get a processId so we can stop it if the user request a new batch before we finished this one
            int processId = ++_loadProcessId;
            SetCollectionView(null);
            Observable.Start(() =>
                {
                    // load items from database
                    using (var provider = new MangosProvider())
                    using (var pc = new PerformanceChecker("GetItems"))
                    {
                        return provider.GetItems(culture)
                            .Select(o => new LocalizedItem(o))
                            .ToList();
                    }
                })
                .ObserveOnDispatcher()
                .Subscribe(result =>
                {
                    // check processId to make sure the user has not requested another batch meantime
                    if (processId != _loadProcessId)
                        return;

                    // display loaded items to user
                    SetCollectionView(result);
                }, OnError);
        }

        private void OnError(Exception ex)
        {
            throw ex;
        }

        private void GetItemsLocalesAsync(IEnumerable<LocalizedItem> items, LocalizationSource localizationSource)
        {
            CultureInfo culture = Settings.LocalizationCulture;
            IDataProvider provider = DataProviderManager.GetHttpCachedProvider();

            Func<int, dbItem> grabItem = null;
            object grabber;
            if (localizationSource == LocalizationSource.Wowhead)
            {
                grabber = new WowheadApi.WowheadClient(provider, culture);
                grabItem = (id) =>
                    {
                        var data = ((WowheadApi.WowheadClient)grabber).GetItem(id);
                        var item = new dbItem { Id = id };
                        if (data != null)
                        {
                            item.Name = data.Name;
                            if (string.IsNullOrEmpty(data.Description) == false)
                                item.Description = data.Description;
                        }
                        return item;
                    };
            }
            else if (localizationSource == LocalizationSource.BattleNet)
            {
                var apiKey = Services.ViewModels.SettingsViewModel.GetBattleNetApiKey();
                grabber = new WowApi.WowApiClient(provider, culture, apiKey);
                grabItem = (id) =>
                    {
                        var data = ((WowApi.WowApiClient)grabber).GetItem(id);
                        var item = new dbItem { Id = id };
                        if (data != null)
                        {
                            item.Name = data.Name;
                            if (string.IsNullOrEmpty(data.Description) == false)
                                item.Description = data.Description;
                        }
                        return item;
                    };
            }

            Parallel.ForEach(items, item => item.ResetStatus());

            Observable.Start(() =>
                Parallel.ForEach(items, item =>
                    {
                        try
                        {
                            // update translated item
                            item.TranslatedEntity = grabItem(item.DatabaseEntity.Id);
                            item.Error = null;
                        }
                        catch (Exception ex)
                        {
                            // if we got an error, keep it to investigate
                            item.Error = ex.Message;
                        }
                    }));
        }

        private void UpdateDatabaseAsync(IEnumerable<LocalizedItem> items)
        {
            CultureInfo culture = Settings.DatabaseCulture;
            Observable.Start(() =>
            {
                // select items to update and convert them
                var dbItems = items
                    .Where(o => o.DatabaseEntity != null && string.IsNullOrEmpty(o.DatabaseEntity.Name) == false
                                && o.TranslatedEntity != null && string.IsNullOrEmpty(o.TranslatedEntity.Name) == false)
                    .Select(GetTranslatedDbItem);

                if (dbItems.Any() == false)
                    return;

                using (var provider = new MangosProvider())
                {
                    dbItems = provider.UpdateItems(dbItems, culture)
                        .ToList();
                }

                // refresh updated items
                items.Join(dbItems, o => o.DatabaseEntity.Id, o => o.Id, (li, dbi) => new { li, dbi })
                    .ToList()
                    .ForEach(o =>
                        {
                            o.li.Error = null;
                            o.li.DatabaseEntity = o.dbi;
                        });
            });
        }

        private dbItem GetTranslatedDbItem(LocalizedItem item)
        {
            return new dbItem
            {
                Id = item.TranslatedEntity.Id,
                Name = item.TranslatedEntity.Name,
                Description = item.TranslatedEntity.Description
            };
        }
        #endregion Methods

        #region Commands
        protected override void InitializeCommands()
        {
            base.InitializeCommands();
            UpdateLocalizationWowheadCommand = new DelegateCommand<IList>(o => UpdateLocalizationExecute(o, LocalizationSource.Wowhead));
            UpdateLocalizationBattleNetCommand = new DelegateCommand<IList>(o => UpdateLocalizationExecute(o, LocalizationSource.BattleNet));
            UpdateDatabaseCommand = new DelegateCommand<IList>(UpdateDatabaseExecute);
        }

        public ICommand UpdateLocalizationWowheadCommand { get; private set; }
        public ICommand UpdateLocalizationBattleNetCommand { get; private set; }
        private void UpdateLocalizationExecute(IList selection, LocalizationSource localizationSource)
        {
            // retrieve selection with the right Type and create a copy
            var items = selection.OfType<LocalizedItem>().ToList();
            // then launch item translation process
            GetItemsLocalesAsync(items, localizationSource);
        }

        public ICommand UpdateDatabaseCommand { get; set; }
        private void UpdateDatabaseExecute(IList selection)
        {
            // retrieve selection with the right Type and create a copy
            var items = selection.OfType<LocalizedItem>().ToList();
            //ServiceProvider.GetInstance<InteractionService>().UserChoice
            UpdateDatabaseAsync(items);
        }
        #endregion Commands
    }
}
