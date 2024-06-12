using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    internal class RobotReader
    {
        public bool Allowed = true;

        public bool AllowedMode = false;
        public List<string> AllowedPages = new List<string>();
        public List<string> DisallowedPages = new List<string>();

        public int CrawlDelay = 0;

        public bool VisitTimeSet = false;
        public DateTime StartVisitTime = DateTime.MinValue;
        public DateTime EndVisitTime = DateTime.MaxValue;

        public bool RequestRateSet = false;
        public int RequestRate = 0;
        public int RequestInterval = 0;

        public List<string> Sitemaps = new List<string>();

        public static async Task<RobotReader> ReadRobotTxt(string url)
        {
            RobotReader robot = new RobotReader();

            Uri robotUri = new Uri(new Uri(url), "robots.txt");

            string contentOfRobotTxt = await new HttpClient().GetStringAsync(robotUri);

            bool relevantUserAgent = false;

            int lastUserAgentLine = 0;
            int index = 0;
            foreach (var line in contentOfRobotTxt.Split('\n'))
            {
                if (line.Equals("User-agent: *"))
                {
                    relevantUserAgent = true;
                    robot.Allowed = true;
                    lastUserAgentLine = index;
                }
                else if (line.StartsWith("User-agent:"))
                {
                    if (index - 1 ==  lastUserAgentLine) 
                    {
                        lastUserAgentLine = index;
                        continue; 
                    }

                    if (!relevantUserAgent) robot.Allowed = false;
                    relevantUserAgent = false;
                }
                else if (relevantUserAgent)
                {
                    if (line.StartsWith("Disallow:"))
                    {
                        robot.DisallowedPages.Add(line.Remove(0, "Disallow:".Length).Trim());
                        if (line.Equals("Disallow: /")) robot.Allowed = false;
                    }
                    else if (line.StartsWith("Allow:"))
                    {
                        robot.AllowedMode = true;
                        robot.AllowedPages.Add(line.Remove(0, "Allow:".Length).Trim());
                    }
                    else if (line.StartsWith("Sitemap:"))
                    {
                        robot.Sitemaps.Add(line.Remove(0, "Sitemap:".Length).Trim());
                    }
                    else if (line.StartsWith("Crawl-delay:"))
                    {
                        robot.CrawlDelay = int.Parse(line.Remove(0, "Crawl-delay:".Length).Trim());
                    }
                    else if (line.StartsWith("Visit-time:")) // EX: 1200-1430
                    {
                        robot.VisitTimeSet = true;
                        var times = line.Remove(0, "Visit-time:".Length).Trim().Split('-');
                        robot.StartVisitTime = DateTime.Parse(times[0]);
                        robot.EndVisitTime = DateTime.Parse(times[1]);
                    }
                    else if (line.StartsWith("Request-rate:")) // 1/5
                    {
                        robot.RequestRateSet = true;
                        var xy = line.Remove(0, "Request-rate:".Length).Trim().Split('/');
                        robot.RequestRate = int.Parse(xy[0]);
                        robot.RequestInterval = int.Parse(xy[1]);
                    }
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
