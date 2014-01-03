﻿using Framework.MVVM;

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
            if (DatabaseEntity != null && TranslatedEntity != null
            && string.IsNullOrEmpty(TranslatedEntity.Name) == false)
            {
                if (DatabaseEntity.Name == TranslatedEntity.Name
                && DatabaseEntity.Description == TranslatedEntity.Description)
                    return LocalizationStatus.Equal;

                return LocalizationStatus.NotEqual;
            }
            return LocalizationStatus.Untranslated;
        }
        #endregion Methods
    }
}
