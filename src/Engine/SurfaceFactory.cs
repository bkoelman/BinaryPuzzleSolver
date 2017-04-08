using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BinaryPuzzleSolver.Engine.Utilities;

namespace BinaryPuzzleSolver.Engine
{
    /// <summary>
    /// Provides methods to construct an <see cref="IPuzzleSurface" /> object from text or a file.
    /// </summary>
    public class SurfaceFactory
    {
        public IPuzzleSurface CreateFromExcelXmlFile(string path, string worksheetName)
        {
            Guard.NotNull(path, nameof(path));

            const string ns = "{urn:schemas-microsoft-com:office:spreadsheet}";

            XDocument doc = XDocument.Load(path);

            XElement tableElement = (from worksheet in doc.Elements(ns + "Workbook").Elements(ns + "Worksheet")
                where worksheet.Attribute(ns + "Name").Value == worksheetName
                select worksheet.Element(ns + "Table")).Single();

            var textLines = new List<string>();
            foreach (XElement rowElement in tableElement.Elements(ns + "Row"))
            {
                var lineBuilder = new StringBuilder();
                foreach (XElement cellElement in rowElement.Elements(ns + "Cell"))
                {
                    string value = cellElement.Value == "" ? "-" : cellElement.Value;
                    lineBuilder.Append(value);
                }
                textLines.Add(lineBuilder.ToString());
            }

            return CreateFromText(textLines.ToArray());
        }

        public IPuzzleSurface CreateFromText(params string[] lines)
        {
            Guard.NotNullNorEmpty(lines, nameof(lines));

            if (lines.Length % 2 != 0)
            {
                throw new ArgumentException("lines must have an even length.", nameof(lines));
            }

            int? columnCount = null;
            bool?[,] cells = null;

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];
                if (line == null)
                {
                    throw new ArgumentNullException(string.Format("lines[{0}]", lineIndex));
                }

                if (line.Length == 0)
                {
                    throw new ArgumentException(string.Format("Lines[{0}] must not be empty.", lineIndex), nameof(lines));
                }

                if (line.Length % 2 != 0)
                {
                    throw new ArgumentException(string.Format("Lines[{0}] must have an even length.", lineIndex), nameof(lines));
                }

                bool?[] columnValues = CreateFromLine(line, lineIndex);

                if (columnCount == null)
                {
                    // Lazy allocation.
                    columnCount = columnValues.Length;
                }

                if (columnCount != columnValues.Length)
                {
                    throw new ArgumentException("All lines must have the same length.", nameof(lines));
                }

                if (cells == null)
                {
                    // Lazy allocation.
                    cells = new bool?[lines.Length, columnCount.Value];
                }

                // Copy cell values for current line.
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    cells[lineIndex, columnIndex] = columnValues[columnIndex];
                }
            }

            return new PuzzleSurface(cells);
        }

        private bool?[] CreateFromLine(string line, int lineIndex)
        {
            var result = new bool?[line.Length];
            for (int columnIndex = 0; columnIndex < line.Length; columnIndex++)
            {
                char ch = line[columnIndex];
                if (ch == '0')
                {
                    result[columnIndex] = false;
                }
                else if (ch == '1')
                {
                    result[columnIndex] = true;
                }
                else if (ch != '-')
                {
                    throw new ArgumentOutOfRangeException(nameof(line), line,
                        string.Format("Found character other than 0, 1 or space on line {0} at position {1}.", lineIndex,
                            columnIndex));
                }
            }
            return result;
        }
    }
}
