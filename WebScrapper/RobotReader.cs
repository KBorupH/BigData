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
        public bool allowed = true;

        public bool AllowedMode = false;
        public List<string> allowedPages = new List<string>();
        public List<string> disallowedPages = new List<string>();

        public int CrawlDelay = 0;

        public bool VisitTimeSet = false;
        public DateTime StartVisitTime = DateTime.MinValue;
        public DateTime EndVisitTime = DateTime.MinValue;

        public bool RequestRateSet = false;
        public int RequestRate = 0;
        public int RequestInterval = 0;

        public bool SitemapIncluded = false;
        public List<string> sitemap = new List<string>();

        public RobotReader(string url)
        {
            ReadRobotTxt(url);
        }

        private async void ReadRobotTxt(string url)
        {
            var disallowedPages = new ConcurrentBag<string>();

            url = Path.Combine(url, "robot.txt");

            string contentOfRobotTxt = await new HttpClient().GetStringAsync(url);

            bool relevantUserAgent = false;

            foreach (var line in contentOfRobotTxt.Split('\n'))
            {
                if (line.Equals("User-agent: *")) relevantUserAgent = true;

                if (relevantUserAgent)
                {
                    if (line.StartsWith("Disallow:"))
                    {
                        disallowedPages.Add(line.Remove(0, "Disallow:".Length).Trim());
                    }
                }

            }
        }
    }
}
