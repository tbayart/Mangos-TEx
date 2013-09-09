using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MangosData.Context;

namespace MangosData
{
    public class MangosProvider
    {
        private MangosEntities _edm;


        public MangosProvider()
        {
            _edm = new MangosEntities();
        }

        private static Func<item_template, Models.Item> _item_Selector = o => new Models.Item { Id = o.entry, Name = o.name, Description = o.description };
        private static Func<locales_item, Models.Item> _locale0_Selector = o => new Models.Item { Id = o.entry, Name = string.Empty, Description = string.Empty };
        private static Func<locales_item, Models.Item> _locale1_Selector = o => new Models.Item { Id = o.entry, Name = o.name_loc1, Description = o.description_loc1 };
        private static Func<locales_item, Models.Item> _locale2_Selector = o => new Models.Item { Id = o.entry, Name = o.name_loc2, Description = o.description_loc2 };
        private static Func<locales_item, Models.Item> _locale3_Selector = o => new Models.Item { Id = o.entry, Name = o.name_loc3, Description = o.description_loc3 };
        private static Func<locales_item, Models.Item> _locale4_Selector = o => new Models.Item { Id = o.entry, Name = o.name_loc4, Description = o.description_loc4 };
        private static Func<locales_item, Models.Item> _locale5_Selector = o => new Models.Item { Id = o.entry, Name = o.name_loc5, Description = o.description_loc5 };
        private static Func<locales_item, Models.Item> _locale6_Selector = o => new Models.Item { Id = o.entry, Name = o.name_loc6, Description = o.description_loc6 };
        private static Func<locales_item, Models.Item> _locale7_Selector = o => new Models.Item { Id = o.entry, Name = o.name_loc7, Description = o.description_loc7 };
        private static Func<locales_item, Models.Item> _locale8_Selector = o => new Models.Item { Id = o.entry, Name = o.name_loc8, Description = o.description_loc8 };

        public IEnumerable<Models.Item> GetItems()
        {
            var items = _edm.item_template
                .Select(_item_Selector);

            return items;
        }

        public void ItemsLocale(IEnumerable<Models.Item> items, CultureInfo culture)
        {
            Func<locales_item, Models.Item> selector;
            switch (WowFramework.Helpers.LocaleHelpers.GetOffset(culture))
            {
                case 1: selector = _locale1_Selector; break;
                case 2: selector = _locale2_Selector; break;
                case 3: selector = _locale3_Selector; break;
                case 4: selector = _locale4_Selector; break;
                case 5: selector = _locale5_Selector; break;
                case 6: selector = _locale6_Selector; break;
                case 7: selector = _locale7_Selector; break;
                case 8: selector = _locale8_Selector; break;
                default: selector = _locale0_Selector; break;
            }

            var entries = items.Select(o => o.Id).ToList();
            var locales = _edm.locales_item
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
    }
}
