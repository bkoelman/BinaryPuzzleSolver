using BinaryPuzzleSolver.Engine.Utilities;

namespace BinaryPuzzleSolver.Engine.Extensions
{
    public static class PuzzleSurfaceExtensions
    {
        public static bool?[,] CopyCells(this IPuzzleSurface source)
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