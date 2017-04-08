using BinaryPuzzleSolver.Engine.Utilities;
using JetBrains.Annotations;

namespace BinaryPuzzleSolver.Engine.Extensions
{
    public static class PuzzleSurfaceExtensions
    {
        [NotNull]
        [ItemCanBeNull]
        public static bool?[,] CopyCells([NotNull] this IPuzzleSurface source)
        {
            Guard.NotNull(source, nameof(source));

            var cells = new bool?[source.LineCount, source.ColumnCount];
            for (int lineIndex = 0; lineIndex < source.LineCount; lineIndex++)
            {
                for (int columnIndex = 0; columnIndex < source.ColumnCount; columnIndex++)
                {
                    bool? value = source.GetCell(lineIndex, columnIndex);
                    cells[lineIndex, columnIndex] = value;
                }
            }
            return cells;
        }
    }
}
