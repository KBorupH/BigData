using HtmlAgilityPack;
using RestSharp;
using WebScrapper.WebScrappers;

namespace WebScrapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RestSharpClient restClient = new RestSharpClient();
            StaticWebScrapper staticScraper = new StaticWebScrapper();

            string xPath = "//li";

            RestClient client = restClient.GetRestSharpClient("https://dmi.dk");
            RestRequest requestType = restClient.SetRequestType();
            RestResponse response = restClient.ExecuteRequest(client, requestType);

            if (true /* response.IsSuccessful */) // For some reason response.IsSuccesful fails, even when content is present
            {
                HtmlDocument doc = staticScraper.GetHtmlDocument(response);
                HtmlNodeCollection nodes = staticScraper.SelectDocumentNodes(doc, xPath);

                foreach (HtmlNode node in nodes)
                {
                    Console.WriteLine(node.InnerText);
                }
            }
            else
            {
                Console.WriteLine("Response not succesful");
            }
        }
    }
}
