using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Framework.MVVM;
using MangosTEx.Events;
using WowheadApi;

namespace MangosTEx.ViewModels
{
    public class ItemLocalizationViewModel : ObservableViewModel
    {
        #region EventHandlers
        private event EventHandler<LocaleItemEventArgs> UpdateItemLocaleEvent;
        #endregion EventHandlers

        #region Ctor
        public ItemLocalizationViewModel()
        {
            InitializeCommands();
            LoadItems();
        }
        #endregion Ctor

        #region Properties
        public IEnumerable<MangosTEx.Services.Models.Item> Items
        {
            get { return _items; }
            private set
            {
                _items = value;
                RaisePropertyChanged(() => Items);
            }
        }
        private IEnumerable<MangosTEx.Services.Models.Item> _items;
        #endregion Properties

        #region Methods
        private void LoadItems()
        {
            Observable.Start(() =>
                {
                    // load a bunch of items from database
                    var provider = new MangosTEx.Services.MangosProvider();
                    return provider.GetItems()
                        .Where(o => o.Id >= 900 && o.Id < 1000)
                        .ToList();
                })
                .ObserveOnDispatcher()
                .Subscribe(result =>
                {
                    Items = result;
                    UpdateItemLocaleEvent += OnUpdateItemLocale;
                    Task.Factory.StartNew(() => GetItemsLocales(result));
                }, OnError);
        }

        private void OnError(Exception ex)
        {
            throw ex;
        }

        private void GetItemsLocales(IEnumerable<MangosTEx.Services.Models.Item> items)
        {
            Parallel.ForEach(items, item =>
            {
                var grabber = new WowheadClient(CultureInfo.GetCultureInfo("zh-TW"));
                try
                {
                    WowheadApi.Models.Item loc = grabber.GetItem(item.Id);
                    UpdateItemLocaleEvent.Invoke(this, new LocaleItemEventArgs(loc));
                }
                catch { }
            });
            UpdateItemLocaleEvent -= OnUpdateItemLocale;
        }

        private void OnUpdateItemLocale(object sender, LocaleItemEventArgs e)
        {
            if (e.Arg == null)
                return;

            var item = Items.FirstOrDefault(o => o.Id == e.Arg.Id);
            if (item != null)
            {
                item.LocalizedName = e.Arg.Name;
                item.LocalizedDescription = e.Arg.Description;
                //item.Error = e.Arg.Error;
            }
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
