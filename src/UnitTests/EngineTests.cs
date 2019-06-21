using System;
using System.Collections.Generic;
using System.IO;
using BinaryPuzzleSolver.Engine;
using JetBrains.Annotations;
using Xunit;

namespace BinaryPuzzleSolver.UnitTests
{
    public sealed class EngineTests
    {
        [Fact]
        public void TestPuzzlesOnDisk_RuleSet()
        {
            foreach (Tuple<IPuzzleSurface, string> surfaceWithPath in EnumeratePuzzlesInFolder("RuleSetTestPuzzles"))
            {
                IPuzzleSurface surface = surfaceWithPath.Item1;
                string path = surfaceWithPath.Item2;

                try
                {
                    var solver = new RuleSetPuzzleSolver(surface);
                    solver.Solve();
                }
                catch (Exception ex)
                {
                    Assert.True(false, $"{ex.Message} At: {path}");

                    // Workaround for error CS0165: Use of unassigned local variable 'surfacePuzzle'
                    throw;
                }

                Assert.True(surface.IsSolved, $"Failed to solve puzzle at: {path}");
            }
        }

        [Fact]
        public void TestPuzzlesOnDisk_BruteForce()
        {
            foreach (Tuple<IPuzzleSurface, string> surfaceWithPath in EnumeratePuzzlesInFolder("BruteForceTestPuzzles"))
            {
                IPuzzleSurface surface = surfaceWithPath.Item1;
                string path = surfaceWithPath.Item2;

                try
                {
                    var solver = new BruteForcePuzzleSolver();
                    solver.Solve(surface);
                }
                catch (Exception ex)
                {
                    Assert.True(false, $"{ex.Message} At: {path}");

                    // Workaround for error CS0165: Use of unassigned local variable 'surfacePuzzle'
                    throw;
                }

                Assert.True(surface.IsSolved, $"Failed to solve puzzle at: {path}");
            }
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<Tuple<IPuzzleSurface, string>> EnumeratePuzzlesInFolder([NotNull] string folderName)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..", folderName);

            foreach (string path in Directory.EnumerateFiles(folder, "*.xml"))
            {
                IPuzzleSurface surface = CreateFromDisk(path);
                yield return Tuple.Create(surface, path);
            }
        }

        [NotNull]
        private IPuzzleSurface CreateFromDisk([NotNull] string path)
        {
            var factory = new SurfaceFactory();

            IPuzzleSurface surfacePuzzle = factory.CreateFromExcelXmlFile(path, "Puzzle");
            new SurfaceValidator(surfacePuzzle).Validate();

            IPuzzleSurface surfaceAnswer = factory.CreateFromExcelXmlFile(path, "Answer");
            new SurfaceValidator(surfaceAnswer).Validate();

            return new ComparingPuzzleSurface(surfacePuzzle, surfaceAnswer);
        }
    }
}
