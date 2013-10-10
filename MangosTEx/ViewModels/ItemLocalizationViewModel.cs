using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Framework.MVVM;
using MangosTEx.Events;
using MangosTEx.Models;
using MangosTEx.Services;
using Framework.Helpers;
using WowheadApi;
using dbItem = MangosTEx.Services.Models.Item;
using whItem = WowheadApi.Models.Item;
using Framework.Debug;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using Framework.Interfaces;
using System.Collections;
using Framework.Commands;

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

        public bool ShowSelectionOnly
        {
            get { return _showSelectionOnly; }
            set
            {
                _showSelectionOnly = value;
                RaisePropertyChanged(() => ShowSelectionOnly);
                RefreshCollectionView();
            }
        }
        private bool _showSelectionOnly;

        public bool ShowTranslatedOnly
        {
            get { return _showTranslatedOnly; }
            set
            {
                _showTranslatedOnly = value;
                RaisePropertyChanged(() => ShowTranslatedOnly);
                RefreshCollectionView();
            }
        }
        private bool _showTranslatedOnly;

        public bool ShowTranslationChangedOnly
        {
            get { return _showTranslationChangedOnly; }
            set
            {
                _showTranslationChangedOnly = value;
                RaisePropertyChanged(() => ShowTranslationChangedOnly);
                RefreshCollectionView();
            }
        }
        private bool _showTranslationChangedOnly;

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
            return FilterSelection(item)
                && FilterTranslated(item)
                && FilterTranslationChanged(item);
        }
        private bool FilterSelection(LocalizedItem item)
        {
            return ShowSelectionOnly == false || item.IsSelected;
        }
        private bool FilterTranslated(LocalizedItem item)
        {
            return ShowTranslatedOnly == false || item.TranslatedItem != null;
        }
        private bool FilterTranslationChanged(LocalizedItem item)
        {
            return ShowTranslationChangedOnly == false || item.TranslatedItem == null
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
        #endregion Methods

        #region Commands
        protected override void InitializeCommands()
        {
            base.InitializeCommands();
            UpdateLocalizationCommand = new DelegateCommand(UpdateLocalizationExecute);
        }

        public ICommand UpdateLocalizationCommand { get; private set; }
        private void UpdateLocalizationExecute()
        {
            var items = Items.OfType<LocalizedItem>().Where(o => o.IsSelected).ToList();
            GetItemsLocalesAsync(items);
        }
        #endregion Commands
    }
}
