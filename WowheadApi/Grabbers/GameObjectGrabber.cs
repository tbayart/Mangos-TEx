using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using WowheadApi.Models;

namespace WowheadApi.Grabbers
{
    public class GameObjectGrabber : IGrabber<GameObject>
    {
        #region Fields
        private static string _idTagFormat = "g_objects[{0}]";
        #endregion Fields

        #region IGrabber
        public GameObject Extract(string data, int id)
        {
            if (string.IsNullOrEmpty(data))
                throw new Exception("GameObject not found");

            // looking for id part
            string token = string.Format(_idTagFormat, id);
            int start = data.IndexOf(token);
            start = data.IndexOf("{", start);
            int end = data.IndexOf("}", start) + 1;
            string dataTooltip = data.Substring(start, end - start);

            // instanciate GameObject
            GameObject result = new GameObject { Id = id };
            // parse data
            JObject obj = JObject.Parse(dataTooltip);
            // extract name
            result.Name = obj["name"].Value<string>();

            // look for book pages
            start = data.IndexOf("new Book");
            if (start > 0)
            {
                start = data.IndexOf("{", start);
                end = data.IndexOf("}", start) + 1;
                string dataBook = data.Substring(start, end - start);
                obj = JObject.Parse(dataBook);
                result.RelatedData = obj["pages"].Values()
                    .Select(o => new BookPage { Text = o.Value<string>() })
                    .ToList();
            }

            return result;
        }
        #endregion IGrabber
    }
}
