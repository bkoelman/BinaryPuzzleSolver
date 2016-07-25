using System;
using System.Collections;
using BinaryPuzzleSolver.Engine;
using Xunit;

namespace BinaryPuzzleSolver.UnitTests
{
    public class BitArrayExtensionsTests
    {
        [Fact]
        public void TestIncrementNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                BitArray bitArray = null;
                bitArray.Increment();
            });
        }

        [Fact]
        public void TestIncrement0()
        {
            var bitArray = new BitArray(1);
            bitArray.Increment();

            Assert.Equal(bitArray[0], true);
        }

        [Fact]
        public void TestIncrement1()
        {
            var bitArray = new BitArray(2);
            bitArray.Set(0, true);
            bitArray.Increment();

            Assert.Equal(bitArray[0], false);
            Assert.Equal(bitArray[1], true);
        }

        [Fact]
        public void TestIncrement2()
        {
            var bitArray = new BitArray(2);
            bitArray.Set(1, true);
            bitArray.Increment();

            Assert.Equal(bitArray[0], true);
            Assert.Equal(bitArray[1], true);
        }

        [Fact]
        public void TestIncrement7()
        {
            var bitArray = new BitArray(4);
            bitArray.Set(0, true);
            bitArray.Set(1, true);
            bitArray.Set(2, true);
            bitArray.Increment();

            Assert.Equal(bitArray[0], false);
            Assert.Equal(bitArray[1], false);
            Assert.Equal(bitArray[2], false);
            Assert.Equal(bitArray[3], true);
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