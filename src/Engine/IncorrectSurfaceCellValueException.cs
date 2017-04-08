using System;
using BinaryPuzzleSolver.Engine.Utilities;
using JetBrains.Annotations;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// The exception that is thrown when a puzzle's cell value differs from the answer.
    /// </summary>
    public sealed class IncorrectSurfaceCellValueException : Exception
    {
        [NotNull]
        public IPuzzleSurface SourceSurface { get; }

        [NotNull]
        public IPuzzleSurface AnswerSurface { get; }

        public int LineIndex { get; }

        public int ColumnIndex { get; }

        public override string Message
        {
            get
            {
                bool? sourceCell = SourceSurface.GetCell(LineIndex, ColumnIndex);
                bool? answerCell = AnswerSurface.GetCell(LineIndex, ColumnIndex);

                return $"Expected {answerCell} at cell ({LineIndex},{ColumnIndex}) instead of {sourceCell}.";
            }
        }

        public IncorrectSurfaceCellValueException([NotNull] IPuzzleSurface sourceSurface, [NotNull] IPuzzleSurface answerSurface,
            int lineIndex, int columnIndex, [CanBeNull] Exception innerException = null)
            : base(null, innerException)
        {
            Guard.NotNull(sourceSurface, nameof(sourceSurface));
            Guard.NotNull(answerSurface, nameof(answerSurface));

            SourceSurface = sourceSurface;
            AnswerSurface = answerSurface;
            LineIndex = lineIndex;
            ColumnIndex = columnIndex;
        }
    }
}
