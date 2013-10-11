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

        public bool HideUntranslated
        {
            get { return _hideUntranslated; }
            set
            {
                _hideUntranslated = value;
                RaisePropertyChanged(() => HideUntranslated);
                RefreshCollectionView();
            }
        }
        private bool _hideUntranslated;

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
            return ShowErrorOnly ? string.IsNullOrEmpty(item.Error) == false
                : FilterHideUntranslated(item) && FilterHideMatchingTranslation(item);
        }
        private bool FilterHideUntranslated(LocalizedItem item)
        {
            return HideUntranslated == false || item.TranslatedItem != null;
        }
        private bool FilterHideMatchingTranslation(LocalizedItem item)
        {
            return HideMatchingTranslation == false || item.TranslatedItem == null
                || item.DatabaseItem.Name != item.TranslatedItem.Name
                || item.DatabaseItem.Description != item.TranslatedItem.Description;
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
                    using (new PerformanceChecker("GetItems"))
                    {
                        return provider.GetItems(culture)
                            .Select(o => new LocalizedItem { DatabaseItem = o })
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
            var grabber = new WowheadClient(culture);
            Observable.Start(() =>
                Parallel.ForEach(items, item =>
                    {
                        try
                        {
                            // update translated item
                            item.TranslatedItem = grabber.GetItem(item.DatabaseItem.Id);
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
                var dbItems = items.Select(o => new dbItem { Id = o.TranslatedItem.Id, Name = o.TranslatedItem.Name, Description = o.TranslatedItem.Description });
                using (var provider = new MangosProvider())
                {
                    dbItems = provider.UpdateItems(dbItems, culture)
                        .ToList();
                }
                items.Join(dbItems, o => o.DatabaseItem.Id, o => o.Id, (li, dbi) => new { li, dbi })
                    .ToList()
                    .ForEach(o => o.li.DatabaseItem = o.dbi);
            });
        }
        #endregion Methods

        #region Commands
        public IList SelectedItems { get; set; }
        protected override void InitializeCommands()
        {
            base.InitializeCommands();
            UpdateLocalizationCommand = new DelegateCommand<IList>(UpdateLocalizationExecute);
            UpdateDatabaseCommand = new DelegateCommand<IList>(UpdateDatabaseExecute);
        }

        public ICommand UpdateLocalizationCommand { get; private set; }
        private void UpdateLocalizationExecute(IList selection)
        {
            // retrieve selection with the good Type and create a copy
            var items = selection.OfType<LocalizedItem>().ToList();
            // then launch item translation process
            GetItemsLocalesAsync(items);
        }

        public ICommand UpdateDatabaseCommand { get; set; }
        private void UpdateDatabaseExecute(IList selection)
        {
            // retrieve selection with the good Type and create a copy
            var items = selection.OfType<LocalizedItem>().ToList();
            //ServiceProvider.GetInstance<InteractionService>().UserChoice
            UpdateDatabaseAsync(items);
        }
        #endregion Commands
    }
}
