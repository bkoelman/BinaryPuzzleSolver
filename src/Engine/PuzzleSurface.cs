using System;
using System.Collections.Generic;
using System.Text;
using BinaryPuzzleSolver.Engine.Utilities;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// Provides memory storage for the grid of cells that represents a binary puzzle.
    /// </summary>
    public class PuzzleSurface : IPuzzleSurface
    {
        private readonly bool?[,] cells;
        private bool hasChanges;

        // Performance optimization: do not re-count cells when nothing has changed.
        // Map structure: LineIndex or ColumnIndex => { unknownCount, zeroCount }
        private readonly Dictionary<int, KeyValuePair<int, int>> countInLineCache = new Dictionary<int, KeyValuePair<int, int>>();

        private readonly Dictionary<int, KeyValuePair<int, int>> countInColumnCache =
            new Dictionary<int, KeyValuePair<int, int>>();

        public int LineCount { get; }

        public int ColumnCount { get; }

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

        public PuzzleSurface(int rowCount, int columnCount)
            : this(new bool?[rowCount, columnCount])
        {
            if (rowCount % 2 != 0 || rowCount == 0)
            {
                throw new ArgumentException("rowCount must be even and nonzero.", nameof(rowCount));
            }
            if (columnCount % 2 != 0 || columnCount == 0)
            {
                throw new ArgumentException("columnCount must be even and nonzero.", nameof(columnCount));
            }
        }

        protected internal PuzzleSurface(bool?[,] cells)
        {
            Guard.NotNull(cells, nameof(cells));

            // No validation on cells here.
            // Assume that either factory or a derived class takes that responsibility.
            this.cells = cells;
            LineCount = this.cells.GetLength(0);
            ColumnCount = this.cells.GetLength(1);
        }

        public virtual bool? GetCell(int lineIndex, int columnIndex)
        {
            // Performance optimization: do not verify indexes in this extremely hot method (array will throw anyway).

            return cells[lineIndex, columnIndex];
        }

        public virtual void SetCell(int lineIndex, int columnIndex, bool value)
        {
            // Performance optimization: do not verify indexes in this extremely hot method (array will throw anyway).

            cells[lineIndex, columnIndex] = value;
            hasChanges = true;

            countInLineCache.Remove(lineIndex);
            countInColumnCache.Remove(columnIndex);
        }

        public bool IsLineComplete(int lineIndex)
        {
            EnsureCountInLineCache(lineIndex);

            return countInLineCache[lineIndex].Key == 0;
        }

        public bool IsColumnComplete(int columnIndex)
        {
            EnsureCountInColumnCache(columnIndex);

            return countInColumnCache[columnIndex].Key == 0;
        }

        public int GetCountInLine(int lineIndex, bool? value)
        {
            EnsureCountInLineCache(lineIndex);

            if (value == null)
            {
                return countInLineCache[lineIndex].Key;
            }
            if (value == false)
            {
                return countInLineCache[lineIndex].Value;
            }
            return ColumnCount - countInLineCache[lineIndex].Key - countInLineCache[lineIndex].Value;
        }

        public int GetCountInColumn(int columnIndex, bool? value)
        {
            EnsureCountInColumnCache(columnIndex);

            if (value == null)
            {
                return countInColumnCache[columnIndex].Key;
            }

            if (value == false)
            {
                return countInColumnCache[columnIndex].Value;
            }

            return LineCount - countInColumnCache[columnIndex].Key - countInColumnCache[columnIndex].Value;
        }

        public virtual bool HasChanges()
        {
            return hasChanges;
        }

        public virtual void AcceptChanges()
        {
            hasChanges = false;
        }

        public override string ToString()
        {
            return ToString(",");
        }

        public virtual string ToString(string lineSeparator)
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

        private void VerifyLineIndex(int lineIndex)
        {
            if (lineIndex < 0 || lineIndex >= LineCount)
            {
                throw new ArgumentOutOfRangeException(nameof(lineIndex), lineIndex,
                    "lineIndex must be greater than zero and less than the number of lines.");
            }
        }

        private void VerifyColumnIndex(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex,
                    "columnIndex must be greater than zero and less than the number of columns.");
            }
        }

        private void EnsureCountInLineCache(int lineIndex)
        {
            VerifyLineIndex(lineIndex);

            if (!countInLineCache.ContainsKey(lineIndex))
            {
                int unknownCount = 0;
                int zeroCount = 0;

                for (int columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
                {
                    bool? cell = GetCell(lineIndex, columnIndex);
                    if (cell == null)
                    {
                        unknownCount++;
                    }
                    else if (cell == false)
                    {
                        zeroCount++;
                    }
                }

                countInLineCache[lineIndex] = new KeyValuePair<int, int>(unknownCount, zeroCount);
            }
        }

        private void EnsureCountInColumnCache(int columnIndex)
        {
            VerifyColumnIndex(columnIndex);

            if (!countInColumnCache.ContainsKey(columnIndex))
            {
                int unknownCount = 0;
                int zeroCount = 0;

                for (int lineIndex = 0; lineIndex < LineCount; lineIndex++)
                {
                    bool? cell = GetCell(lineIndex, columnIndex);
                    if (cell == null)
                    {
                        unknownCount++;
                    }
                    else if (cell == false)
                    {
                        zeroCount++;
                    }
                }

                countInColumnCache[columnIndex] = new KeyValuePair<int, int>(unknownCount, zeroCount);
            }
        }
    }
}
