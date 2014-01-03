using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using WowheadApi.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace WowheadApi.Grabbers
{
    public class QuestGrabber : IGrabber<Quest>
    {
        #region Fields
        private static string _idTagFormat = "_[{0}]";
        #endregion Fields

        #region Methods
        private HtmlNode NextNode(HtmlNode node, string name)
        {
            for (node = node.NextSibling;
                node != null && node.Name != name;
                node = node.NextSibling) ;
            return node;
        }

        private string GetTextFromH3(HtmlNode node)
        {
            string result = string.Empty;
            HtmlNode endNode = NextNode(node, "h3");
            while (node.NextSibling != endNode)
            {
                node = node.NextSibling;
                switch (node.Name)
                {
                    case "br":
                        result += "$B";
                        break;
                    default:
                        result += node.InnerText;
                        break;
                }
            }
            return CleanText(result);
        }

        private string CleanText(string text)
        {
            return text.Trim().Replace("&lt;", "<").Replace("&gt;", ">").Replace("&nbsp;", " ");
        }
        #endregion Methods

        #region IGrabber
        public Quest Extract(string data, int id)
        {
            if (string.IsNullOrEmpty(data))
                throw new Exception("Quest not found");

            // looking for id part
            string token = string.Format(_idTagFormat, id);
            int start = data.IndexOf(token);
            start = data.IndexOf("{", start);
            int end = data.IndexOf("}", start) + 1;
            string dataTooltip = data.Substring(start, end - start);

            Quest result = new Quest { Id = id };
            // parse data
            JObject obj = JObject.Parse(dataTooltip);
            // extract title
            result.Title = obj.First.First.Value<string>();

            // create an html document to parse data
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            // locate title in h1 tag
            var titleNode = doc.DocumentNode.Descendants("h1").Where(o => o.InnerText == result.Title).FirstOrDefault();
            if (titleNode == null)
                throw new Exception("Unable to find title in document");

            HtmlNode node = titleNode.NextSibling;
            while (node != null)
            {
                if (node.InnerHtml.Contains("-progress'"))
                    result.RequestItemsText = GetTextFromH3(node);
                else if (node.InnerHtml.Contains("-completion'"))
                    result.OfferRewardText = GetTextFromH3(node);
                node = node.NextSibling;
            }

            // next node is the reward text
            node = NextNode(titleNode.NextSibling, "table");
            result.Objectives = CleanText(node.InnerText);
            // look for objective text
            node = node.NextSibling;
            if (node.Name == "table")
                result.ObjectiveText1 = CleanText(node.ChildNodes[1].ChildNodes[1].InnerText);
            // details
            node = NextNode(node, "h3");
            result.Details = GetTextFromH3(node);
            // remaining data
            for (; ; )
            {
                node = NextNode(node, "h3");
                if (node == null)
                    break;

            }

            return result;
        }
        #endregion IGrabber
    }
}
