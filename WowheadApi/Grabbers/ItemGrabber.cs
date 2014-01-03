using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using WowheadApi.Models;
using System.Collections.Generic;

namespace WowheadApi.Grabbers
{
    public class ItemGrabber : IGrabber<Item>
    {
        #region Fields
        private static string _idTagFormat = "_[{0}]";
        private static string _ttTag = ".tooltip";
        // regex to remove parasite strings like "<clic to read>" or "(0.99% @ L15)"
        private static Regex[] _cleanupRemove = 
            {
                new Regex(@"^<.*>$"),
                new Regex(@"\([0-9]+\.[0-9]+%\s@\sL[0-9]+\)"),
            };
        private static Dictionary<string, string> _replacements = new Dictionary<string, string>
            {
                { "\\'", "'" },
                { "&quot;", "\"" },
                { "&nbsp;", " " },
                { "&lt;", "<" },
                { "&gt;", ">" },
                { "[", "" },
                { "]", "" },
                { "(", "" },
                { ")", "" },
            };
        private static char[] _trimLabel = { '\\', '"', ' ', '\t', '\r', '\n' };
        private static Regex _compute = new Regex(@"([0-9]+(\s*[-+*/]\s*)+)+[0-9]+"); // regex to find undone calculations
        #endregion Fields

        #region Methods
        private string CleanupLabel(string value)
        {
            // replace values in data
            foreach (var rep in _replacements)
                value = value.Replace(rep.Key, rep.Value);

            return value.Trim(_trimLabel).Trim();
        }

        private string CleanupDescription(string value)
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
                match = _compute.Match(value);
                if (match.Success == false)
                    break;

                // use DataTable.Compute to compute calculation
                var computeResult = new System.Data.DataTable().Compute(match.Value, null);
                if (computeResult is double == false)
                    break; // break if compute failed

                string result = Math.Ceiling((double)computeResult).ToString();
                value = value.Replace(match.Value, result);
            }

            return CleanupLabel(value);
        }
        #endregion Methods

        #region IGrabber
        public Item Extract(string data, int id)
        {
            if (string.IsNullOrEmpty(data))
                throw new Exception("Item not found");

            // looking for tooltip part
            string token = string.Format(_idTagFormat, id);
            int start = data.IndexOf(token + _ttTag);
            start = data.IndexOf("<", start);
            // end of tooltip is start of next idtag
            int end = data.IndexOf(token, start);
            data = data.Substring(start, end - start);

            Item item = new Item { Id = id };
            // create an html document to parse data
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            // look for table tags
            var tables = doc.DocumentNode.Descendants("table").ToList();

            // name is in first table if any, or directly in data
            // and in a <b class="q"> node, but q can be followed by a number (q0, q1...)
            Func<HtmlNode, bool> isClass_q_Node = o => o.Attributes.Any(a => a.Name == "class" && a.Value.StartsWith("q"));
            item.Name = (tables.FirstOrDefault() ?? doc.DocumentNode).Descendants("b").Where(isClass_q_Node)
                .Select(o => CleanupLabel(o.InnerText))
                .FirstOrDefault();

            if (tables.Any())
            {
                // description is in last table and in a <span class="q"> node; again, q can be followed by a number
                var descNodes = tables.Last().Descendants("span").Where(isClass_q_Node);
                var qnode = descNodes.FirstOrDefault(o => o.Attributes["class"].Value == "q")
                    ?? descNodes.FirstOrDefault(); // if we can't find a class="q" span node, we take the first

                if (qnode != null)
                {
                    // check if the nod contains <a> node as child
                    qnode = qnode.Descendants("a").FirstOrDefault() ?? qnode;
                    // remove comments
                    var comments = qnode.ChildNodes.OfType<HtmlCommentNode>().ToList();
                    foreach (var c in comments)
                        qnode.ChildNodes.Remove(c);
                    // and finally, extract the description
                    item.Description = CleanupDescription(qnode.InnerText);
                }
            }

            return item;
        }
        #endregion IGrabber
    }
}
