using Newtonsoft.Json;
using NLog;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibNetwork
{
    public class GraphValue : Dictionary<string, VertexValue>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static GraphValue ReadCsv(string fileName, Graph graph)
        {
            var graphValue = new GraphValue();

            foreach (var line in File.ReadLines(fileName))
            {
                var vertexValue = new VertexValue();

                string[] parts = line.Split(new char[] { ',' });

                var vertexKey = parts[0];

                var vertex = graph.GetVertex(vertexKey);

                int nParts = parts.Count();

                for (int p = 1; p < nParts; p++)
                {
                    // This assumes that states order is preserved
                    var stateKey = vertex.States[p - 1].Key;

                    vertexValue[stateKey] = Double.Parse(parts[p]);
                }

                graphValue[vertexKey] = vertexValue;
            }

            return graphValue;
        }

        public void WriteJson(string fileName)
        {
            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (var streamWriter = new StreamWriter(fileName))
            {
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(jsonTextWriter, this);
                }
            }
        }

        public void WriteXlsx(string fileName)
        {
            var excelPackage = new ExcelPackage(new FileInfo(fileName));
            var excelWorkSheetName = "Sheet1";

            try
            {
                excelPackage.Workbook.Worksheets.Add(excelWorkSheetName);
            }
            catch (InvalidOperationException exp)
            {
                logger.Warn("The worksheet {0} already exists.", excelWorkSheetName);
            }

            var excelWorkSheet = excelPackage.Workbook.Worksheets[excelWorkSheetName];

            var excelCol = 1;

            foreach (var vertexKey in this.Keys)
            {
                var excelRow = 1;

                excelWorkSheet.SetValue(excelRow++, excelCol, vertexKey);

                var vertexValue = this[vertexKey];

                foreach (var stateKey in vertexValue.Keys)
                {
                    excelWorkSheet.SetValue(excelRow++, excelCol, vertexValue[stateKey]);
                }

                excelCol++;
            }

            excelPackage.Save();
        }
    }
}