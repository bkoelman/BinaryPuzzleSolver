using System;
using BinaryPuzzleSolver.Engine;
using JetBrains.Annotations;
using Xunit;

namespace BinaryPuzzleSolver.UnitTests
{
    public sealed class SurfaceValidatorTests
    {
        [Fact]
        public void TestNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // ReSharper disable once UnusedVariable
                // ReSharper disable once AssignNullToNotNullAttribute
                var validator = new SurfaceValidator(null);
            });
        }

        [Fact]
        public void TestValidateCompleteIncorrectDigitCountsInLine()
        {
            Assert.Throws<IncorrectPuzzleSurfaceException>(() =>
            {
                SurfaceValidator validator = CreateValidator(new[] { "01010100", "--------" });
                validator.Validate();
            });
        }

        [Fact]
        public void TestValidateCompleteIncorrectDigitCountsInColumn()
        {
            Assert.Throws<IncorrectPuzzleSurfaceException>(() =>
            {
                SurfaceValidator validator = CreateValidator(new[] { "-0", "-1", "-0", "-1", "-0", "-1", "-0", "-0" });
                validator.Validate();
            });
        }

        [Fact]
        public void TestValidateIncompleteIncorrectDigitCountsInLine()
        {
            Assert.Throws<IncorrectPuzzleSurfaceException>(() =>
            {
                SurfaceValidator validator = CreateValidator(new[] { "00-00-01", "--------" });
                validator.Validate();
            });
        }

        [Fact]
        public void TestValidateCompleteIncorrectSequenceLengthInLine()
        {
            Assert.Throws<IncorrectPuzzleSurfaceException>(() =>
            {
                SurfaceValidator validator = CreateValidator(new[] { "00101110", "--------" });
                validator.Validate();
            });
        }

        [Fact]
        public void TestValidateCompleteIncorrectSequenceLengthInColumn()
        {
            Assert.Throws<IncorrectPuzzleSurfaceException>(() =>
            {
                SurfaceValidator validator = CreateValidator(new[] { "-0", "-0", "-1", "-0", "-0", "-1", "-1", "-1" });
                validator.Validate();
            });
        }

        [Fact]
        public void TestValidateIncompleteIncorrectSequenceLengthInLine()
        {
            Assert.Throws<IncorrectPuzzleSurfaceException>(() =>
            {
                SurfaceValidator validator = CreateValidator(new[] { "00---111", "--------" });
                validator.Validate();
            });
        }

        [Fact]
        public void TestValidateCompleteNoDuplicateLinesOrColumns()
        {
            Assert.Throws<IncorrectPuzzleSurfaceException>(() =>
            {
                SurfaceValidator validator = CreateValidator(new[] { "00110101", "00110101", "11001010", "11001010" });
                validator.Validate();
            });
        }

        [Fact]
        public void TestCorrectUnknown()
        {
            SurfaceValidator validator = CreateValidator(new[] { "------", "------" });
            validator.Validate();
        }

        [Fact]
        public void TestCorrectIncomplete()
        {
            SurfaceValidator validator = CreateValidator(new[] { "1-0-01", "01-1--" });
            validator.Validate();
        }

        [Fact]
        public void TestCorrectComplete_2x2()
        {
            SurfaceValidator validator = CreateValidator(new[] { "10", "01" });
            validator.Validate();
        }

        [Fact]
        public void TestCorrectComplete_4x4()
        {
            SurfaceValidator validator = CreateValidator(new[] { "0101", "1010", "0011", "1100" });
            validator.Validate();
        }

        [NotNull]
        private SurfaceValidator CreateValidator([NotNull] [ItemNotNull] string[] surfaceText)
        {
            var factory = new SurfaceFactory();
            IPuzzleSurface surface = factory.CreateFromText(surfaceText);
            return new SurfaceValidator(surface);
        }
    }
}
