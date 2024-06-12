using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScrapper.WebScrappers;

namespace WebScrapper
{
    internal class ScrapperTest
    {
        public async Task<int> Start()
        {
            try
            {
                RestSharpClient restClient = new RestSharpClient();
                StaticWebScrapper staticScraper = new StaticWebScrapper();

                string baseUrl = "https://www.dmi.dk/";
                string xPath = "//p";

                RobotReader robot = await RobotReader.ReadRobotTxt(baseUrl);

                Scrapper scrapper = new Scrapper();

                foreach (var disallowed in robot.DisallowedPages)
                {
                    var test1 = "google.com/*/test";

                    var testc = "google.com/*/test/*/this";
                }

                var chromeOptions = new ChromeOptions();

                chromeOptions.AddArgument("headless");

                using (var driver = new ChromeDriver(chromeOptions))
                {
                    driver.Navigate().GoToUrl(baseUrl);

                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                    //wait.Until(d => ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector(\"div#c22116 > div > div > div > p\").textContent.trim() != \"\"")); // Fix this
                    wait.Until(d => ((IJavaScriptExecutor)driver).ExecuteScript("return (document.querySelector(\"div#c22116 > div > div > div > p\").Displayed)")); // Fix this

                    IWebElement temp = driver.FindElement(By.CssSelector("#weather-news-block"));

                    foreach (WebElement item in temp.FindElements(By.TagName("p")))
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
