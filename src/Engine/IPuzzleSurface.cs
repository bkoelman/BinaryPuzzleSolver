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

        bool? GetCell(int lineIndex, int columnIndex);
        void SetCell(int lineIndex, int columnIndex, bool value);

        bool IsLineComplete(int lineIndex);
        bool IsColumnComplete(int columnIndex);

        int GetCountInLine(int lineIndex, bool? value);
        int GetCountInColumn(int columnIndex, bool? value);

        bool HasChanges();
        void AcceptChanges();

        string ToString(string lineSeparator);
    }
}