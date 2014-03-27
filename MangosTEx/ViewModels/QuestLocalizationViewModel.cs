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
using WowheadApi;
using dbQuest = MangosTEx.Services.Models.Quest;

namespace MangosTEx.ViewModels
{
    public class QuestLocalizationViewModel : ObservableViewModel
    {
        #region Ctor
        public QuestLocalizationViewModel()
        {
            SetCollectionView(null);
            LoadDataAsync();
        }
        #endregion Ctor

        #region Properties
        public ICollectionView Data { get; private set; }

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
        private void SetCollectionView(IEnumerable<LocalizedQuest> source)
        {
            Data = CollectionViewSource.GetDefaultView(source);
            if (Data != null)
            {
                Data.Filter = DataFilter;
                RefreshCollectionView = Data.Refresh;
            }
            else
            {
                RefreshCollectionView = () => { };
            }
            RaisePropertyChanged(() => Data);
        }

        private bool DataFilter(object obj)
        {
            LocalizedQuest quest = (LocalizedQuest)obj;
            return ShowErrorOnly ? string.IsNullOrEmpty(quest.Error) == false
                : FilterHideUntranslated(quest) && FilterHideMatchingTranslation(quest);
        }
        private bool FilterHideUntranslated(LocalizedQuest quest)
        {
            return HideUntranslated == false || quest.TranslatedEntity != null;
        }
        private bool FilterHideMatchingTranslation(LocalizedQuest quest)
        {
            return HideMatchingTranslation == false || quest.TranslatedEntity == null
                || quest.DatabaseEntity.Title != quest.TranslatedEntity.Title
                || quest.DatabaseEntity.Details != quest.TranslatedEntity.Details;
        }

        private int _loadProcessId;
        private void LoadDataAsync()
        {
            CultureInfo culture = Settings.DatabaseCulture;
            // get a processId so we can stop it if the user request a new batch before we finished this one
            int processId = ++_loadProcessId;
            SetCollectionView(null);
            Observable.Start(() =>
                {
                    // load items from database
                    using (var provider = new MangosProvider())
                    using (new PerformanceChecker("GetQuests"))
                    {
                        return provider.GetQuests(culture)
                            .Where(o => o.Id >= 9874 && o.Id <= 9888)
                            .Select(o => new LocalizedQuest(o))
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

        private void GetLocalesAsync(IEnumerable<LocalizedQuest> items)
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
                            item.TranslatedEntity = grabber.GetQuest(item.DatabaseEntity.Id);
                        }
                        catch (Exception ex)
                        {
                            // if we got an error, keep it to investigate
                            item.Error = ex.Message;
                        }
                    }));
        }

        private void UpdateDatabaseAsync(IEnumerable<LocalizedQuest> items)
        {
            CultureInfo culture = Settings.DatabaseCulture;
            Observable.Start(() =>
            {
                // select valid items and convert them to update database
                var dbItems = items
                    .Where(o => string.IsNullOrEmpty(o.Error) == false)
                    .Select(GetTranslatedDbItem);

                using (var provider = new MangosProvider())
                {
                    dbItems = provider.UpdateQuests(dbItems, culture)
                        .ToList();
                }
                items.Join(dbItems, o => o.DatabaseEntity.Id, o => o.Id, (li, dbi) => new { li, dbi })
                    .ToList()
                    .ForEach(o => o.li.DatabaseEntity = o.dbi);
            });
        }

        private dbQuest GetTranslatedDbItem(LocalizedQuest item)
        {
            return new dbQuest
                {
                    Id = item.TranslatedEntity.Id,
                    Title = item.TranslatedEntity.Title,
                    Details = item.TranslatedEntity.Details
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
            var items = selection.OfType<LocalizedQuest>().ToList();
            // then launch item translation process
            GetLocalesAsync(items);
        }

        public ICommand UpdateDatabaseCommand { get; set; }
        private void UpdateDatabaseExecute(IList selection)
        {
            // retrieve selection with the right Type and create a copy
            var items = selection.OfType<LocalizedQuest>().ToList();
            //ServiceProvider.GetInstance<InteractionService>().UserChoice
            UpdateDatabaseAsync(items);
        }
        #endregion Commands
    }
}
