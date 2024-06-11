using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper.WebScrappers
{
    internal class StaticWebScrapper
    {
        public HtmlDocument GetHtmlDocument(RestResponse response)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response.Content);
            return htmlDoc;
        }

        public HtmlNodeCollection SelectDocumentNodes(HtmlDocument htmlDoc, string xPath)
        {
            return htmlDoc.DocumentNode.SelectNodes(xPath);
        }
    }
}
