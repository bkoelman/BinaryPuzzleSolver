using System.Collections.Generic;
using System.IO;
using BinaryPuzzleSolver.Engine.Utilities;

namespace BinaryPuzzleDotComScraper.Extensions
{
    public static class TextReaderExtensions
    {
        public static IEnumerable<string> GetLines(this TextReader source)
        {
            Guard.NotNull(source, nameof(source));

            string line;
            while ((line = source.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}