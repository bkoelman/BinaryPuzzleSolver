using System.Collections.Generic;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// Represents a puzzle surface that records cell assignments.
    /// </summary>
    public sealed class ChangeTrackingPuzzleSurface : PuzzleSurface
    {
        // Performance optimization: Setting capacity does not help; calculating the capacity from cells actually makes it slower.
        private readonly Dictionary<SurfacePosition, bool> changedCells = new Dictionary<SurfacePosition, bool>();

        public ChangeTrackingPuzzleSurface(bool?[,] cells)
            : base(cells)
        {
        }

        public override void SetCell(int lineIndex, int columnIndex, bool value)
        {
            base.SetCell(lineIndex, columnIndex, value);

            var position = new SurfacePosition(lineIndex, columnIndex);
            changedCells[position] = value;
        }

        public override void AcceptChanges()
        {
            base.AcceptChanges();
            changedCells.Clear();
        }

        public IEnumerable<KeyValuePair<SurfacePosition, bool>> GetChangedCells()
        {
            foreach (KeyValuePair<SurfacePosition, bool> pair in changedCells)
            {
                yield return new KeyValuePair<SurfacePosition, bool>(pair.Key, pair.Value);
            }
        }
    }
}
