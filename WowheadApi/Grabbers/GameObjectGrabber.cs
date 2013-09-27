using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using WowheadApi.Models;

namespace WowheadApi.Grabbers
{
    public class GameObjectGrabber
    {
        #region Fields
        private static string _idTagFormat = "g_objects[{0}]";
        #endregion Fields

        #region Ctor
        public GameObjectGrabber()
        {
        }
        #endregion Ctor

        #region Methods
        public GameObject Extract(string data, int id)
        {
            GameObject result = new GameObject { Id = id };
            try
            {
                // looking for id part
                string token = string.Format(_idTagFormat, id);
                int start = data.IndexOf(token);
                start = data.IndexOf("{", start);
                int end = data.IndexOf("}", start) + 1;
                data = data.Substring(start, end - start);

                if (string.IsNullOrEmpty(data))
                    throw new Exception("GameObject not found");

                JObject obj = JObject.Parse(data);
                foreach (var entry in obj)
                {
                    switch (entry.Key)
                    {
                        case "name":
                            result.Name = ((JValue)entry.Value).Value<string>();
                            break;
                        case "type":
                            {
                                if (((JValue)entry.Value).Value<int>() == 9)
                                    result.Type = "Book";
                            } break;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            return result;
        }
        #endregion Methods
    }
}
