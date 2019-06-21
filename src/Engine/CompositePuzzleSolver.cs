using JetBrains.Annotations;

namespace BinaryPuzzleSolver.Engine
{
    public sealed class CompositePuzzleSolver
    {
        public void Solve([NotNull] IPuzzleSurface surface)
        {
            var ruleSetPuzzleSolver = new RuleSetPuzzleSolver(surface);
            ruleSetPuzzleSolver.Solve();

            if (!surface.IsSolved)
            {
                var bruteForcePuzzleSolver = new BruteForcePuzzleSolver();
                bruteForcePuzzleSolver.Solve(surface);
            }
        }
    }
}
