using Framework.MVVM;

namespace MangosTEx.Models
{
    public class LocalizedQuest : Localized<MangosTEx.Services.Models.Quest, WowheadApi.Models.Quest>
    {
        #region Ctor
        public LocalizedQuest(MangosTEx.Services.Models.Quest dbQuest)
        {
            DatabaseEntity = dbQuest;
            TranslatedEntity = new WowheadApi.Models.Quest { Id = dbQuest.Id };
        }
        #endregion Ctor

        #region Methods
        protected override LocalizationStatus GetStatus()
        {
            if (DatabaseEntity != null && TranslatedEntity != null
            && string.IsNullOrEmpty(TranslatedEntity.Title) == false)
            {
                if (DatabaseEntity.Title == TranslatedEntity.Title)
                    return LocalizationStatus.Equal;

                return LocalizationStatus.NotEqual;
            }
            return LocalizationStatus.Untranslated;
        }
        #endregion Methods
    }
}
