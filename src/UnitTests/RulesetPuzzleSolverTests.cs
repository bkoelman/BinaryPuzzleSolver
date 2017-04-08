using System;
using BinaryPuzzleSolver.Engine;
using JetBrains.Annotations;
using Xunit;

namespace BinaryPuzzleSolver.UnitTests
{
    public sealed class RulesetPuzzleSolverTests
    {
        [Fact]
        public void TestResolveBeforeAfterPairsInLines()
        {
            VerifySolve(new[] { "-00-", "-11-" }, new[] { "1001", "0110" }, solver => solver.ResolveBeforeAfterPairs());
        }

        [Fact]
        public void TestResolveBeforeAfterPairsInColumns()
        {
            VerifySolve(new[] { "01--", "0101", "--01", "0101" }, new[] { "0110", "0101", "1001", "0101" },
                solver => solver.ResolveBeforeAfterPairs());
        }

        [Fact]
        public void TestCannotResolveBeforeAfterPairs()
        {
            string[] source = { "10-1", "0-1-", "-10-", "0--1" };
            VerifySolve(source, source, solver => solver.ResolveBeforeAfterPairs());
        }

        [Fact]
        public void TestResolveBetweenTripletsInLines()
        {
            VerifySolve(new[] { "0-01", "01-1" }, new[] { "0101", "0101" }, solver => solver.ResolveBetweenTriplets());
        }

        [Fact]
        public void TestResolveBetweenTripletsInColumns()
        {
            VerifySolve(new[] { "00", "-1", "0-", "11" }, new[] { "00", "11", "00", "11" },
                solver => solver.ResolveBetweenTriplets());
        }

        [Fact]
        public void TestCannotResolveBetweenTriplets()
        {
            string[] source = { "10-1", "0-1-", "-10-", "---1" };
            VerifySolve(source, source, solver => solver.ResolveBetweenTriplets());
        }

        [Fact]
        public void TestResolveDigitCountsInLines()
        {
            VerifySolve(new[] { "-010", "0--0", "----", "----", "----", "----" },
                new[] { "1010", "0110", "----", "----", "----", "----" }, solver => solver.ResolveDigitCounts());
        }

        [Fact]
        public void TestResolveDigitCountsInColumns()
        {
            VerifySolve(new[] { "-0----", "0-----", "1-----", "00----" }, new[] { "10----", "01----", "11----", "00----" },
                solver => solver.ResolveDigitCounts());
        }

        [Fact]
        public void TestResolveDigitCountsInBoth()
        {
            VerifySolve(new[] { "11011001--", "----------" }, new[] { "1101100100", "0010011011" },
                solver => solver.ResolveDigitCounts());
        }

        [Fact]
        public void TestCannotResolveDigitCounts()
        {
            string[] source = { "10--", "0-1-", "-10-", "---1" };
            VerifySolve(source, source, solver => solver.ResolveDigitCounts());
        }

        [Fact]
        public void TestResolveMissingSingleDigitInLines()
        {
            VerifySolve(new[] { "001-0--1-0", "110-1--0-1" }, new[] { "0011010110", "1100101001" },
                solver => solver.ResolveMissingSingleDigit());
        }

        [Fact]
        public void TestResolveMissingSingleDigitInColumns()
        {
            VerifySolve(new[] { "01", "01", "10", "--", "01", "--", "--", "10", "--", "01" },
                new[] { "01", "01", "10", "10", "01", "10", "01", "10", "10", "01" },
                solver => solver.ResolveMissingSingleDigit());
        }

        [Fact]
        public void TestResolveMissingSingleDigitOppositeInLines()
        {
            VerifySolve(new[] { "101-0-1-", "--------", "--------", "--------" },
                new[] { "101-0-10", "--------", "--------", "--------" }, solver => solver.ResolveMissingSingleDigit());
        }

        [Fact]
        public void TestResolveMissingSingleDigitOppositeInColumns()
        {
            VerifySolve(new[] { "1---", "0---", "1---", "----", "0---", "----", "1---", "----" },
                new[] { "1---", "0---", "1---", "----", "0---", "----", "1---", "0---" },
                solver => solver.ResolveMissingSingleDigit());
        }

        [Fact]
        public void TestCannotResolveMissingSingleDigit()
        {
            string[] source = { "-01-0--1-0", "-10-1--0-1" };
            VerifySolve(source, source, solver => solver.ResolveMissingSingleDigit());
        }

        [Fact]
        public void TestResolveNoDuplicateLines()
        {
            VerifySolve(new[] { "001101", "001--1" }, new[] { "001101", "001011" }, solver => solver.ResolveNoDuplicateLines());
        }

        [Fact]
        public void TestResolveNoDuplicateColumns()
        {
            VerifySolve(new[] { "00", "00", "11", "1-", "0-", "11" }, new[] { "00", "00", "11", "10", "01", "11" },
                solver => solver.ResolveNoDuplicateLines());
        }

        [Fact]
        public void TestCannotResolveNoDuplicateLines()
        {
            string[] source =
            {
                "01101100",
                "011--100", // Adding 10 would cause 3x1, which is not allowed.
                "--------",
                "--------"
            };
            VerifySolve(source, source, solver => solver.ResolveNoDuplicateLines());
        }

        [AssertionMethod]
        private void VerifySolve([NotNull] [ItemNotNull] string[] input, [NotNull] [ItemNotNull] string[] expected,
            [NotNull] Action<RulesetPuzzleSolver> solveAction)
        {
            var factory = new SurfaceFactory();
            IPuzzleSurface surface = factory.CreateFromText(input);
            var solver = new RulesetPuzzleSolver(surface);

            solveAction(solver);

            Assert.Equal(string.Join("#", expected), surface.ToString("#"));
        }
    }
}
