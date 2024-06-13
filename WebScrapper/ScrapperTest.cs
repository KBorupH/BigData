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
using System.Data.SqlTypes;
using System.Xml;

namespace WebScrapper
{
    internal class ScrapperTest
    {
        public async Task<int> Start()
        {
            try
            {
                Uri baseUrl = new Uri("https://befound.pt/");

                RobotReader robot = await RobotReader.ReadRobotTxt(baseUrl);

                if (!robot.Allowed) return 0;

                Scrapper scrapper = new Scrapper();

                scrapper.PagesToScrape.Enqueue(baseUrl);

                if (robot.AllowedMode)
                {
                    foreach (var allowed in robot.AllowedPages)
                    {
                        scrapper.AllowedPages.Add(new Uri(baseUrl, allowed));
                    }
                }
                else
                {
                    foreach (var disallowed in robot.DisallowedPages)
                    {
                        scrapper.DisallowedPages.Add(new Uri(baseUrl, disallowed));
                    }
                }

                foreach (string sitemap in robot.Sitemaps)
                {
                    XmlTextReader reader = new XmlTextReader(new Uri(baseUrl, sitemap).AbsoluteUri);

                    while (reader.Read())
                    {
                        // Do some work here on the data.
                        Console.WriteLine(reader.Name);
                    }
                    Console.ReadLine();
                }

                var chromeOptions = new ChromeOptions();

                chromeOptions.AddArgument("headless");

                using (var driver = new ChromeDriver(chromeOptions))
                {
                    int i = 0;
                    while (scrapper.PagesToScrape.Count != 0 && i < scrapper.ScrapeLimit)
                    {

                    }

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

                return 0;
            }
            catch (Exception e)
            {
                return 1;
            }
        }
    }
}
