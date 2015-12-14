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
using WowApi;
using Framework.Collections;

namespace MangosTEx.ViewModels
{
    public class WowApiDataType
    {
        public WowApiDataType(Type dataType)
        {
            System.Diagnostics.Debug.Assert(dataType.Namespace.StartsWith("WowApi.Models"));
            DataType = dataType;
        }

        public string Libelle { get { return DataType.Name; } }
        public Type DataType { get; private set; }
    }
    public class BattleNetLocalizationViewModel : ObservableViewModel
    {
        #region Ctor
        public BattleNetLocalizationViewModel()
        {
            SetCollectionView(null);
            SelectedDataType = ListDataType.First();
        }
        #endregion Ctor

        #region Properties
        public ICollectionView Data { get; private set; }

        public List<WowApiDataType> ListDataType
        {
            get { return _listDataType; }
        }
        private List<WowApiDataType> _listDataType = new List<WowApiDataType>
            {
                /*
                    select Id, subject, text from achievement_reward;
                    select entry, subject_loc2, text_loc2 from locales_achievement_reward;
                */
                new WowApiDataType(typeof(WowApi.Models.Achievement)),

                /*
                    select entry, name, description from item_template;
                    select entry, name_loc2, description_loc2 from locales_item;
                */
                new WowApiDataType(typeof(WowApi.Models.Item)),

                /*
                    select entry, Title, Details, Objectives, OfferRewardText, RequestItemsText, EndText, CompletedText, 
                        ObjectiveText1, ObjectiveText2, ObjectiveText3, ObjectiveText4 from quest_template;
                    select entry, Title_loc2, Details_loc2, Objectives_loc2, OfferRewardText_loc2, RequestItemsText_loc2, EndText_loc2, CompletedText_loc2,
                        ObjectiveText1_loc2, ObjectiveText2_loc2, ObjectiveText3_loc2, ObjectiveText4_loc2 from locales_quest;
                */
                new WowApiDataType(typeof(WowApi.Models.Quest)),
                new WowApiDataType(typeof(WowApi.Models.Recipe)),
                new WowApiDataType(typeof(WowApi.Models.Spell)),
                /**************************************************
                new WowApiDataType(typeof(WowApi.Models.BattlePetAbility)),
                new WowApiDataType(typeof(WowApi.Models.BattlePetSpecies)),
                new WowApiDataType(typeof(WowApi.Models.BattlePetStats)),
                new WowApiDataType(typeof(WowApi.Models.ItemSet)),
                new WowApiDataType(typeof(WowApi.Models.Talent)),
                new WowApiDataType(typeof(WowApi.Models.TalentGlyph)),
                new WowApiDataType(typeof(WowApi.Models.TalentSpec)),
                new WowApiDataType(typeof(WowApi.Models.TalentTier)),
                ***************************************************/
            };

        public WowApiDataType SelectedDataType
        {
            get { return _selectedDataType; }
            set
            {
                _selectedDataType = value;
                RaisePropertyChanged(() => SelectedDataType);
            }
        }
        private WowApiDataType _selectedDataType;

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

        private Properties.Settings Settings { get { return Properties.Settings.Default; } }
        #endregion Properties

        #region Methods
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == GetPropertyName(() => SelectedDataType))
            {
                Data = null;
                LoadDataAsync();
            }
        }

        private Action RefreshCollectionView;
        private void SetCollectionView(IEnumerable<WowApiLocalizedData> source)
        {
            Data = CollectionViewSource.GetDefaultView(source);
            if (Data != null)
            {
                Data.Filter = DataFilter;
                RaisePropertyChanged(() => Data);
                RefreshCollectionView = Data.Refresh;
            }
            else
            {
                RefreshCollectionView = () => { };
            }
        }

        // filter untranslated items
        private bool DataFilter(object obj)
        {
            WowApiLocalizedData item = (WowApiLocalizedData)obj;
            return FilterHideUntranslated(item);
        }
        private bool FilterHideUntranslated(WowApiLocalizedData item)
        {
            return HideTranslated == false || string.IsNullOrEmpty(item.Data);
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
                    var result = Enumerable.Empty<WowApiLocalizedData>();
                    using (var provider = new MangosProvider())
                    {
                        if(SelectedDataType.DataType == typeof(WowApi.Models.Achievement))
                        {
                            result = provider.GetAchievements(culture)
                                .Select(o => new WowApiLocalizedData { ItemId = o.Id });
                        }
                        else if (SelectedDataType.DataType == typeof(WowApi.Models.Item))
                        {
                            result = provider.GetItems(culture)
                               .Select(o => new WowApiLocalizedData { ItemId = o.Id });
                        }
                        else if (SelectedDataType.DataType == typeof(WowApi.Models.Quest))
                        {
                            result = provider.GetQuests(culture)
                                .Select(o => new WowApiLocalizedData { ItemId = o.Id });
                        }
                        return result.ToList();
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

        private void GetItemsLocalesAsync(IEnumerable<WowApiLocalizedData> items)
        {
            CultureInfo culture = Settings.LocalizationCulture;
            var apiKey = Services.ViewModels.SettingsViewModel.GetBattleNetApiKey();
            IDataProvider provider = DataProviderManager.GetHttpCachedProvider();
            var grabber = new WowApiClient(provider, culture, apiKey);
            Type dataType = SelectedDataType.DataType;
            int batchSize = 50; // for query throttling, this is the number of items per second we allow to be processed
            TimeSpan throttle = TimeSpan.FromMilliseconds(1000d / batchSize);
            Observable.Start(() =>
                {
                    var unprocessed = items.Where(o => o.Status == LocalizationStatus.Unprocessed);
                    while(unprocessed.Any())
                    {
                        var item = unprocessed.First();
                        try
                        {
                            item.Query = grabber.GetUrl(dataType, item.ItemId);
                            item.Data = provider.ProvideData(item.Query);
                        }
                        catch (Exception ex)
                        {
                            // if we got an error, keep it to investigate
                            item.Error = ex.Message;
                            break; // stop processing if any error
                        }
                        System.Threading.Thread.Sleep((int)throttle.TotalMilliseconds);
                    }
                });
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
            var items = selection.OfType<WowApiLocalizedData>().ToList();
            // then launch item translation process
            GetItemsLocalesAsync(items);
        }

        public ICommand UpdateDatabaseCommand { get; set; }
        private void UpdateDatabaseExecute(IList selection)
        {
            // retrieve selection with the right Type and create a copy
            var items = selection.OfType<WowApiLocalizedData>().ToList();
            //ServiceProvider.GetInstance<InteractionService>().UserChoice
            //UpdateDatabaseAsync(items);
        }
        #endregion Commands
    }
}
