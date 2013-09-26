using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using WowheadApi.Models;

namespace WowheadApi.Grabbers
{
    public class ItemGrabber
    {
        #region Fields
        private static string _idTagFormat = "_[{0}]";
        private static string _ttTag = ".tooltip";
        private static char[] _trimTokens = { '\'', ';', '\r', '\n' };
        private static Regex[] _cleanupRemove = 
            {
                new Regex(@"^<.*>$"),
                new Regex(@"\([0-9]+\.[0-9]+%\s@\sL[0-9]+\)"),
            };
        private static Regex _cleanupCompute = new Regex(@"\[[-0-9+*/\s]+\]");
        #endregion Fields

        #region Ctor
        public ItemGrabber()
        {
        }
        #endregion Ctor

        #region Methods
        private string Cleanup(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            Match match;
            // remove parasite strings
            foreach (Regex remove in _cleanupRemove)
            {
                for (; ; )
                {
                    match = remove.Match(value);
                    if (match.Success == false)
                        break;
                    value = value.Replace(match.Value, "");
                }
            }

            // compute undone calculations
            for (; ; )
            {
                match = _cleanupCompute.Match(value);
                if (match.Success == false)
                    break;

                string replace = match.Value;
                var computeResult = new System.Data.DataTable().Compute(match.Value.Trim(new[] { '[', ']' }), null);
                if (computeResult is double)
                    replace = Math.Ceiling((double)computeResult).ToString();
                value = value.Replace(match.Value, replace);
            }

            return value.Trim();
        }

        public Item Extract(string data, int id)
        {
            try
            {
                // looking for tooltip part
                string token = string.Format(_idTagFormat, id);
                int start = data.IndexOf(token + _ttTag);
                start = data.IndexOf("<", start);
                int end = data.IndexOf(token, start);
                data = data.Substring(start, end - start).Trim(_trimTokens);
                // tooltip can have multiple roots, we embed it between <body> tag
                data = string.Format("<body>{0}</body>", data).Replace("&nbsp;", " ");

                // the list of extracted values
                string[] extracted = new string[2];
                // reader for tooltip data
                using (var sr = new StringReader(data))
                {
                    using (var reader = XmlReader.Create(sr))
                    {
                        int index = 0;
                        string getValueEndElement = null;
                        while (reader.Read() && index < extracted.Length)
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    {
                                        string c = reader.GetAttribute("class");
                                        if (string.IsNullOrEmpty(c) == false && c[0] == 'q')
                                        {
                                            extracted[index] = string.Empty;
                                            getValueEndElement = reader.Name;
                                        }
                                        else if (string.IsNullOrEmpty(getValueEndElement) == false)
                                        {
                                            if (reader.Name != "small")
                                                extracted[index] = string.Empty;
                                        }
                                    } break;
                                case XmlNodeType.Text:
                                    if (string.IsNullOrEmpty(getValueEndElement) == false)
                                    {
                                        extracted[index] += reader.Value;
                                    }
                                    break;
                                case XmlNodeType.EndElement:
                                    if (reader.Name == getValueEndElement
                                    && string.IsNullOrEmpty(extracted[index]) == false)
                                    {
                                        getValueEndElement = null;
                                        extracted[index] = extracted[index].Trim(new[] { '\\', '"' }).Replace("\\'", "'").Trim();
                                        ++index;
                                    }
                                    break;
                            }
                        }
                    }
                }

                return new Item {
                    Id = id,
                    Name = extracted[0],
                    Description = Cleanup(extracted[1]),
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion Methods
    }
}
