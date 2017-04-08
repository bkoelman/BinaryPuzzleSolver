using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BinaryPuzzleSolver.Engine.Utilities;
using JetBrains.Annotations;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// Attempts to solve as much as possible of a binary puzzle using built-in rulesets.
    /// </summary>
    public class RulesetPuzzleSolver
    {
        private static readonly ArrayEqualityComparer<bool> BooleanArrayEqualityComparer = new ArrayEqualityComparer<bool>();

        private readonly IPuzzleSurface sourceSurface;
        private readonly PuzzleRotatedSurface rotatedSurface;

        public RulesetPuzzleSolver([NotNull] IPuzzleSurface surface)
        {
            Guard.NotNull(surface, nameof(surface));

            sourceSurface = surface;
            rotatedSurface = new PuzzleRotatedSurface(surface);
        }

        public void Solve()
        {
            bool hasChanges;
            do
            {
                bool b1 = ResolveBeforeAfterPairs();
                bool b2 = ResolveBetweenTriplets();
                bool b3 = ResolveDigitCounts();
                bool b4 = ResolveMissingSingleDigit();
                bool b5 = ResolveNoDuplicateLines();

                hasChanges = b1 || b2 || b3 || b4 || b5;
            }
            while (hasChanges && !sourceSurface.IsSolved);
        }

        public bool ResolveBeforeAfterPairs()
        {
            return TrySolve(surface =>
            {
                // Because sequences of the same digit cannot be longer than two, before/after each 
                // such sequence must occur the opposite digit.
                // Example: -11- => 0110

                // Scan for sequences in lines.
                for (int lineIndex = 0; lineIndex < surface.LineCount; lineIndex++)
                {
                    if (!surface.IsLineComplete(lineIndex))
                    {
                        for (int columnIndex = 0; columnIndex < surface.ColumnCount - 2; columnIndex++)
                        {
                            if (surface.GetCell(lineIndex, columnIndex) == null)
                            {
                                // Find -00 or -11
                                if (surface.GetCell(lineIndex, columnIndex + 1) == false &&
                                    surface.GetCell(lineIndex, columnIndex + 2) == false)
                                {
                                    // Solve: -00 => 100
                                    surface.SetCell(lineIndex, columnIndex, true);
                                }
                                else if (surface.GetCell(lineIndex, columnIndex + 1) == true &&
                                    surface.GetCell(lineIndex, columnIndex + 2) == true)
                                {
                                    // Solve: -11 => 011
                                    surface.SetCell(lineIndex, columnIndex, false);
                                }
                            }
                            else if (surface.GetCell(lineIndex, columnIndex + 2) == null)
                            {
                                // Find 00- or 11-
                                if (surface.GetCell(lineIndex, columnIndex) == false &&
                                    surface.GetCell(lineIndex, columnIndex + 1) == false)
                                {
                                    // Solve: 00- => 001
                                    surface.SetCell(lineIndex, columnIndex + 2, true);
                                }
                                else if (surface.GetCell(lineIndex, columnIndex) == true &&
                                    surface.GetCell(lineIndex, columnIndex + 1) == true)
                                {
                                    // Solve: 11- => 110
                                    surface.SetCell(lineIndex, columnIndex + 2, false);
                                }
                            }
                        }
                    }
                }
            });
        }

        public bool ResolveBetweenTriplets()
        {
            return TrySolve(surface =>
            {
                // Because sequences of the same digit cannot be longer than two, in-between them 
                // must be the opposite digit.
                // Example: 0-0 => 010

                // Scan for sequences in lines.
                for (int lineIndex = 0; lineIndex < surface.LineCount; lineIndex++)
                {
                    if (!surface.IsLineComplete(lineIndex))
                    {
                        for (int columnIndex = 0; columnIndex < surface.ColumnCount - 2; columnIndex++)
                        {
                            if (surface.GetCell(lineIndex, columnIndex + 1) == null)
                            {
                                // Find 0-0 or 1-1
                                if (surface.GetCell(lineIndex, columnIndex) == false &&
                                    surface.GetCell(lineIndex, columnIndex + 2) == false)
                                {
                                    // Solve: 0-0 => 010
                                    surface.SetCell(lineIndex, columnIndex + 1, true);
                                }
                                else if (surface.GetCell(lineIndex, columnIndex) == true &&
                                    surface.GetCell(lineIndex, columnIndex + 2) == true)
                                {
                                    // Solve: 1-1 => 101
                                    surface.SetCell(lineIndex, columnIndex + 1, false);
                                }
                            }
                        }
                    }
                }
            });
        }

        public bool ResolveDigitCounts()
        {
            return TrySolve(surface =>
            {
                // Because a line/column must have the same number of zeroes as ones, unknowns can 
                // be added by counting the occurences of zeroes and ones.
                // Example: 11010-10101101001- =>  9x1 and 7x0, so need two more 0s => 110100101011010010

                // Scan for counts in lines.
                for (int lineIndex = 0; lineIndex < surface.LineCount; lineIndex++)
                {
                    if (!surface.IsLineComplete(lineIndex))
                    {
                        int zeroCount = surface.GetCountInLine(lineIndex, false);
                        int oneCount = surface.GetCountInLine(lineIndex, true);

                        bool? solveWith = null;

                        if (zeroCount != oneCount)
                        {
                            int delta = Math.Abs(oneCount - zeroCount);

                            int unknownCount = surface.ColumnCount - zeroCount - oneCount;
                            if (delta == unknownCount)
                            {
                                // When more 1s occur, fill remaining space with 0s (and the other way around).
                                solveWith = zeroCount > oneCount;
                            }
                        }

                        if (solveWith != null)
                        {
                            // Replace all remaining unknowns in line with value.
                            for (int columnIndex = 0; columnIndex < surface.ColumnCount; columnIndex++)
                            {
                                if (surface.GetCell(lineIndex, columnIndex) == null)
                                {
                                    surface.SetCell(lineIndex, columnIndex, solveWith.Value);
                                }
                            }
                        }
                    }
                }
            });
        }

        public bool ResolveMissingSingleDigit()
        {
            return TrySolve(surface =>
            {
                // When a line/column has a single unknown zero/one, that digit can only be placed at a single 
                // spot, in order to obey the rule that sequences of the same digit cannot be longer than two.
                // Example: 
                //   001-0--1-0 => 4x0 and 2x1, which means a single 0 needs to be placed.
                // Possibilities:
                //   00100--1-0 => 0010011110 => invalid, because sequence of 4x1
                //   001-00-1-0 => 0011001110 => invalid, because sequence of 3x1
                //   001-0-01-0 => 0011010110 => valid
                //   001-0--100 => 0011011100 => invalid, because sequence of 3x1
                //
                // Like the above, but when multiple valid solutions exist, we cannot solve the line/column. However,
                // if all valid solutions set a single unknown to the same value, we can reduce the number of unknowns.
                // Example: 
                //   101-0-1- => 3x1 and 2x0, which means a single 1 needs to be placed.
                // Possibilities:
                //   10110-1- => 10110010 => valid
                //   101-011- => 10100110 => valid
                //   101-0-11 => 10100011 => invalid, because sequence of 3x0
                // Although we do not have a single solution, the two valid combinations both set a 0 at the end, so we
                // can reduce the line to: 101-0-10.

                // Scan for lines where a single zero/one is missing.
                for (int lineIndex = 0; lineIndex < surface.LineCount; lineIndex++)
                {
                    if (!surface.IsLineComplete(lineIndex))
                    {
                        bool needsSingleZeroOrOne = false;

                        int zeroCount = surface.GetCountInLine(lineIndex, false);
                        if (zeroCount == surface.ColumnCount / 2 - 1)
                        {
                            // Need to place a single 0. 
                            needsSingleZeroOrOne = true;
                        }
                        else
                        {
                            int oneCount = surface.GetCountInLine(lineIndex, true);
                            if (oneCount == surface.ColumnCount / 2 - 1)
                            {
                                // Need to place a single 1.
                                needsSingleZeroOrOne = true;
                            }
                        }

                        if (needsSingleZeroOrOne)
                        {
                            // Brute-force create all valid combinations of the unknowns in this line.
                            List<bool[]> combinations = CreateLineCombinations(surface, lineIndex);

                            if (combinations.Count == 1)
                            {
                                // Found a single solution, so apply it.
                                for (int columnIndex = 0; columnIndex < surface.ColumnCount; columnIndex++)
                                {
                                    if (surface.GetCell(lineIndex, columnIndex) == null)
                                    {
                                        bool newValue = combinations[0][columnIndex];
                                        surface.SetCell(lineIndex, columnIndex, newValue);
                                    }
                                }
                            }
                            else if (combinations.Count > 1)
                            {
                                // See if we have the same value in all combinations for an unknown.

                                for (int columnIndex = 0; columnIndex < surface.ColumnCount; columnIndex++)
                                {
                                    int columnIndexCopy = columnIndex;

                                    bool? cell = surface.GetCell(lineIndex, columnIndex);
                                    if (cell == null)
                                    {
                                        bool areZero = combinations.All(sequence => sequence[columnIndexCopy] == false);
                                        if (areZero)
                                        {
                                            // All valid combinations have 0 at this position, so apply it.
                                            surface.SetCell(lineIndex, columnIndex, false);
                                        }
                                        else
                                        {
                                            bool areOne = combinations.All(sequence => sequence[columnIndexCopy]);
                                            if (areOne)
                                            {
                                                // All valid combinations have 1 at this position, so apply it.
                                                surface.SetCell(lineIndex, columnIndex, true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        public bool ResolveNoDuplicateLines()
        {
            // Duplicate lines/columns are not allowed (but a line can be the same as a column).
            // Example:
            //  10011010110010
            //  100110101--010 => 10011010101010

            return TrySolve(surface =>
            {
                // Create list of complete lines that we can compare against.
                var completeLines = new List<bool[]>();
                for (int lineIndex = 0; lineIndex < surface.LineCount; lineIndex++)
                {
                    if (surface.IsLineComplete(lineIndex))
                    {
                        var completeLine = new bool[surface.ColumnCount];
                        for (int columnIndex = 0; columnIndex < surface.ColumnCount; columnIndex++)
                        {
                            completeLine[columnIndex] = surface.GetCell(lineIndex, columnIndex).Value;
                        }
                        completeLines.Add(completeLine);
                    }
                }

                if (completeLines.Count > 0)
                {
                    // Scan for lines that have unknowns, but a similar complete line exists.
                    for (int lineIndex = 0; lineIndex < surface.LineCount; lineIndex++)
                    {
                        if (!surface.IsLineComplete(lineIndex))
                        {
                            List<bool[]> matchingCompleteLines = FilterMatchingCompleteLines(completeLines, surface, lineIndex);
                            if (matchingCompleteLines.Count > 0)
                            {
                                // Brute-force create all valid combinations of the unknowns in this line.
                                List<bool[]> combinations = CreateLineCombinations(surface, lineIndex);

                                // Remove combinations that exist in completeLines, hope we end up with one.
                                for (int combinationIndex = 0; combinationIndex < combinations.Count; combinationIndex++)
                                {
                                    bool[] currentCombination = combinations[combinationIndex];
                                    if (completeLines.Contains(currentCombination, BooleanArrayEqualityComparer))
                                    {
                                        combinations.RemoveAt(combinationIndex);
                                        combinationIndex--;
                                    }
                                }

                                if (combinations.Count == 1)
                                {
                                    // Found a single solution, so apply it.
                                    for (int columnIndex = 0; columnIndex < surface.ColumnCount; columnIndex++)
                                    {
                                        if (surface.GetCell(lineIndex, columnIndex) == null)
                                        {
                                            bool newValue = combinations[0][columnIndex];
                                            surface.SetCell(lineIndex, columnIndex, newValue);
                                        }
                                    }

                                    // Add this line to the list of complete lines.
                                    completeLines.Add(combinations[0]);
                                }
                            }
                        }
                    }
                }
            });
        }

        private List<bool[]> CreateLineCombinations(IPuzzleSurface surface, int lineIndex)
        {
            var validCombinations = new List<bool[]>();

            int unknownCount = surface.GetCountInLine(lineIndex, null);
            int combinationCount = checked((int)Math.Pow(2, unknownCount));

            var bitArray = new BitArray(unknownCount);
            for (int combinationIndex = 0; combinationIndex < combinationCount; combinationIndex++)
            {
                if (combinationIndex > 0)
                {
                    bitArray.Increment();
                }

                var lineCombination = new bool[surface.ColumnCount];
                for (int bitIndex = 0, columnIndex = 0; columnIndex < lineCombination.Length; columnIndex++)
                {
                    bool? cell = surface.GetCell(lineIndex, columnIndex);
                    if (cell == null)
                    {
                        bool bitValue = bitArray[bitIndex];
                        lineCombination[columnIndex] = bitValue;
                        bitIndex++;
                    }
                    else
                    {
                        // Copy existing known digits as-is.
                        lineCombination[columnIndex] = cell.Value;
                    }
                }

                if (IsValidCompleteSequence(lineCombination))
                {
                    validCombinations.Add(lineCombination);
                }
            }

            return validCombinations;
        }

        private List<bool[]> FilterMatchingCompleteLines(List<bool[]> completeLines, IPuzzleSurface surface, int lineIndex)
        {
            var matchingCompleteLines = new List<bool[]>();

            foreach (bool[] completeLine in completeLines)
            {
                bool isMatch = true;
                for (int columnIndex = 0; columnIndex < surface.ColumnCount; columnIndex++)
                {
                    bool candidateCell = completeLine[columnIndex];
                    bool? surfaceCell = surface.GetCell(lineIndex, columnIndex);

                    if (surfaceCell != null)
                    {
                        if (candidateCell != surfaceCell)
                        {
                            isMatch = false;
                            break;
                        }
                    }
                }

                if (isMatch)
                {
                    matchingCompleteLines.Add(completeLine);
                }
            }
            return matchingCompleteLines;
        }

        private static bool IsValidCompleteSequence(bool[] sequence)
        {
            // Check that:
            // - Sequences of same digit are no longer than two.
            // - Count of 1s and 0s is equal and nonzero.
            int zeroCount = 0;
            int oneCount = 0;

            int length = sequence.Length;
            for (int index = 0; index < length; index++)
            {
                if (sequence[index] == false)
                {
                    zeroCount++;
                }
                else
                {
                    oneCount++;
                }

                if (index < length - 2)
                {
                    if (sequence[index] == sequence[index + 1] && sequence[index + 1] == sequence[index + 2])
                    {
                        return false;
                    }
                }
            }

            return zeroCount == oneCount && zeroCount > 0;
        }

        private bool TrySolve(Action<IPuzzleSurface> body)
        {
            bool seenChanges = false;

            bool tryAgain;
            do
            {
                sourceSurface.AcceptChanges();
                rotatedSurface.AcceptChanges();

                body(sourceSurface);
                body(rotatedSurface);

                tryAgain = sourceSurface.HasChanges() || rotatedSurface.HasChanges();
                if (!seenChanges && tryAgain)
                {
                    seenChanges = true;
                }
            }
            while (tryAgain);

            return seenChanges;
        }
    }
}
