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

namespace MangosTEx.ViewModels
{
    public class ItemLocalizationViewModel : ObservableViewModel
    {
        #region Ctor
        public ItemLocalizationViewModel()
        {
            InitializeCommands();
            LoadItemsAsync();
        }
        #endregion Ctor

        #region Properties
        public int StartId
        {
            get { return _startId; }
            set
            {
                _startId = value;
                RaisePropertyChanged(() => StartId);
            }
        }
        private int _startId = 5000;

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                RaisePropertyChanged(() => PageSize);
            }
        }
        private int _pageSize = 100;

        public int ProcessId
        {
            get { return _processId; }
            set
            {
                _processId = value;
                RaisePropertyChanged(() => ProcessId);
            }
        }
        private int _processId;

        private ObservableCollection<LocalizedItem> _items = new ObservableCollection<LocalizedItem>();
        public IEnumerable<LocalizedItem> Items { get { return _items; } }
        #endregion Properties

        #region Methods
        private void LoadItemsAsync()
        {
            // update processId so we can stop it if the user request a new batch before we finished this one
            int processId = ++ProcessId;
            Observable.Start(() =>
                {
                    // load items from database
                    int startId = StartId, pageSize = PageSize;
                    using (var provider = new MangosProvider())
                    {
                        var items = provider.GetItems()
                            .Where(o => o.Id >= startId)
                            .Take(pageSize)
                            .Select(o => new LocalizedItem { DatabaseItem = o });
                        return items.ToList();
                    }
                })
                .ObserveOnDispatcher()
                .Subscribe(result =>
                {
                    // check processId to make sure the user has not requested another batch meantime
                    if (processId != _processId)
                        return;

                    // display loaded items to user
                    _items.AddRange(result);
                    // update localized translation
                    GetItemsLocalesAsync(result, processId);
                }, OnError);
        }

        private void OnError(Exception ex)
        {
            throw ex;
        }

        private void GetItemsLocalesAsync(IEnumerable<LocalizedItem> items, int processId)
        {
            Observable.Start(() =>
                Parallel.ForEach(items, (item, ls) =>
                    {
                        var grabber = new WowheadClient(CultureInfo.CurrentCulture);
                        try
                        {
                            // if processId has changed, we stop this process
                            if (processId != ProcessId)
                                ls.Stop();

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
        }
        #endregion Commands
    }
}
