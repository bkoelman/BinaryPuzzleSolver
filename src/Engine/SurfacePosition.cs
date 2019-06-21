using System;

namespace BinaryPuzzleSolver.Engine
{
    public struct SurfacePosition : IComparable<SurfacePosition>, IEquatable<SurfacePosition>
    {
        private readonly int hashCode;

        public int LineIndex { get; }

        public int ColumnIndex { get; }

        public SurfacePosition(int lineIndex, int columnIndex)
            : this()
        {
            LineIndex = lineIndex;
            ColumnIndex = columnIndex;
            hashCode = LineIndex.GetHashCode() ^ ColumnIndex.GetHashCode();
        }

        public int CompareTo(SurfacePosition other)
        {
            int result = LineIndex.CompareTo(other.LineIndex);
            if (result == 0)
            {
                result = ColumnIndex.CompareTo(other.ColumnIndex);
            }
            return result;
        }

        public bool Equals(SurfacePosition other)
        {
            return other.LineIndex == LineIndex && other.ColumnIndex == ColumnIndex;
        }

        public override bool Equals(object obj)
        {
            if (!ReferenceEquals(obj, null) && obj is SurfacePosition surfacePosition)
            {
                return Equals(surfacePosition);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override string ToString()
        {
            return $"({LineIndex},{ColumnIndex})";
        }
    }
}
