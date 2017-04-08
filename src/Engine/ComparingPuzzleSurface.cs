using System;
using BinaryPuzzleSolver.Engine.Utilities;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// A puzzle surface wrapper that verifies setting cell values by comparing to the specified puzzle answer. When a proposed
    /// cell value differs from the answer, an <see cref="IncorrectSurfaceCellValueException" /> is thrown.
    /// </summary>
    public sealed class ComparingPuzzleSurface : IPuzzleSurface
    {
        private readonly IPuzzleSurface source;
        private readonly IPuzzleSurface answer;

        public ComparingPuzzleSurface(IPuzzleSurface source, IPuzzleSurface answer)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(answer, nameof(answer));

            this.source = source;
            this.answer = answer;

            Verify();
        }

        public int LineCount => source.LineCount;

        public int ColumnCount => source.ColumnCount;

        public bool IsSolved => source.IsSolved;

        public bool? GetCell(int lineIndex, int columnIndex)
        {
            return source.GetCell(lineIndex, columnIndex);
        }

        public void SetCell(int lineIndex, int columnIndex, bool value)
        {
            bool? answerCell = answer.GetCell(lineIndex, columnIndex);
            if (answerCell != value)
            {
                throw new IncorrectSurfaceCellValueException(source, answer, lineIndex, columnIndex);
            }

            source.SetCell(lineIndex, columnIndex, value);
        }

        public bool IsLineComplete(int lineIndex)
        {
            return source.IsLineComplete(lineIndex);
        }

        public bool IsColumnComplete(int columnIndex)
        {
            return source.IsColumnComplete(columnIndex);
        }

        public int GetCountInLine(int lineIndex, bool? value)
        {
            return source.GetCountInLine(lineIndex, value);
        }

        public int GetCountInColumn(int columnIndex, bool? value)
        {
            return source.GetCountInColumn(columnIndex, value);
        }

        public bool HasChanges()
        {
            return source.HasChanges();
        }

        public void AcceptChanges()
        {
            source.AcceptChanges();
        }

        public string ToString(string lineSeparator)
        {
            return source.ToString(lineSeparator);
        }

        private void Verify()
        {
            if (source.LineCount != answer.LineCount || source.ColumnCount != answer.ColumnCount)
            {
                throw new InvalidOperationException("The source and answer surfaces must have the same size.");
            }

            for (int lineIndex = 0; lineIndex < source.LineCount; lineIndex++)
            {
                for (int columnIndex = 0; columnIndex < source.ColumnCount; columnIndex++)
                {
                    bool? answerCell = answer.GetCell(lineIndex, columnIndex);
                    if (answerCell == null)
                    {
                        throw new InvalidOperationException($"Missing value in answer surface at ({lineIndex},{columnIndex}).");
                    }

                    bool? sourceCell = source.GetCell(lineIndex, columnIndex);
                    if (sourceCell != null)
                    {
                        if (sourceCell.Value != answerCell.Value)
                        {
                            throw new IncorrectSurfaceCellValueException(source, answer, lineIndex, columnIndex);
                        }
                    }
                }
            }
        }
    }
}
