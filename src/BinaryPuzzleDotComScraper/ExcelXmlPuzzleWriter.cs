using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using BinaryPuzzleSolver.Engine;
using BinaryPuzzleSolver.Engine.Utilities;

namespace BinaryPuzzleDotComScraper
{
    public class ExcelXmlPuzzleWriter
    {
        private static readonly Dictionary<int, string> Templates = new Dictionary<int, string>();

        static ExcelXmlPuzzleWriter()
        {
            var templateSizes = new[] { 6, 8, 10, 12, 14 };

            Assembly assembly = typeof(ExcelXmlPuzzleWriter).GetTypeInfo().Assembly;
            foreach (int size in templateSizes)
            {
                string resourceName = typeof(ExcelXmlPuzzleWriter).Namespace +
                    string.Format(".Templates.ExcelXml{0:00}x{0:00}.xml", size);
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        Templates[size] = reader.ReadToEnd();
                    }
                }
            }
        }

        public void Save(string path, IPuzzleSurface puzzle, IPuzzleSurface answer)
        {
            Guard.NotNull(puzzle, nameof(puzzle));
            Guard.NotNull(answer, nameof(answer));

            if (puzzle.LineCount != answer.LineCount || puzzle.ColumnCount != answer.ColumnCount)
            {
                throw new ArgumentException("puzzle and answer must have the same size.");
            }

            if (puzzle.LineCount != puzzle.ColumnCount)
            {
                throw new NotSupportedException("Puzzle height and width must be the same.");
            }
            if (!Templates.ContainsKey(puzzle.LineCount))
            {
                throw new NotSupportedException(string.Format("Puzzle size of {0}x{0} is not supported.", puzzle.LineCount));
            }

            string template = Templates[puzzle.LineCount];
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(template);

            SetCells(puzzle, doc, "Puzzle");
            SetCells(answer, doc, "Answer");

            using (FileStream fileStream = File.Create(path))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream))
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(textWriter,
                        new XmlWriterSettings
                        {
                            Indent = false,
                            NewLineHandling = NewLineHandling.None,
                            OmitXmlDeclaration = false
                        }))
                    {
                        doc.Save(xmlWriter);
                    }
                }
            }
        }

        private void SetCells(IPuzzleSurface surface, XmlDocument doc, string worksheetName)
        {
            const string ns = "urn:schemas-microsoft-com:office:spreadsheet";

            XPathNavigator rootNav = doc.CreateNavigator();
            var namespaceManager = new XmlNamespaceManager(rootNav.NameTable);
            namespaceManager.AddNamespace("ss", ns);

            int rowIndex = 0;
            XPathNodeIterator rowIterator = rootNav.Select(
                "/ss:Workbook/ss:Worksheet[@ss:Name='" + worksheetName + "']/ss:Table/ss:Row", namespaceManager);
            while (rowIterator.MoveNext())
            {
                int columnIndex = 0;
                XPathNodeIterator columnIterator = rowIterator.Current.Select("ss:Cell", namespaceManager);
                while (columnIterator.MoveNext())
                {
                    bool? value = surface.GetCell(rowIndex, columnIndex);
                    if (value != null)
                    {
                        var node = (XmlNode)columnIterator.Current.UnderlyingObject;

                        XmlElement dataElement = doc.CreateElement("Data", ns);
                        XmlAttribute typeAttribute = doc.CreateAttribute("ss", "Type", ns);
                        typeAttribute.Value = "Number";
                        dataElement.Attributes.Append(typeAttribute);
                        dataElement.InnerText = value.Value ? "1" : "0";

                        node.AppendChild(dataElement);
                    }

                    columnIndex++;
                }
                rowIndex++;
            }
        }
    }
}
