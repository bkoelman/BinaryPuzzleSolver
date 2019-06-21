using System;
using System.Collections;
using BinaryPuzzleSolver.Engine;
using Xunit;

namespace BinaryPuzzleSolver.UnitTests
{
    public sealed class BitArrayExtensionsTests
    {
        [Fact]
        public void TestIncrementNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                BitArray bitArray = null;
                // ReSharper disable once AssignNullToNotNullAttribute
                bitArray.Increment();
            });
        }

        [Fact]
        public void TestIncrement0()
        {
            var bitArray = new BitArray(1);
            bitArray.Increment();

            Assert.True(bitArray[0]);
        }

        [Fact]
        public void TestIncrement1()
        {
            var bitArray = new BitArray(2);
            bitArray.Set(0, true);
            bitArray.Increment();

            Assert.False(bitArray[0]);
            Assert.True(bitArray[1]);
        }

        [Fact]
        public void TestIncrement2()
        {
            var bitArray = new BitArray(2);
            bitArray.Set(1, true);
            bitArray.Increment();

            Assert.True(bitArray[0]);
            Assert.True(bitArray[1]);
        }

        [Fact]
        public void TestIncrement7()
        {
            var bitArray = new BitArray(4);
            bitArray.Set(0, true);
            bitArray.Set(1, true);
            bitArray.Set(2, true);
            bitArray.Increment();

            Assert.False(bitArray[0]);
            Assert.False(bitArray[1]);
            Assert.False(bitArray[2]);
            Assert.True(bitArray[3]);
        }

        [Fact]
        public void TestIncrementOverflow()
        {
            Assert.Throws<OverflowException>(() =>
            {
                var bitArray = new BitArray(4);
                bitArray.Set(0, true);
                bitArray.Set(1, true);
                bitArray.Set(2, true);
                bitArray.Set(3, true);
                bitArray.Increment();
            });
        }
    }
}
