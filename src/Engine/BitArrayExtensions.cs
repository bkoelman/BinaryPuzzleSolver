using System;
using System.Collections;
using BinaryPuzzleSolver.Engine.Utilities;
using JetBrains.Annotations;

namespace BinaryPuzzleSolver.Engine
{
    public static class BitArrayExtensions
    {
        public static void Increment([NotNull] [ItemNotNull] this BitArray source)
        {
            Guard.NotNull(source, nameof(source));

            for (int index = 0; index < source.Length; index++)
            {
                if (source[index] == false)
                {
                    // Bit is set to False. Increment means: set this bit to True and all bits before it to False.
                    source[index] = true;
                    for (int beforeIndex = index - 1; beforeIndex >= 0; beforeIndex--)
                    {
                        source[beforeIndex] = false;
                    }
                    return;
                }
            }

            throw new OverflowException();
        }

        public static bool AreAllSet([NotNull] [ItemNotNull] this BitArray source)
        {
            Guard.NotNull(source, nameof(source));

            for (int index = source.Length - 1; index >= 0; index--)
            {
                if (source[index] == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
