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
using ApiCommon.Interfaces;
using Framework.Commands;
using Framework.Debug;
using Framework.MVVM;
using MangosTEx.Models;
using MangosTEx.Services;
using WowheadApi;
using dbItem = MangosTEx.Services.Models.Item;

namespace MangosTEx.ViewModels
{
    public class ItemLocalizationViewModel : ObservableViewModel
    {
        #region Ctor
        public ItemLocalizationViewModel()
        {
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

        private void GetItemsLocalesAsync(IEnumerable<LocalizedItem> items)
        {
            CultureInfo culture = Settings.LocalizationCulture;
            IDataProvider provider = DataProviderManager.GetSimpleHttpProvider();
            var grabber = new WowheadClient(provider, culture);
            Observable.Start(() =>
                Parallel.ForEach(items, item =>
                    {
                        try
                        {
                            // update translated item
                            item.TranslatedEntity = grabber.GetItem(item.DatabaseEntity.Id);
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
                    //.Where(o => o.Status == LocalizationStatus.NotEqual)
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
                    .ForEach(o => o.li.DatabaseEntity = o.dbi);
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
            UpdateLocalizationCommand = new DelegateCommand<IList>(UpdateLocalizationExecute);
            UpdateDatabaseCommand = new DelegateCommand<IList>(UpdateDatabaseExecute);
        }

        public ICommand UpdateLocalizationCommand { get; private set; }
        private void UpdateLocalizationExecute(IList selection)
        {
            // retrieve selection with the right Type and create a copy
            var items = selection.OfType<LocalizedItem>().ToList();
            // then launch item translation process
            GetItemsLocalesAsync(items);
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
