using System;
using BinaryPuzzleSolver.Engine;
using JetBrains.Annotations;

namespace BinaryPuzzleSolver.DemoApp
{
    internal static class Program
    {
        private const string DebugPuzzlePath = @"..\Puzzles\BinaryPuzzles.com\Hard\14x14\Puzzle001.xml";

        public static void Main()
        {
            IPuzzleSurface puzzle = CreatePuzzle();

            Console.WriteLine("Source puzzle:");
            DumpSurface(puzzle);

            using (new CodeTimer("Solve"))
            {
                var solver = new CompositePuzzleSolver();
                solver.Solve(puzzle);

                Console.WriteLine(!puzzle.IsSolved ? "Unable to solve this puzzle." : "Puzzle solved.");
                DumpSurface(puzzle);
            }

            var validator = new SurfaceValidator(puzzle);
            validator.Validate();

            Console.WriteLine();
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }

        [NotNull]
        private static IPuzzleSurface CreatePuzzle()
        {
            var factory = new SurfaceFactory();

            IPuzzleSurface surfacePuzzle = factory.CreateFromExcelXmlFile(DebugPuzzlePath, "Puzzle");
            new SurfaceValidator(surfacePuzzle).Validate();

            IPuzzleSurface surfaceAnswer = factory.CreateFromExcelXmlFile(DebugPuzzlePath, "Answer");
            new SurfaceValidator(surfaceAnswer).Validate();

            return new ComparingPuzzleSurface(surfacePuzzle, surfaceAnswer);
        }

        private static void DumpSurface([NotNull] IPuzzleSurface surface)
        {
            foreach (string line in surface.ToString("#").Split('#'))
            {
                Console.WriteLine("  " + line);
            }
            Console.WriteLine();
        }
    }
}
