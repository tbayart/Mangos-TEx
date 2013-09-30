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

namespace MangosTEx.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region EventHandlers
        private event EventHandler<LocaleItemEventArgs> UpdateItemLocaleEvent;
        private event EventHandler<LocaleGameObjectEventArgs> UpdateGameObjectLocaleEvent;
        #endregion EventHandlers

        #region Ctor
        public MainWindowViewModel()
        {
            InitializeCommands();

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

            // loading items
            //LoadItems();
            // loading game objects
            LoadGameObjects();
        }
        #endregion Ctor

        #region Properties
        public CultureInfo[] Locales
        {
            get { return _locales; }
            private set
            {
                _locales = value;
                RaisePropertyChanged(() => Locales);
            }
        }
        private CultureInfo[] _locales;

        public IEnumerable<MangosModels.Item> Items
        {
            get { return _items; }
            private set
            {
                _items = value;
                RaisePropertyChanged(() => Items);
            }
        }
        private IEnumerable<MangosModels.Item> _items;

        public IEnumerable<MangosModels.GameObject> GameObjects
        {
            get { return _gameObjects; }
            private set
            {
                _gameObjects = value;
                RaisePropertyChanged(() => GameObjects);
            }
        }
        private IEnumerable<MangosModels.GameObject> _gameObjects;
        #endregion Properties

        #region Methods
        private void OnError(Exception ex)
        {
            throw ex;
        }
        #endregion Methods

        #region Items Test Methods
        private void LoadItems()
        {
            Observable.Start(() =>
                {
                    // load a bunch of items from database
                    var provider = new MangosDataProvider.MangosProvider();
                    return provider.GetItems()
                        .Where(o => o.Id >= 900 && o.Id < 1000)
                        .AsEnumerable();
                })
                .ObserveOnDispatcher()
                .Subscribe(result =>
                {
                    Items = result;
                    UpdateItemLocaleEvent += OnUpdateItemLocale;
                    Task.Factory.StartNew(() => GetItemsLocales(result));
                }, OnError);
        }

        private void GetItemsLocales(IEnumerable<MangosModels.Item> items)
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
        #endregion Items Test Methods

        #region GameObjects Test Methods
        private void LoadGameObjects()
        {
            Observable.Start(() =>
                {
                    int minId = 0;
                    // load a bunch of objects from database
                    var provider = new MangosDataProvider.MangosProvider();
                    return provider.GetGameObjects()
                        .Where(o => o.Type == (int)MangosModels.DataTypes.GameObjectType.TEXT)
                        //.Where(o => o.Id > minId)
                        .Take(150)
                        .ToList();
                })
                .ObserveOnDispatcher()
                .Subscribe(result =>
                {
                    GameObjects = result;
                    UpdateGameObjectLocaleEvent += OnUpdateGameObjectLocale;
                    Task.Factory.StartNew(() => GetGameObjectsLocales(result));
                }, OnError);
        }

        private void GetGameObjectsLocales(IEnumerable<MangosModels.GameObject> gameObjects)
        {
            Parallel.ForEach(gameObjects, go =>
            {
                var grabber = new WowheadClient(CultureInfo.CurrentCulture);
                try
                {
                    WowheadApi.Models.GameObject loc = grabber.GetGameObject(go.Id);
                    UpdateGameObjectLocaleEvent.Invoke(this, new LocaleGameObjectEventArgs(loc));
                }
                catch { }
            });
            UpdateGameObjectLocaleEvent -= OnUpdateGameObjectLocale;
        }

        private void OnUpdateGameObjectLocale(object sender, LocaleGameObjectEventArgs e)
        {
            if (e.Arg == null)
                return;

            var go = GameObjects.FirstOrDefault(o => o.Id == e.Arg.Id);
            if (go != null)
            {
                int id = 0;
                go.LocalizedName = e.Arg.Name;
                go.RelatedData = e.Arg.RelatedData != null
                    ? e.Arg.RelatedData
                        .OfType<WowheadApi.Models.BookPage>()
                        .Select(o => new MangosModels.PageText { Id = ++id, Text = o.Text })
                    : null;
                //go.Error = e.Arg.Error;
            }
        }
        #endregion GameObjects Test Methods

        #region Commands
        private void InitializeCommands()
        {
        }
        #endregion Commands
    }
}
