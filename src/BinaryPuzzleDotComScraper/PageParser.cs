using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using BinaryPuzzleDotComScraper.Extensions;
using BinaryPuzzleSolver.Engine;
using JetBrains.Annotations;
using SmallCode.AspNetCore.HtmlAgilityPack;

namespace BinaryPuzzleDotComScraper
{
    public sealed class PageParser
    {
        [NotNull]
        private static readonly Regex PageTitleRegex = new Regex(
            @"^(?<height>[0-9]+)x(?<width>[0-9]+)\sBinary\sPuzzles\s-\sBinaryPuzzle\.com$", RegexOptions.Compiled);

        [NotNull]
        private static readonly Regex PuzzleNumberOptionRegex =
            new Regex(@"^<option[^>]*>(?<number>[0-9]+)</option>$", RegexOptions.Compiled);

        [NotNull]
        private static readonly Regex ScriptAssignmentRegex =
            new Regex(@"^(?<varName>puzzel|oplossing)\[(?<rowIndex>[0-9]+)\]\[(?<columnIndex>[0-9]+)\]\s*=\s*'(?<value>[10-])';$",
                RegexOptions.Compiled);

        [NotNull]
        public IPuzzleSurface Puzzle { get; }

        [NotNull]
        public IPuzzleSurface Answer { get; }

        public PuzzleDifficulty Difficulty { get; private set; }

        public int Number { get; private set; }

        public PageParser([NotNull] string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            XPathNavigator rootNavigator = doc.CreateNavigator();

            XPathNavigator titleNavigator = rootNavigator.SelectSingleNode("/html/head/title");
            Match titleMatch = PageTitleRegex.Match(titleNavigator.Value);
            if (!titleMatch.Success)
            {
                throw new Exception("Failed to read puzzle size from page title.");
            }
            int height = int.Parse(titleMatch.Groups["height"].Value);
            int width = int.Parse(titleMatch.Groups["width"].Value);

            Puzzle = new PuzzleSurface(height, width);
            Answer = new PuzzleSurface(height, width);

            ParseDifficulty(html);
            ParsePuzzleNumber(html);

            XPathNodeIterator scriptIterator = rootNavigator.Select("//script[contains(text(),'var oplossing')]");
            if (scriptIterator.MoveNext())
            {
                string scriptText = scriptIterator.Current.Value;
                ParseScript(scriptText);
            }
        }

        private void ParseDifficulty([NotNull] string html)
        {
            // Due to a bug in HtmlAgilityPack we cannot use:
            // rootNavigator.Select("//select[@name='level']/option[@selected='selected']");

            using (TextReader reader = new StringReader(html))
            {
                string selectedOption = reader.GetLines().Select(line => line.Trim())
                    .SkipWhile(line => !line.StartsWith("<select", StringComparison.Ordinal) ||
                        line.IndexOf("name=\"level\"", StringComparison.Ordinal) == -1)
                    .TakeWhile(line => !line.StartsWith("</select>", StringComparison.Ordinal))
                    .Single(line => line.StartsWith("<option", StringComparison.Ordinal) && line.Contains("selected"));

                if (selectedOption.IndexOf("Easy", StringComparison.Ordinal) != -1)
                {
                    Difficulty = PuzzleDifficulty.Easy;
                }
                else if (selectedOption.IndexOf("Medium", StringComparison.Ordinal) != -1)
                {
                    Difficulty = PuzzleDifficulty.Medium;
                }
                else if (selectedOption.IndexOf("Hard", StringComparison.Ordinal) != -1)
                {
                    Difficulty = PuzzleDifficulty.Hard;
                }
                else if (selectedOption.IndexOf("Very hard", StringComparison.Ordinal) != -1)
                {
                    Difficulty = PuzzleDifficulty.VeryHard;
                }
                else
                {
                    throw new Exception($"Unknown difficulty on line: \"{selectedOption}\".");
                }
            }
        }

        private void ParsePuzzleNumber([NotNull] string html)
        {
            // Due to a bug in HtmlAgilityPack we cannot use:
            // rootNavigator.Select("//select[@name='nr']/option[@selected='selected']");

            using (TextReader reader = new StringReader(html))
            {
                string selectedOption = reader.GetLines().Select(line => line.Trim())
                    .SkipWhile(line => !line.StartsWith("<select", StringComparison.Ordinal) ||
                        line.IndexOf("name=\"nr\"", StringComparison.Ordinal) == -1)
                    .TakeWhile(line => !line.StartsWith("</select>", StringComparison.Ordinal))
                    .Single(line => line.StartsWith("<option", StringComparison.Ordinal) && line.Contains("selected"));

                Match match = PuzzleNumberOptionRegex.Match(selectedOption);
                if (!match.Success)
                {
                    throw new Exception($"Failed to read puzzle number on line: \"{selectedOption}\".");
                }

                Number = int.Parse(match.Groups["number"].Value);
            }
        }

        private void ParseScript([NotNull] string script)
        {
            using (TextReader reader = new StringReader(script))
            {
                IEnumerable<string> lines = reader.GetLines().Select(line => line.Trim())
                    .SkipWhile(line => !line.StartsWith("puzzel[0][0]", StringComparison.Ordinal))
                    .TakeWhile(line => !line.StartsWith("function", StringComparison.Ordinal));

                foreach (string line in lines)
                {
                    Match match = ScriptAssignmentRegex.Match(line);
                    if (match.Success)
                    {
                        string varName = match.Groups["varName"].Value;
                        int rowIndex = int.Parse(match.Groups["rowIndex"].Value);
                        int columnIndex = int.Parse(match.Groups["columnIndex"].Value);
                        bool? cell = ParseCell(match.Groups["value"].Value);

                        if (cell != null)
                        {
                            if (varName == "puzzel")
                            {
                                Puzzle.SetCell(rowIndex, columnIndex, cell.Value);
                            }
                            else
                            {
                                Answer.SetCell(rowIndex, columnIndex, cell.Value);
                            }
                        }
                    }
                }
            }
        }

        [CanBeNull]
        private bool? ParseCell([CanBeNull] string value)
        {
            return value == "1" ? true : value == "0" ? false : (bool?)null;
        }
    }
}
