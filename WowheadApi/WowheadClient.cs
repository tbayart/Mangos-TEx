using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Framework.Helpers;
using Framework.MVVM;
using WowFramework.Helpers;
using WowheadApi.Grabbers;
using WowheadApi.Models;

namespace WowheadApi
{
    public class WowheadClient : NotificationObject
    {
        // itemset   :              http://www.wowhead.com/itemset=1196
        // entités   :              http://www.wowhead.com/object=3714
        // event     :              http://www.wowhead.com/event=181
        // faction   :              http://www.wowhead.com/faction=1067
        // hunterpet :              http://www.wowhead.com/pets=0
        // haut-fait :              http://www.wowhead.com/achievement=940
        // pet ability :            http://www.wowhead.com/petability=1062
        // mascotte de combat :     http://www.wowhead.com/npc=72098
        // métier :                 http://www.wowhead.com/skill=773
        // monnaies :               http://www.wowhead.com/currency=416
        // pnjs :                   http://www.wowhead.com/npc=15727
        // quêtes :                 http://www.wowhead.com/quest=9874
        // races :                  http://www.wowhead.com/race=22
        // Sets transmogrifiés :    http://www.wowhead.com/transmog-set=1308
        // sorts/glyphs :           http://www.wowhead.com/spell=86524
        // titres :                 http://www.wowhead.com/title=125
        // zones :                  http://www.wowhead.com/zone=5287

        #region Fields
        private string _baseUrl;
        private string _baseQuery;
        #endregion Fields

        #region Ctor
        public WowheadClient()
            : this(LocaleHelpers.DefaultCulture)
        {
        }

        public WowheadClient(CultureInfo locale)
        {
            CurrentLocale = locale;
        }
        #endregion Ctor

        #region Properties
        public CultureInfo CurrentLocale
        {
            get { return _currentLocale; }
            set
            {
                value = LocaleHelpers.ValidateCulture(value);
                _currentLocale = value;
                RaisePropertyChanged(() => CurrentLocale);
                CurrentLocaleChanged();
            }
        }
        private CultureInfo _currentLocale;
        #endregion Properties

        #region Private Methods
        private void CurrentLocaleChanged()
        {
            string localeToken = CurrentLocale.Name.Substring(0, 2);
            _baseUrl = string.Format("http://{0}.wowhead.com/", localeToken);
            _baseQuery = string.Concat(_baseUrl, "{0}={1}");
        }

        private string GetUrl(string type, int id)
        {
            return string.Format(_baseQuery, type, id);
        }
        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// Provides data about an individual item
        /// http://www.wowhead.com/item=99371
        /// </summary>
        /// <param name="id">The item id</param>
        /// <returns>The item</returns>
        public Item GetItem(int id)
        {
            // retrieve html data
            string address = GetUrl("item", id);
            string data = WebRequestHelpers.DownloadString(address);
            // create a grabber
            var grabber = new ItemGrabber();
            // extract item data
            return grabber.Extract(data, id);
        }
        #endregion Public Methods
    }
}
