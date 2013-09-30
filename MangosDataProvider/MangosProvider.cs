using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MangosData.Context;
using MangosModels;

namespace MangosDataProvider
{
    public class MangosProvider
    {
        #region Fields
        private MangosEntities _edm;
        #endregion Fields

        #region Ctor
        public MangosProvider()
        {
            _edm = new MangosEntities();
        }
        #endregion Ctor

        #region Items
        private static Func<item_template, Item> _getItem = o => new Item { Id = o.entry, Name = o.name, Description = o.description };
        private static Func<locales_item, Item>[] _getLoc =
        {
            o => new Item { Id = o.entry, Name = string.Empty, Description = string.Empty },
            o => new Item { Id = o.entry, Name = o.name_loc1, Description = o.description_loc1 },
            o => new Item { Id = o.entry, Name = o.name_loc2, Description = o.description_loc2 },
            o => new Item { Id = o.entry, Name = o.name_loc3, Description = o.description_loc3 },
            o => new Item { Id = o.entry, Name = o.name_loc4, Description = o.description_loc4 },
            o => new Item { Id = o.entry, Name = o.name_loc5, Description = o.description_loc5 },
            o => new Item { Id = o.entry, Name = o.name_loc6, Description = o.description_loc6 },
            o => new Item { Id = o.entry, Name = o.name_loc7, Description = o.description_loc7 },
            o => new Item { Id = o.entry, Name = o.name_loc8, Description = o.description_loc8 },
        };

        public IEnumerable<Item> GetItems()
        {
            var items = _edm.item_template
                .AsNoTracking()
                .Select(_getItem);

            return items;
        }

        public void ItemsLocale(IEnumerable<Item> items, CultureInfo culture)
        {
            int offset = WowFramework.Helpers.LocaleHelpers.GetOffset(culture);
            if (offset >= _getLoc.Length)
                offset = 0;

            Func<locales_item, Item> selector = _getLoc[offset];
            var entries = items.Select(o => o.Id).ToList();
            var locales = _edm.locales_item
                .AsNoTracking()
                //.Where(o => entries.Contains(o.entry))
                .Select(selector)
                .ToList()
                .AsParallel();

            items.AsParallel()
                .Join(locales, o => o.Id, o => o.Id, (item, loc) =>
                    {
                        item.LocalizedName = loc.Name;
                        item.LocalizedDescription = loc.Description;
                        return item;
                    })
                .ToList();
        }
        #endregion Items

        #region GameObjects
        public IEnumerable<GameObject> GetGameObjects()
        {
            var gameobjects = _edm.gameobject_template
                .AsNoTracking()
                .Select(o => new GameObject { Id = o.entry, Name = o.name, Type = o.type });

            return gameobjects;
        }
        #endregion GameObjects
    }
}
