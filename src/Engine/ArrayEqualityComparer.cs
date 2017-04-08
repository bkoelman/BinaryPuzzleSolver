using System.Collections.Generic;
using JetBrains.Annotations;

namespace BinaryPuzzleSolver.Engine
{
    public sealed class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
    {
        public bool Equals([CanBeNull] [ItemCanBeNull] T[] x, [CanBeNull] [ItemCanBeNull] T[] y)
        {
            if (ReferenceEquals(x, y))
            {
                // Both null or same instance.
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.Length != y.Length)
            {
                return false;
            }

            for (int index = 0; index < x.Length; index++)
            {
                T xItem = x[index];
                T yItem = y[index];

                if (!EqualityComparer<T>.Default.Equals(xItem, yItem))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode([CanBeNull] [ItemCanBeNull] T[] obj)
        {
            int hashCode = EqualityComparer<T>.Default.GetHashCode(default(T));
            if (!ReferenceEquals(obj, null))
            {
                for (int index = 0; index < obj.Length; index++)
                {
                    hashCode ^= EqualityComparer<T>.Default.GetHashCode(obj[index]);
                }
            }
            return hashCode;
        }
    }
}
