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
        string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
        int scrapeLimit = 40;

        ConcurrentBag<string> pagesToScrape;
        ConcurrentBag<string> discoveredPages;
        ConcurrentBag<string> scrapedPages;
        ConcurrentBag<string> disallowedPages;

    }
}
