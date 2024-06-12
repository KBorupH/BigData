using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    internal class Scrapper
    {
        public string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
        public int ScrapeLimit = 40;

        public ConcurrentBag<string> PagesToScrape = new ConcurrentBag<string>();
        public ConcurrentBag<string> DiscoveredPages = new ConcurrentBag<string>();
        public ConcurrentBag<string> ScrapedPages = new ConcurrentBag<string>();
        public ConcurrentBag<string> DisallowedPages = new ConcurrentBag<string>();

    }
}
