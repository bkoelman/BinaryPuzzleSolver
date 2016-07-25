using System.Text;
using BinaryPuzzleSolver.Engine.Utilities;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// Wraps a puzzle surface and exposes it rotated by 90 degrees. This effectively switches lines and columns.
    /// </summary>
    public sealed class PuzzleRotatedSurface : IPuzzleSurface
    {
        private readonly IPuzzleSurface source;

        public int LineCount => source.ColumnCount;

        public int ColumnCount => source.LineCount;

        public bool IsSolved => source.IsSolved;

        public PuzzleRotatedSurface(IPuzzleSurface source)
        {
            Guard.NotNull(source, nameof(source));

            this.source = source;
        }

        public bool? GetCell(int lineIndex, int columnIndex)
        {
            return source.GetCell(columnIndex, lineIndex);
        }

        public void SetCell(int lineIndex, int columnIndex, bool value)
        {
            source.SetCell(columnIndex, lineIndex, value);
        }

        public bool IsLineComplete(int lineIndex)
        {
            return source.IsColumnComplete(lineIndex);
        }

        public bool IsColumnComplete(int columnIndex)
        {
            return source.IsLineComplete(columnIndex);
        }

        public int GetCountInLine(int lineIndex, bool? value)
        {
            return source.GetCountInColumn(lineIndex, value);
        }

        public int GetCountInColumn(int columnIndex, bool? value)
        {
            return source.GetCountInLine(columnIndex, value);
        }

        public bool HasChanges()
        {
            return source.HasChanges();
        }

        public void AcceptChanges()
        {
            source.AcceptChanges();
        }

        public override string ToString()
        {
            return ToString(",");
        }

        public string ToString(string lineSeparator)
        {
            int lineCount = LineCount;
            int columnCount = ColumnCount;

            var sb = new StringBuilder();
            for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
            {
                if (sb.Length > 0)
                {
                    sb.Append(lineSeparator);
                }

                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    bool? value = GetCell(lineIndex, columnIndex);
                    sb.Append(value == true ? "1" : value == false ? "0" : "-");
                }
            }
            return sb.ToString();
        }
    }
}