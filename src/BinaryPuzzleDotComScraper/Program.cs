using System;

namespace BinaryPuzzleDotComScraper
{
    internal static class Program
    {
        public static void Main()
        {
            var crawler = new SiteCrawler();
            crawler.Crawl();

            Console.WriteLine();
            Console.WriteLine("All puzzles downloaded. Press any key to close.");
            Console.ReadKey();
        }
    }
}
