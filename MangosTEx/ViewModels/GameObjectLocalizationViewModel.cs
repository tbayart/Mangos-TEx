using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ApiCommon;
using Framework.MVVM;
using MangosTEx.Events;
using MangosTEx.Services;
using MangosTEx.Services.ApiDataProvider;
using WowheadApi;
using dbGameObject = MangosTEx.Services.Models.GameObject;
using dbPageText = MangosTEx.Services.Models.PageText;
using whBookPage = WowheadApi.Models.BookPage;

namespace MangosTEx.ViewModels
{
    public class GameObjectLocalizationViewModel : ObservableViewModel
    {
        #region EventHandlers
        private event EventHandler<LocaleGameObjectEventArgs> UpdateGameObjectLocaleEvent;
        #endregion EventHandlers

        #region Ctor
        public GameObjectLocalizationViewModel()
        {
            LoadGameObjects();
        }
        #endregion Ctor

        #region Properties
        public IEnumerable<dbGameObject> GameObjects
        {
            get { return _gameObjects; }
            private set
            {
                _gameObjects = value;
                RaisePropertyChanged(() => GameObjects);
            }
        }
        private IEnumerable<dbGameObject> _gameObjects;

        private Properties.Settings Settings { get { return Properties.Settings.Default; } }
        #endregion Properties

        #region Methods
        private void LoadGameObjects()
        {
            CultureInfo culture = Settings.DatabaseCulture;
            Observable.Start(() =>
                {
                    int minId = 0;
                    // load a bunch of objects from database
                    var provider = new MangosProvider();
                    return provider.GetGameObjects(culture)
                        .Where(o => o.Type == (int)MangosTEx.Services.DataTypes.GameObjectType.TEXT)
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

        private void OnError(Exception ex)
        {
            throw ex;
        }

        private void GetGameObjectsLocales(IEnumerable<MangosTEx.Services.Models.GameObject> gameObjects)
        {
            CultureInfo culture = Settings.LocalizationCulture;
            IDataProvider provider = DataProviderManager.GetSimpleHttpProvider();
            Parallel.ForEach(gameObjects, go =>
            {
                var grabber = new WowheadClient(provider, culture);
                try
                {
                    var loc = grabber.GetGameObject(go.Id);
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
                        .OfType<whBookPage>()
                        .Select(o => new dbPageText { Id = ++id, Text = o.Text })
                    : null;
                //go.Error = e.Arg.Error;
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
