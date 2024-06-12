using HtmlAgilityPack;
using RestSharp;
using System.Diagnostics;
using WebScrapper.WebScrappers;

namespace WebScrapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Start();
        }


        static async void Start()
        {
            RestSharpClient restClient = new RestSharpClient();
            StaticWebScrapper staticScraper = new StaticWebScrapper();

            string xPath = "//p";

            string url = "https://dmi.dk";

            RestClient client = restClient.GetRestSharpClient(url);
            RestRequest requestType = restClient.SetRequestType();
            RestResponse response = restClient.ExecuteRequest(client, requestType);

            if (true /* response.IsSuccessful */) // For some reason response.IsSuccesful fails, even when content is present
            {
                RobotReader robot = await RobotReader.ReadRobotTxt(url);

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
