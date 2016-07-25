﻿using System;
using System.Collections.Generic;
using System.IO;
using BinaryPuzzleSolver.Engine;
using Xunit;

namespace BinaryPuzzleSolver.UnitTests
{
    public class EngineTests
    {
        [Fact]
        public void TestPuzzlesOnDisk_Ruleset()
        {
            foreach (Tuple<IPuzzleSurface, string> surfaceWithPath in EnumeratePuzzlesInFolder("RulesetTestPuzzles"))
            {
                IPuzzleSurface surface = surfaceWithPath.Item1;
                string path = surfaceWithPath.Item2;

                try
                {
                    var solver = new RulesetPuzzleSolver(surface);
                    solver.Solve();
                }
                catch (Exception ex)
                {
                    Assert.True(false, string.Format("{0} At: {1}", ex.Message, path));

                    // Workaround for error CS0165: Use of unassigned local variable 'surfacePuzzle'
                    throw;
                }

                Assert.True(surface.IsSolved, string.Format("Failed to solve puzzle at: {0}", path));
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
                    Assert.True(false, string.Format("{0} At: {1}", ex.Message, path));

                    // Workaround for error CS0165: Use of unassigned local variable 'surfacePuzzle'
                    throw;
                }

                Assert.True(surface.IsSolved, string.Format("Failed to solve puzzle at: {0}", path));
            }
        }

        private IEnumerable<Tuple<IPuzzleSurface, string>> EnumeratePuzzlesInFolder(string folderName)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            foreach (string path in Directory.EnumerateFiles(folder, "*.xml"))
            {
                IPuzzleSurface surface = CreateFromDisk(path);
                yield return Tuple.Create(surface, path);
            }
        }

        private IPuzzleSurface CreateFromDisk(string path)
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