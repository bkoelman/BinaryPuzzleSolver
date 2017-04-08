using System;
using BinaryPuzzleSolver.Engine;
using Xunit;

namespace BinaryPuzzleSolver.UnitTests
{
    public class ComparingPuzzleSurfaceTests
    {
        [Fact]
        public void TestSourceNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var factory = new SurfaceFactory();
                IPuzzleSurface answer = factory.CreateFromText("11", "00");

                var surface = new ComparingPuzzleSurface(null, answer);
            });
        }

        [Fact]
        public void TestAnswerNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var factory = new SurfaceFactory();
                IPuzzleSurface source = factory.CreateFromText("11", "00");

                var surface = new ComparingPuzzleSurface(source, null);
            });
        }

        [Fact]
        public void TestNotSameSize()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var factory = new SurfaceFactory();
                IPuzzleSurface source = factory.CreateFromText("11", "00");
                IPuzzleSurface answer = factory.CreateFromText("1101", "0010", "1001", "1010");

                var surface = new ComparingPuzzleSurface(source, answer);
            });
        }

        [Fact]
        public void TestIncompleteAnswer()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var factory = new SurfaceFactory();
                IPuzzleSurface surface = factory.CreateFromText("10", "0-");

                var result = new ComparingPuzzleSurface(surface, surface);
            });
        }

        [Fact]
        public void TestIncorrectSourceAtStart()
        {
            Assert.Throws<IncorrectSurfaceCellValueException>(() =>
            {
                var factory = new SurfaceFactory();
                IPuzzleSurface source = factory.CreateFromText("1-", "1-");
                IPuzzleSurface answer = factory.CreateFromText("10", "01");

                var result = new ComparingPuzzleSurface(source, answer);
            });
        }

        [Fact]
        public void TestCorrectSourceAtStart()
        {
            var factory = new SurfaceFactory();
            IPuzzleSurface source = factory.CreateFromText("1-", "-0");
            IPuzzleSurface answer = factory.CreateFromText("10", "00");

            var result = new ComparingPuzzleSurface(source, answer);
        }

        [Fact]
        public void TestIncorrectSourceAssignmentFalse()
        {
            Assert.Throws<IncorrectSurfaceCellValueException>(() =>
            {
                var factory = new SurfaceFactory();
                IPuzzleSurface source = factory.CreateFromText("1-", "--");
                IPuzzleSurface answer = factory.CreateFromText("11", "01");

                var result = new ComparingPuzzleSurface(source, answer);
                result.SetCell(0, 1, false);
            });
        }

        [Fact]
        public void TestIncorrectSourceAssignmentTrue()
        {
            Assert.Throws<IncorrectSurfaceCellValueException>(() =>
            {
                var factory = new SurfaceFactory();
                IPuzzleSurface source = factory.CreateFromText("1-", "--");
                IPuzzleSurface answer = factory.CreateFromText("10", "01");

                var result = new ComparingPuzzleSurface(source, answer);
                result.SetCell(0, 1, true);
            });
        }

        [Fact]
        public void TestCorrectSourceAssignment()
        {
            var factory = new SurfaceFactory();
            IPuzzleSurface source = factory.CreateFromText("1-", "--");
            IPuzzleSurface answer = factory.CreateFromText("10", "01");

            var result = new ComparingPuzzleSurface(source, answer);
            result.SetCell(0, 1, false);
            result.SetCell(1, 0, false);
            result.SetCell(1, 1, true);
        }
    }
}
