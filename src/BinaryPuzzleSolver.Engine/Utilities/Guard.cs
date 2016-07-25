using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace BinaryPuzzleSolver.Engine.Utilities
{
    /// <summary>
    /// Provides assertions for preconditions.
    /// </summary>
    public static class Guard
    {
        [AssertionMethod]
        [ContractAnnotation("value: null => halt")]
        public static void NotNull<T>([CanBeNull] [NoEnumeration] T value, [NotNull] [InvokerParameterName] string name)
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(name);
            }
        }

        [AssertionMethod]
        [ContractAnnotation("value: null => halt")]
        public static void NotNullNorEmpty<T>([CanBeNull] [ItemCanBeNull] IEnumerable<T> value,
            [NotNull] [InvokerParameterName] string name)
        {
            NotNull(value, name);

            if (!value.Any())
            {
                throw new ArgumentException(name + @" cannot be empty.", name);
            }
        }
    }
}