using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebScrapper
{
    internal class RobotReader
    {
        public bool Allowed = true;

        public bool AllowedMode = false;
        public List<string> AllowedPages = new List<string>();
        public List<string> DisallowedPages = new List<string>();

        public bool CrawlDelaySet = false;
        public int CrawlDelay = 0;

        public bool VisitTimeSet = false;
        public TimeSpan StartVisitTime = TimeSpan.MinValue;
        public TimeSpan EndVisitTime = TimeSpan.MaxValue;

        public bool RequestRateSet = false;
        public int RequestRate = 0;
        public int RequestInterval = 0;

        public List<string> Sitemaps = new List<string>();

        public static async Task<RobotReader> ReadRobotTxt(Uri url)
        {
            RobotReader robot = new RobotReader();

            Uri robotUri = new Uri(url, "robots.txt");
            string contentOfRobotTxt = "";
            try
            {

                contentOfRobotTxt = await new HttpClient().GetStringAsync(robotUri.AbsoluteUri);
            }
            catch (Exception e)
            {
                ;
            }

            bool relevantUserAgent = false;

            int lastUserAgentLine = 0;
            int index = 0;
            foreach (string line in contentOfRobotTxt.Split('\n'))
            {
                string cleanedLine = Regex.Replace(line, @"\t|\n|\r", "");

                if (cleanedLine.StartsWith("User-agent: *"))
                {
                    relevantUserAgent = true;
                    robot.Allowed = true;
                    lastUserAgentLine = index;
                }
                else if (cleanedLine.StartsWith("User-agent:"))
                {
                    if (index - 1 == lastUserAgentLine)
                    {
                        lastUserAgentLine = index;
                        continue;
                    }

                    if (!relevantUserAgent) robot.Allowed = false;
                    relevantUserAgent = false;
                }
                else if (relevantUserAgent)
                {
                    if (cleanedLine.StartsWith("Disallow:"))
                    {
                        robot.DisallowedPages.Add(cleanedLine.Remove(0, "Disallow:".Length).Trim());
                        if (cleanedLine.Equals("Disallow: /")) robot.Allowed = false;
                    }
                    else if (cleanedLine.StartsWith("Allow:"))
                    {
                        robot.AllowedMode = true;
                        robot.AllowedPages.Add(cleanedLine.Remove(0, "Allow:".Length).Trim());
                    }
                    else if (cleanedLine.StartsWith("Crawl-delay:"))
                    {
                        robot.CrawlDelaySet = true;
                        robot.CrawlDelay = int.Parse(cleanedLine.Remove(0, "Crawl-delay:".Length).Trim());
                    }
                    else if (cleanedLine.StartsWith("Visit-time:")) // EX: 1200-1430
                    {
                        robot.VisitTimeSet = true;
                        string[] times = cleanedLine.Remove(0, "Visit-time:".Length).Trim().Split('-');
                        robot.StartVisitTime = TimeSpan.ParseExact(times[0], "hmm", CultureInfo.InvariantCulture);
                        robot.EndVisitTime = TimeSpan.ParseExact(times[1], "hmm", CultureInfo.InvariantCulture);
                    }
                    else if (cleanedLine.StartsWith("Request-rate:")) // 1/5
                    {
                        robot.RequestRateSet = true;
                        string[] xy = cleanedLine.Remove(0, "Request-rate:".Length).Trim().Split('/');
                        robot.RequestRate = int.Parse(xy[0]);
                        robot.RequestInterval = int.Parse(xy[1]);
                    }
                }

                if (cleanedLine.StartsWith("Sitemap:"))
                {
                    robot.Sitemaps.Add(cleanedLine.Remove(0, "Sitemap:".Length).Trim());
                }

                index++;
            }

            if (robot.AllowedPages.Count > 0 && !robot.Allowed)
            {
                robot.Allowed = true;
            }

            return robot;
        }
    }
}
