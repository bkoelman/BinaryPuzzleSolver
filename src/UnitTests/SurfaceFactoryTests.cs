using System;
using BinaryPuzzleSolver.Engine;
using Xunit;

namespace BinaryPuzzleSolver.UnitTests
{
    public class SurfaceFactoryTests
    {
        [Fact]
        public void TestFailOnNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var factory = new SurfaceFactory();
                factory.CreateFromText(null);
            });
        }

        [Fact]
        public void TestFailOnEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var factory = new SurfaceFactory();
                factory.CreateFromText();
            });
        }

        [Fact]
        public void TestFailOnLineNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var factory = new SurfaceFactory();
                factory.CreateFromText(null, null);
            });
        }

        [Fact]
        public void TestFailOnLineEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var factory = new SurfaceFactory();
                factory.CreateFromText(string.Empty, string.Empty);
            });
        }

        [Fact]
        public void TestFailOnInvalidCharacters()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var factory = new SurfaceFactory();
                factory.CreateFromText("110*", "0101");
            });
        }

        [Fact]
        public void TestFailOnDifferentLineLengths()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var factory = new SurfaceFactory();
                factory.CreateFromText("10", "1100");
            });
        }

        [Fact]
        public void TestFailOnUnevenLineCount()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var factory = new SurfaceFactory();
                factory.CreateFromText("1011", "1101", "1001");
            });
        }

        [Fact]
        public void TestFailOnUnevenColumnCount()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var factory = new SurfaceFactory();
                factory.CreateFromText("111", "010", "100", "111");
            });
        }

        [Fact]
        public void Test_2x2()
        {
            VerifySurface(new[] { "10", "11" });
        }

        [Fact]
        public void Test_4x2()
        {
            VerifySurface(new[] { "1011", "1101" });
        }

        [Fact]
        public void Test_4x6()
        {
            VerifySurface(new[] { "1011", "11-0", "0101", "1-10", "-111", "1-11" });
        }

        private void VerifySurface(string[] lines)
        {
            var factory = new SurfaceFactory();
            IPuzzleSurface surface = factory.CreateFromText(lines);

            string result = surface.ToString(":");
            Assert.Equal(string.Join(":", lines), result);
        }
    }
}
