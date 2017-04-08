using System.Collections.Generic;
using BinaryPuzzleSolver.Engine.Utilities;
using JetBrains.Annotations;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// Attempts to solve a binary puzzle using recursive brute-force.
    /// </summary>
    public sealed class BruteForcePuzzleSolver
    {
        public void Solve([NotNull] IPuzzleSurface surface)
        {
            Guard.NotNull(surface, nameof(surface));

            Stack<SurfacePosition> unknownPositionStack = GetUnknownPositions(surface);
            IPuzzleSurface solution = RecursiveTestUnknownPositionsOnStack(unknownPositionStack, surface);

            if (solution != null)
            {
                Flatten(surface, solution);
            }
        }

        private void Flatten([NotNull] IPuzzleSurface sourceSurface, [NotNull] IPuzzleSurface completedSurface)
        {
            foreach (SurfacePosition position in GetUnknownPositions(sourceSurface))
            {
                sourceSurface.SetCell(position.LineIndex, position.ColumnIndex,
                    completedSurface.GetCell(position.LineIndex, position.ColumnIndex).Value);
            }
        }

        [CanBeNull]
        private IPuzzleSurface RecursiveTestUnknownPositionsOnStack([NotNull] Stack<SurfacePosition> unknownPositions,
            [NotNull] IPuzzleSurface surface)
        {
            var validator = new SurfaceValidator(surface);
            bool isValid = validator.TryValidate();
            if (!isValid)
            {
                return null;
            }

            if (unknownPositions.Count == 0)
            {
                return surface;
            }

            SurfacePosition position = unknownPositions.Pop();

            var falseDelta = new SingleChangePuzzleSurface(surface, position, false);
            IPuzzleSurface falseSolution = RecursiveTestUnknownPositionsOnStack(unknownPositions, falseDelta);
            if (falseSolution != null)
            {
                return falseSolution;
            }

            var trueDelta = new SingleChangePuzzleSurface(surface, position, true);
            IPuzzleSurface trueSolution = RecursiveTestUnknownPositionsOnStack(unknownPositions, trueDelta);
            if (trueSolution != null)
            {
                return trueSolution;
            }

            unknownPositions.Push(position);
            return null;
        }

        [NotNull]
        private Stack<SurfacePosition> GetUnknownPositions([NotNull] IPuzzleSurface surface)
        {
            var unknownPositions = new Stack<SurfacePosition>();
            for (int lineIndex = 0; lineIndex < surface.LineCount; lineIndex++)
            {
                for (int columnIndex = 0; columnIndex < surface.ColumnCount; columnIndex++)
                {
                    bool? cell = surface.GetCell(lineIndex, columnIndex);
                    if (cell == null)
                    {
                        unknownPositions.Push(new SurfacePosition(lineIndex, columnIndex));
                    }
                }
            }
            return unknownPositions;
        }
    }
}
