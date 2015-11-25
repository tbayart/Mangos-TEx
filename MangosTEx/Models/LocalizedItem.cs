using Framework.MVVM;

namespace MangosTEx.Models
{
    public class LocalizedItem : Localized<MangosTEx.Services.Models.Item, WowheadApi.Models.Item>
    {
        #region Ctor
        public LocalizedItem(MangosTEx.Services.Models.Item databaseItem)
        {
            DatabaseEntity = databaseItem;
            TranslatedEntity = new WowheadApi.Models.Item { Id = databaseItem.Id };
        }
        #endregion Ctor

        #region Methods
        protected override LocalizationStatus GetStatus()
        {
            if (TranslatedEntity == null || string.IsNullOrEmpty(TranslatedEntity.Name) == true)
            {
                if (string.IsNullOrEmpty(DatabaseEntity.Name) == true)
                    return LocalizationStatus.Untranslated;
                return LocalizationStatus.Unprocessed;
            }

            if (DatabaseEntity.Name == TranslatedEntity.Name
            && DatabaseEntity.Description == TranslatedEntity.Description)
                return LocalizationStatus.Equal;

            return LocalizationStatus.NotEqual;
        }
        #endregion Methods
    }
}
