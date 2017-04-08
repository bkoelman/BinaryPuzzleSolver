using System.Collections.Generic;
using BinaryPuzzleSolver.Engine;
using JetBrains.Annotations;
using Xunit;

namespace BinaryPuzzleSolver.UnitTests
{
    public sealed class ArrayEqualityComparerTests
    {
        [Fact]
        public void TestEqualsBothNull()
        {
            var comparer = new ArrayEqualityComparer<ObjectWithEquals>();
            Assert.True(comparer.Equals(null, null));
        }

        [Fact]
        public void TestEqualsSameInstance()
        {
            ObjectWithEquals[] target = { new ObjectWithEquals("text") };

            var comparer = new ArrayEqualityComparer<ObjectWithEquals>();
            Assert.True(comparer.Equals(target, target));
        }

        [Fact]
        public void TestEqualsFirstNull()
        {
            ObjectWithEquals[] target = { new ObjectWithEquals("text") };

            var comparer = new ArrayEqualityComparer<ObjectWithEquals>();
            Assert.False(comparer.Equals(target, null));
        }

        [Fact]
        public void TestEqualsSecondNull()
        {
            ObjectWithEquals[] target = { new ObjectWithEquals("text") };

            var comparer = new ArrayEqualityComparer<ObjectWithEquals>();
            Assert.False(comparer.Equals(null, target));
        }

        [Fact]
        public void TestEqualsNotSameLength()
        {
            ObjectWithEquals[] target1 = { new ObjectWithEquals("text") };
            ObjectWithEquals[] target2 = { new ObjectWithEquals("text"), new ObjectWithEquals("text") };

            var comparer = new ArrayEqualityComparer<ObjectWithEquals>();
            Assert.False(comparer.Equals(target1, target2));
        }

        [Fact]
        public void TestEqualsNotSameValue()
        {
            ObjectWithEquals[] target1 = { new ObjectWithEquals("first") };
            ObjectWithEquals[] target2 = { new ObjectWithEquals("second") };

            var comparer = new ArrayEqualityComparer<ObjectWithEquals>();
            Assert.False(comparer.Equals(target1, target2));
        }

        [Fact]
        public void TestEqualsSameValue()
        {
            ObjectWithEquals[] target1 = { new ObjectWithEquals("text") };
            ObjectWithEquals[] target2 = { new ObjectWithEquals("text") };

            var comparer = new ArrayEqualityComparer<ObjectWithEquals>();
            Assert.True(comparer.Equals(target1, target2));
        }

        [Fact]
        public void TestGetHashCodeNull()
        {
            int expected = EqualityComparer<ObjectWithEquals>.Default.GetHashCode(null);

            var comparer = new ArrayEqualityComparer<ObjectWithEquals>();
            Assert.Equal(comparer.GetHashCode(null), expected);
        }

        [Fact]
        public void TestGetHashCodeEmpty()
        {
            var target = new ObjectWithEquals[0];
            int expected = EqualityComparer<ObjectWithEquals>.Default.GetHashCode(null);

            var comparer = new ArrayEqualityComparer<ObjectWithEquals>();
            Assert.Equal(comparer.GetHashCode(target), expected);
        }

        [Fact]
        public void TestGetHashCodeNotEmpty()
        {
            ObjectWithEquals[] target = { new ObjectWithEquals("abc"), new ObjectWithEquals("xyz") };

            int expected = "abc".GetHashCode() ^ "xyz".GetHashCode();

            var comparer = new ArrayEqualityComparer<ObjectWithEquals>();
            Assert.Equal(comparer.GetHashCode(target), expected);
        }

        private sealed class ObjectWithEquals
        {
            [CanBeNull]
            public string Value { get; }

            public ObjectWithEquals([CanBeNull] string value)
            {
                Value = value;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ObjectWithEquals;
                if (!ReferenceEquals(other, null))
                {
                    return other.Value == Value;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return Value?.GetHashCode() ?? 0;
            }
        }
    }
}
