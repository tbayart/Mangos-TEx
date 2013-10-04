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
    public class GameObjectLocalizationViewModel : ObservableViewModel
    {
        #region EventHandlers
        private event EventHandler<LocaleGameObjectEventArgs> UpdateGameObjectLocaleEvent;
        #endregion EventHandlers

        #region Ctor
        public GameObjectLocalizationViewModel()
        {
            InitializeCommands();
            LoadGameObjects();
        }
        #endregion Ctor

        #region Properties
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
        
        private void OnError(Exception ex)
        {
            throw ex;
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
        #endregion Methods

        #region Commands
        private void InitializeCommands()
        {
        }
        #endregion Commands
    }
}
