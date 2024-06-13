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

        public ConcurrentBag<Uri> AllowedPages = new ConcurrentBag<Uri>();
        public ConcurrentQueue<Uri> PagesToScrape = new ConcurrentQueue<Uri>();
        public ConcurrentBag<Uri> ScrapedPages = new ConcurrentBag<Uri>();
        public ConcurrentBag<Uri> DisallowedPages = new ConcurrentBag<Uri>();

    }
}
