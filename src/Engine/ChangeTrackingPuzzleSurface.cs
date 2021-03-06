﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// Represents a puzzle surface that records cell assignments.
    /// </summary>
    public sealed class ChangeTrackingPuzzleSurface : PuzzleSurface
    {
        // Performance optimization: Setting capacity does not help; calculating the capacity from cells actually makes it slower.
        [NotNull]
        private readonly Dictionary<SurfacePosition, bool> changedCells = new Dictionary<SurfacePosition, bool>();

        public ChangeTrackingPuzzleSurface([NotNull] [ItemCanBeNull] bool?[,] cells)
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

        [NotNull]
        public IEnumerable<KeyValuePair<SurfacePosition, bool>> GetChangedCells()
        {
            foreach (KeyValuePair<SurfacePosition, bool> pair in changedCells)
            {
                yield return new KeyValuePair<SurfacePosition, bool>(pair.Key, pair.Value);
            }
        }
    }
}
