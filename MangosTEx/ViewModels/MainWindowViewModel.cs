using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using MangosTEx.Grabbers;
using Framework.Helpers;
using System.Globalization;
using Framework.Commands;
using Framework.MVVM;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MangosTEx.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private class EventArgs<T> : EventArgs
        {
            public T Arg { get; protected set; }
        }

        private class MangosDataItemEventArgs : EventArgs<List<MangosData.Models.Item>>
        {
            public MangosDataItemEventArgs(List<MangosData.Models.Item> items) { Arg = items; }
        }

        private class LocaleItemEventArgs : EventArgs<MangosTEx.Grabbers.Models.Item>
        {
            public LocaleItemEventArgs(MangosTEx.Grabbers.Models.Item loc) { Arg = loc; }
        }

        private event EventHandler<MangosDataItemEventArgs> ItemsLoaded;
        private event EventHandler<LocaleItemEventArgs> ItemLocale;

        #region Ctor
        public MainWindowViewModel()
        {
            WowApi.WowApiClient c = new WowApi.WowApiClient(CultureInfo.CurrentCulture);
            //var character = c.GetCharacterData("Elune", "Kerenn", WowApi.WowApiCharacterDataField.All);
            //var character = c.GetCharacterData("trollbane", "Hayase", WowApi.WowApiCharacterDataField.All);
			
			// I downloaded character data to avoid querying API for debugging purpose
            var character = c.CharTest("data_test.json");

            this.ItemsLoaded += OnItemsLoaded;
            this.ItemLocale += OnItemLocale;
            Task.Factory.StartNew(LoadItems);

            //Locales = WowFramework.Helpers.LocaleHelpers.GetCultures();
            //provider.ItemsLocale(items, CultureInfo.GetCultureInfo("fr-fr"));

            // wowhead test items
            // http://fr.wowhead.com/item=756
            // http://fr.wowhead.com/item=728
            // http://fr.wowhead.com/object=191656

            //var c = new WowApi.WowApiClient();
            //WowApi.Models.Item item = c.GetItem(756);
        }

        void LoadItems()
        {
            var provider = new MangosData.MangosProvider();
            var items = provider.GetItems()
                .Where(o => o.Id > 700 && o.Id < 1000)
                .ToList();
            ItemsLoaded.Invoke(this, new MangosDataItemEventArgs(items));
        }

        private void OnItemsLoaded(object sender, MangosDataItemEventArgs e)
        {
            Items = e.Arg;
            Task.Factory.StartNew(() => GetLocales(e.Arg));
        }

        private void GetLocales(List<MangosData.Models.Item> items)
        {
            Parallel.ForEach(items, item =>
            {
                var grabber = new WowheadGrabber("fr");
                MangosTEx.Grabbers.Models.Item loc = grabber.GetItem(item.Id);
                ItemLocale.Invoke(this, new LocaleItemEventArgs(loc));
            });
        }

        private void OnItemLocale(object sender, LocaleItemEventArgs e)
        {
            if (e.Arg == null)
                return;

            var item = Items.FirstOrDefault(o => o.Id == e.Arg.Id);
            if (item != null)
            {
                item.LocalizedName = e.Arg.Name;
                item.LocalizedDescription = e.Arg.Description;
                item.Error = e.Arg.Error;
            }
        }
        #endregion Ctor

        #region Properties
        private CultureInfo[] _locales;
        public CultureInfo[] Locales
        {
            get { return _locales; }
            private set
            {
                _locales = value;
                RaisePropertyChanged(() => Locales);
            }
        }

        private List<MangosData.Models.Item> _items;
        public List<MangosData.Models.Item> Items
        {
            get { return _items; }
            private set
            {
                _items = value;
                RaisePropertyChanged(() => Items);
            }
        }

        #endregion Properties

        #region Commands
        private void InitializeCommands()
        {
            RefreshDateCommand = new DelegateCommand(OnRefreshDate);
            RefreshPersonsCommand = new DelegateCommand(OnRefreshPersons);
            DoNothingCommand = new DelegateCommand(OnDoNothing, CanExecuteDoNothing);
        }

        public ICommand RefreshDateCommand { get; private set; }
        private void OnRefreshDate()
        {
        }

        public ICommand RefreshPersonsCommand { get; private set; }
        private void OnRefreshPersons()
        {
        }

        public ICommand DoNothingCommand { get; private set; }
        private void OnDoNothing()
        {
        }
        private bool CanExecuteDoNothing()
        {
            return false;
        }
        #endregion Commands
    }
}