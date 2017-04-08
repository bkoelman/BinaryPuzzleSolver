using JetBrains.Annotations;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// Defines the grid of cells that represents a binary puzzle.
    /// </summary>
    public interface IPuzzleSurface
    {
        int LineCount { get; }

        int ColumnCount { get; }

        bool IsSolved { get; }

        [CanBeNull]
        bool? GetCell(int lineIndex, int columnIndex);

        void SetCell(int lineIndex, int columnIndex, bool value);

        bool IsLineComplete(int lineIndex);
        bool IsColumnComplete(int columnIndex);

        int GetCountInLine(int lineIndex, [CanBeNull] bool? value);
        int GetCountInColumn(int columnIndex, [CanBeNull] bool? value);

        bool HasChanges();
        void AcceptChanges();

        [NotNull]
        string ToString([NotNull] string lineSeparator);
    }
}
