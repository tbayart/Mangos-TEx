using Framework.MVVM;
using MangosTEx.Services.Models;

namespace MangosTEx.Models
{
    public class LocalizedItem : Localized<Item, Item>
    {
        #region Ctor
        public LocalizedItem(Item databaseItem)
        {
            DatabaseEntity = databaseItem;
            TranslatedEntity = new Item { Id = databaseItem.Id };
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
