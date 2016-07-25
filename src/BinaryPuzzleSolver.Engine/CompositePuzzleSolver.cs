namespace BinaryPuzzleSolver.Engine
{
    public class CompositePuzzleSolver
    {
        public void Solve(IPuzzleSurface surface)
        {
            var rulesetPuzzleSolver = new RulesetPuzzleSolver(surface);
            rulesetPuzzleSolver.Solve();

            if (!surface.IsSolved)
            {
                var bruteForcePuzzleSolver = new BruteForcePuzzleSolver();
                bruteForcePuzzleSolver.Solve(surface);
            }
        }
    }
}