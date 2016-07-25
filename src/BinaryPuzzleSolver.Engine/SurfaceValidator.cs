using System;
using System.Collections.Generic;
using BinaryPuzzleSolver.Engine.Utilities;

namespace BinaryPuzzleSolver.Engine
{
    public class SurfaceValidator
    {
        private static readonly ArrayEqualityComparer<bool> BooleanArrayEqualityComparer =
            new ArrayEqualityComparer<bool>();

        private readonly IPuzzleSurface sourceSurface;
        private readonly PuzzleRotatedSurface rotatedSurface;

        public SurfaceValidator(IPuzzleSurface surface)
        {
            Guard.NotNull(surface, nameof(surface));

            sourceSurface = surface;
            rotatedSurface = new PuzzleRotatedSurface(surface);
        }

        public void Validate()
        {
            // Implicit puzzle rules that are not verified here:
            // - LineCount and ColumnCount must be even and >= 2. This is ensured by factory/ctor.
            // - Cell values must be in range [1, 0, -]. 
            //   This is ensured by using bool? in the object model, which contains values in range [True, False, null].

            // Explicit puzzle rules that are verified by this method:
            // - Each line/column must have same number of 1s and 0s.
            // - Each line/column cannot contain a sequence of more than two 1's or 0's.
            // - Duplicate lines are not allowed. Duplicate columns are not allowed.

            ValidateDigitCounts(true);
            ValidateSequenceLengths(true);
            ValidateNoDuplicates(true);
        }

        public bool TryValidate()
        {
            bool succeeded = ValidateDigitCounts(false);
            succeeded = succeeded && ValidateSequenceLengths(false);
            succeeded = succeeded && ValidateNoDuplicates(false);
            return succeeded;
        }

        private bool ValidateDigitCounts(bool throwOnError)
        {
            return InnerValidate((surface, isRotated) =>
            {
                for (int lineIndex = 0; lineIndex < surface.LineCount; lineIndex++)
                {
                    int oneCount = surface.GetCountInLine(lineIndex, true);
                    if (oneCount > surface.ColumnCount / 2)
                    {
                        // Too many 1s.
                        if (throwOnError)
                        {
                            throw new IncorrectPuzzleSurfaceException(string.Format("Too many 1s in {0} {1}.",
                                isRotated ? "column" : "line", lineIndex));
                        }
                        return false;
                    }

                    int zeroCount = surface.GetCountInLine(lineIndex, false);
                    if (zeroCount > surface.ColumnCount / 2)
                    {
                        // Too many 0s.
                        if (throwOnError)
                        {
                            throw new IncorrectPuzzleSurfaceException(string.Format("Too many 0s in {0} {1}.",
                                isRotated ? "column" : "line", lineIndex));
                        }
                        return false;
                    }
                }
                return true;
            });
        }

        private bool ValidateSequenceLengths(bool throwOnError)
        {
            return InnerValidate((surface, isRotated) =>
            {
                for (int lineIndex = 0; lineIndex < surface.LineCount; lineIndex++)
                {
                    for (int columnIndex = 0; columnIndex < surface.ColumnCount - 2; columnIndex++)
                    {
                        bool? cell1 = surface.GetCell(lineIndex, columnIndex);
                        bool? cell2 = surface.GetCell(lineIndex, columnIndex + 1);
                        bool? cell3 = surface.GetCell(lineIndex, columnIndex + 2);

                        if (cell1 == cell2 && cell2 == cell3)
                        {
                            // Found sequence of three. Only allow when they are unknowns.
                            if (cell1 != null)
                            {
                                if (throwOnError)
                                {
                                    throw new IncorrectPuzzleSurfaceException(
                                        string.Format("Found sequence of three {0}s in {1} {2}.",
                                            cell1.Value ? "1" : "0", isRotated ? "column" : "line", lineIndex));
                                }
                                return false;
                            }
                        }
                    }
                }
                return true;
            });
        }

        private bool ValidateNoDuplicates(bool throwOnError)
        {
            return InnerValidate((surface, isRotated) =>
            {
                var completeLines = new HashSet<bool[]>(BooleanArrayEqualityComparer);
                for (int lineIndex = 0; lineIndex < surface.LineCount; lineIndex++)
                {
                    if (surface.IsLineComplete(lineIndex))
                    {
                        var line = new bool[surface.ColumnCount];
                        for (int columnIndex = 0; columnIndex < line.Length; columnIndex++)
                        {
                            line[columnIndex] = surface.GetCell(lineIndex, columnIndex).Value;
                        }

                        if (completeLines.Contains(line))
                        {
                            if (throwOnError)
                            {
                                throw new IncorrectPuzzleSurfaceException(string.Format(
                                    "{0} {1} occurs more than once.", isRotated ? "Column" : "Line", lineIndex));
                            }
                            return false;
                        }

                        completeLines.Add(line);
                    }
                }
                return true;
            });
        }

        private bool InnerValidate(Func<IPuzzleSurface, bool /* isRotated */, bool /* succeeded */> body)
        {
            // By invoking the validation logic on the source surface and its 90 degrees rotated counterpart,
            // the validation logic only needs to take lines into account. The same validation over columns
            // occurs when rerunning against the rotated surface.

            bool succeeded = body(sourceSurface, false);
            succeeded = succeeded && body(rotatedSurface, true);
            return succeeded;
        }
    }
}