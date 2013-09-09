using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Framework.Helpers;
using MangosTEx.Grabbers.Models;

namespace MangosTEx.Grabbers
{
    public class WowheadGrabber
    {
        private string _baseUrl;
        private string _baseQuery;

        public WowheadGrabber(string langage)
        {
            _baseUrl = string.Format("http://{0}.wowhead.com/", langage);
            _baseQuery = string.Concat(_baseUrl, "{0}={1}");
        }

        public Item GetItem(int id)
        {
            // retrieve html data
            string address = string.Format(_baseQuery, "item", id);
            string data = WebRequestHelpers.DownloadString(address);

            Item item = new Item { Id = id };
            try
            {
                // looking for tooltip part
                var tt = string.Format("_[{0}].tooltip", id);
                int start = data.IndexOf(tt);
                start = data.IndexOf("<", start);
                tt = string.Format("_[{0}]", id);
                int end = data.IndexOf(tt, start);
                data = data.Substring(start, end - start);
                // tooltip expedted to have multiple roots, we embed it between tags
                data = string.Format("<body>{0}</body>", data.Trim(new char[] { '\'', ';', '\r', '\n' }));
                data = data.Replace("&nbsp;", " ");

                List<string> nodes = new List<string>();
                // the list of extracted values
                string[] extracted = new string[10];
                // reader for tooltip data

                using (var reader = XmlReader.Create(new StringReader(data)))
                {
                    int index = 0, part = 0;
                    string getValueEndElement = null;
                    while (reader.Read() && index < extracted.Length)
                    {
                        string node = string.Format("{0}-{1}-{2}", reader.NodeType, reader.Name, reader.Value);
                        nodes.Add(node);

                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                {
                                    if (reader.Depth == 1)
                                        ++part;
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

                item.Name = extracted[0];
                item.Description = Cleanup(extracted[1]);
            }
            catch (Exception ex)
            {
                item.Error = ex.Message;
            }
            return item;
        }

        private static Regex[] _cleanupRemove = 
            {
                new Regex(@"^<.*>$"),
                new Regex(@"\([0-9]+\.[0-9]+%\s@\sL[0-9]+\)"),
            };
        private static Regex _cleanupCompute = new Regex(@"\[[-0-9+*/\s]+\]");

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
        //public Item GetItem(int id)
        //{
        //    // retrieve html data
        //    string address = string.Format(_baseQuery, "item", id);
        //    string data = WebRequestHelpers.DownloadString(address);

        //    Item item = new Item { Id = id };
        //    try
        //    {
        //        // looking for tooltip part
        //        var tt = string.Format("_[{0}].tooltip", id);
        //        int start = data.IndexOf(tt);
        //        start = data.IndexOf("<", start);
        //        tt = string.Format("_[{0}]", id);
        //        int end = data.IndexOf(tt, start);
        //        data = data.Substring(start, end - start);
        //        // tooltip expedted to have multiple roots, we embed it between tags
        //        data = string.Format("<body>{0}</body>", data.Trim(new char[] { '\'', ';', '\r', '\n' }));

        //        List<string> nodes = new List<string>();
        //        // the list of extracted values
        //        string[] extracted = new string[2];
        //        // reader for tooltip data
        //        var reader = XmlReader.Create(new StringReader(data));
        //        int index = 0;
        //        bool getNextData = true;
        //        while (reader.Read())
        //        {
        //            string node = string.Format("{0}-{1}-{2}", reader.NodeType, reader.Name, reader.Value);
        //            nodes.Add(node);

        //            if (getNextData == true && reader.HasValue && index < 2)
        //            {
        //                extracted[index] = reader.Value.Trim(new[] { '\\', '"' }).Replace("\\'", "'");
        //            }
        //            if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1 && reader.Name == "table")
        //            {
        //                getNextData = true;
        //            }
        //            if (reader.NodeType == XmlNodeType.EndElement && getNextData)
        //            {
        //                getNextData = false;
        //                ++index;
        //            }
        //        }

        //        item.Name = extracted[0];
        //        item.Description = extracted[1];
        //    }
        //    catch (Exception ex)
        //    {
        //        item.Error = ex.Message;
        //    }
        //    return item;
        //}
    }
}
