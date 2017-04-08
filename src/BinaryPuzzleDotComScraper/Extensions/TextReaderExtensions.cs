using System.Collections.Generic;
using System.IO;
using BinaryPuzzleSolver.Engine.Utilities;
using JetBrains.Annotations;

namespace BinaryPuzzleDotComScraper.Extensions
{
    public static class TextReaderExtensions
    {
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<string> GetLines([NotNull] this TextReader source)
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
