using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using System.Diagnostics;
using WebScrapper.WebScrappers;

namespace WebScrapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ScrapperTest test = new ScrapperTest();
            int t = test.Start().Result;
            int r = Start().Result;
            Console.WriteLine(r);
        }


        static async Task<int> Start()
        {
            try
            {
                RestSharpClient restClient = new RestSharpClient();
                StaticWebScrapper staticScraper = new StaticWebScrapper();

                string baseUrl = "https://www.scrapingcourse.com/ecommerce/";
                string xPath = "//bdi";

                RobotReader robot = await RobotReader.ReadRobotTxt(new Uri(baseUrl));

                Scrapper scrapper = new Scrapper();

                foreach (var disallowed in robot.DisallowedPages)
                {
                    var testc = "document.query .innertext != ";
                }

                var chromeOptions = new ChromeOptions();

                chromeOptions.AddArgument("headless");

                using (var driver = new ChromeDriver(chromeOptions))
                {
                    driver.Navigate().GoToUrl(baseUrl);

                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                    //wait.Until(d => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete")); // Fix this

                    IWebElement temp = driver.FindElement(By.CssSelector("#weather-news-block"));

                    foreach (IWebElement item in temp.FindElements(By.TagName("p")))
                    {
                        Console.WriteLine(item.Text);
                    }
                }

                RestClient client = restClient.GetRestSharpClient(baseUrl);
                RestRequest requestType = restClient.SetRequestType();
                RestResponse response = restClient.ExecuteRequest(client, requestType);

                if (true /* response.IsSuccessful */) // For some reason response.IsSuccesful fails, even when content is present
                {
                    HtmlDocument doc = staticScraper.GetHtmlDocument(response);
                    HtmlNodeCollection nodes = staticScraper.SelectDocumentNodes(doc, xPath);
                    //HtmlNode nodes = staticScraper.SelectSingleNode(doc, xPath);

                    //foreach (HtmlNode node in nodes.ChildNodes)
                    //{
                    //    Console.WriteLine(node.InnerText);
                    //}

                    foreach (HtmlNode node in nodes)
                    {
                        Console.WriteLine(node.InnerText);
                    }
                }

                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}
