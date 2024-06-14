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
using System.Collections;
using System.Text.RegularExpressions;

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

                if (robot.VisitTimeSet && robot.StartVisitTime < DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay < robot.EndVisitTime) return 0;

                Scrapper scrapper = new Scrapper();

                scrapper.PagesToScrape.Enqueue(baseUrl);

                for (int i = 0; i < robot.Sitemaps.Count; i++)
                {
                    string sitemap = robot.Sitemaps[i];

                    XmlTextReader reader = new XmlTextReader(new Uri(baseUrl, sitemap).AbsoluteUri);

                    while (reader.Read())
                    {
                        if (reader.Name.ToLower() == "loc")
                        {
                            reader.Read();
                            Uri urlFound = new Uri(reader.Value);
                            if (urlFound.AbsolutePath.ToLower().Contains("sitemap") && urlFound.AbsolutePath.ToLower().EndsWith(".xml"))
                            {
                                robot.Sitemaps.Add(urlFound.AbsoluteUri);
                            } 
                            else
                            {
                                scrapper.PagesToScrape.Enqueue(urlFound);
                            }
                        }
                    }

                }

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

                var chromeOptions = new ChromeOptions();

                chromeOptions.AddArgument("headless");

                DateTime lastReadTime = DateTime.MinValue;
                int requestRate = robot.RequestRateSet ? robot.RequestInterval / robot.RequestRate : 0;

                using (var driver = new ChromeDriver(chromeOptions))
                {
                    int i = 0;
                    while (scrapper.PagesToScrape.Count != 0 && i < scrapper.ScrapeLimit)
                    {
                        if (robot.VisitTimeSet && robot.StartVisitTime < DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay < robot.EndVisitTime) return 0;

                        int secondsPassed = 0;

                        do { secondsPassed = (DateTime.Now.TimeOfDay - lastReadTime.TimeOfDay).Seconds; } 
                        while ((robot.RequestRateSet && secondsPassed < requestRate) || (robot.CrawlDelaySet && secondsPassed < robot.CrawlDelay));

                        if (scrapper.PagesToScrape.TryDequeue(out var currentUrl))
                        {
                            if (robot.AllowedMode)
                            {
                                foreach (string allowedPage in robot.AllowedPages)
                                {
                                    string regexString = allowedPage.Replace(".", "[.]").Replace("/", "[/]").Replace("*", "[^/]*").Replace("$", "[$]");
                                    if (!Regex.Match(currentUrl.AbsoluteUri, regexString).Success) continue;
                                }
                            }
                            else
                            {
                                foreach (string disallowedPage in robot.DisallowedPages)
                                {
                                    string regexString = disallowedPage.Replace(".", "[.]").Replace("/", "[/]").Replace("*", "[^/]*").Replace("$", "[$]");
                                    if (Regex.Match(currentUrl.AbsoluteUri, regexString).Success) continue;
                                }
                            }

                            driver.Navigate().GoToUrl(currentUrl);

                            lastReadTime = DateTime.Now;

                            scrapper.ScrapedPages.Add(currentUrl);
                        }
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
