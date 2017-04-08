using System;
using System.Text;
using BinaryPuzzleSolver.Engine.Utilities;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// Wraps a puzzle surface, but with exactly one cell value that is different from the source surface.
    /// </summary>
    public sealed class SingleChangePuzzleSurface : IPuzzleSurface
    {
        private readonly IPuzzleSurface source;

        public SurfacePosition DeltaPosition { get; }

        public bool DeltaValue { get; }

        public int LineCount => source.LineCount;

        public int ColumnCount => source.ColumnCount;

        public bool IsSolved
        {
            get
            {
                for (int lineIndex = 0; lineIndex < LineCount; lineIndex++)
                {
                    if (!IsLineComplete(lineIndex))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public SingleChangePuzzleSurface(IPuzzleSurface source, SurfacePosition position, bool value)
        {
            Guard.NotNull(source, nameof(source));

            this.source = source;
            DeltaPosition = position;
            DeltaValue = value;
        }

        public bool? GetCell(int lineIndex, int columnIndex)
        {
            if (lineIndex == DeltaPosition.LineIndex && columnIndex == DeltaPosition.ColumnIndex)
            {
                return DeltaValue;
            }
            return source.GetCell(lineIndex, columnIndex);
        }

        public void SetCell(int lineIndex, int columnIndex, bool value)
        {
            throw new NotSupportedException();
        }

        public bool IsLineComplete(int lineIndex)
        {
            if (lineIndex == DeltaPosition.LineIndex)
            {
                for (int columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
                {
                    bool? value = GetCell(lineIndex, columnIndex);
                    if (value == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            return source.IsLineComplete(lineIndex);
        }

        public bool IsColumnComplete(int columnIndex)
        {
            if (columnIndex == DeltaPosition.ColumnIndex)
            {
                for (int lineIndex = 0; lineIndex < LineCount; lineIndex++)
                {
                    bool? value = GetCell(lineIndex, columnIndex);
                    if (value == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            return source.IsColumnComplete(columnIndex);
        }

        public int GetCountInLine(int lineIndex, bool? value)
        {
            if (lineIndex == DeltaPosition.LineIndex)
            {
                int count = 0;
                for (int columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
                {
                    bool? cell = GetCell(lineIndex, columnIndex);
                    if (cell == value)
                    {
                        count++;
                    }
                }
                return count;
            }
            return source.GetCountInLine(lineIndex, value);
        }

        public int GetCountInColumn(int columnIndex, bool? value)
        {
            if (columnIndex == DeltaPosition.ColumnIndex)
            {
                int count = 0;
                for (int lineIndex = 0; lineIndex < LineCount; lineIndex++)
                {
                    bool? cell = GetCell(lineIndex, columnIndex);
                    if (cell == value)
                    {
                        count++;
                    }
                }
                return count;
            }
            return source.GetCountInColumn(columnIndex, value);
        }

        public bool HasChanges()
        {
            throw new NotSupportedException();
        }

        public void AcceptChanges()
        {
            throw new NotSupportedException();
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
