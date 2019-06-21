using System;
using System.IO;
using System.Linq;
using System.Net;
using BinaryPuzzleSolver.Engine;
using JetBrains.Annotations;

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
                        string puzzleRelativePath =
                            string.Format(@"{0}\{1:00}x{1:00}\Puzzle{2:000}.xml", difficulty, size, number);
                        string puzzleAbsolutePath = Path.Combine(absoluteFolder, puzzleRelativePath);

                        if (!File.Exists(puzzleAbsolutePath))
                        {
                            string url = string.Format(UrlTemplate, size, (int)difficulty, number);

                            string html = GetPuzzleHtml(url);
                            var parser = new PageParser(html);

                            Console.WriteLine("Writing " + puzzleRelativePath);
                            WritePuzzleToDisk(puzzleAbsolutePath, parser.Puzzle, parser.Answer);
                        }
                    }
                }
            }
        }

        [NotNull]
        private static string GetPuzzleHtml([NotNull] string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
            using (WebResponse response = webRequest.GetResponseAsync().Result)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        private static void WritePuzzleToDisk([NotNull] string puzzlePath, [NotNull] IPuzzleSurface puzzle,
            [NotNull] IPuzzleSurface answer)
        {
            var writer = new ExcelXmlPuzzleWriter();

            Directory.CreateDirectory(Path.GetDirectoryName(puzzlePath));
            writer.Save(puzzlePath, puzzle, answer);
        }
    }
}
