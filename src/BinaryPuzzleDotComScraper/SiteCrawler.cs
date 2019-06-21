using System;
using System.IO;
using System.Linq;
using System.Net;

namespace BinaryPuzzleDotComScraper
{
    public sealed class SiteCrawler
    {
        private const string UrlTemplate = "http://www.binarypuzzle.com/puzzles.php?size={0}&level={1}&nr={2}";
        private const string BaseFolder = @"..\..\..\..\Puzzles\BinaryPuzzles.com";

        public void Crawl()
        {
            string absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), BaseFolder);
            Directory.CreateDirectory(absoluteFolder);

            foreach (PuzzleDifficulty difficulty in Enum.GetValues(typeof(PuzzleDifficulty)))
            {
                foreach (int size in new[] { 6, 8, 10, 12, 14 })
                {
                    foreach (int number in Enumerable.Range(1, 100))
                    {
                        string url = string.Format(UrlTemplate, size, (int)difficulty, number);
                        string puzzlePath = absoluteFolder +
                            string.Format(@"\{0}\{1:00}x{1:00}\Puzzle{2:000}.xml", difficulty, size, number);

                        if (!File.Exists(puzzlePath))
                        {
                            string html;
                            WebRequest webRequest = WebRequest.Create(url);
                            using (WebResponse response = webRequest.GetResponseAsync().Result)
                            {
                                using (Stream stream = response.GetResponseStream())
                                {
                                    using (var reader = new StreamReader(stream))
                                    {
                                        html = reader.ReadToEnd();
                                    }
                                }
                            }

                            var parser = new PageParser(html);
                            var writer = new ExcelXmlPuzzleWriter();

                            string puzzleName = puzzlePath.Substring(absoluteFolder.Length);
                            Console.WriteLine("Writing " + puzzleName);

                            Directory.CreateDirectory(Path.GetDirectoryName(puzzlePath));
                            writer.Save(puzzlePath, parser.Puzzle, parser.Answer);
                        }
                    }
                }
            }
        }
    }
}
