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

        public RobotReader(string url)
        {
            ReadRobotTxt(url);
        }

        private async void ReadRobotTxt(string url)
        {
            url = Path.Combine(url, "robot.txt");

            string contentOfRobotTxt = await new HttpClient().GetStringAsync(url);

            bool relevantUserAgent = false;

            int lastUserAgentLine = 0;
            int index = 0;
            foreach (var line in contentOfRobotTxt.Split('\n'))
            {
                if (line.Equals("User-agent: *"))
                {
                    relevantUserAgent = true;
                    Allowed = true;
                    lastUserAgentLine = index;
                }
                else if (line.StartsWith("User-agent:"))
                {
                    if (index - 1 ==  lastUserAgentLine) 
                    {
                        lastUserAgentLine = index;
                        continue; 
                    }

                    if (!relevantUserAgent) Allowed = false;
                    relevantUserAgent = false;
                }


                if (relevantUserAgent)
                {
                    if (line.StartsWith("Disallow:"))
                    {
                        DisallowedPages.Add(line.Remove(0, "Disallow:".Length).Trim());
                        if (line.Equals("Disallow: /")) Allowed = false;
                    }

                    if (line.StartsWith("Allow:"))
                    {
                        AllowedMode = true;
                        AllowedPages.Add(line.Remove(0, "Allow:".Length).Trim());
                    }

                    if (line.StartsWith("Sitemap:"))
                    {
                        Sitemaps.Add(line.Remove(0, "Sitemap:".Length).Trim());
                    }

                    if (line.StartsWith("Crawl-delay:"))
                    {
                        CrawlDelay = int.Parse(line.Remove(0, "Crawl-delay:".Length).Trim());
                    }

                    if (line.StartsWith("Visit-time:"))
                    {
                        VisitTimeSet = true;
                        var times = line.Remove(0, "Visit-time:".Length).Trim().Split('-');
                        StartVisitTime = DateTime.Parse(times[0]);
                        EndVisitTime = DateTime.Parse(times[1]);
                    }

                    if (line.StartsWith("Request-rate:"))
                    {
                        RequestRateSet = true;
                        var xy = line.Remove(0, "Request-rate:".Length).Trim().Split('/');
                        RequestRate = int.Parse(xy[0]);
                        RequestInterval = int.Parse(xy[1]);
                    }
                }

                index++;
            }

            if (AllowedPages.Count > 0 && !Allowed) 
            {
                Allowed = true;
            }
        }
    }
}
