using System;
using System.IO;

namespace Marv
{
    public class GridFloatHeader
    {
        public string ByteOrder;
        public double CellSize;
        public int NoDataValue = 9999;
        public double XLL;
        public double YLL;
        public int nCols;
        public int nRows;

        public static GridFloatHeader Read(string filePath)
        {
            var gridFloatHeader = new GridFloatHeader();

            var headerParts = new string[7, 2];
            var j = 0;

            // Read the file and display it line by line.
            using (var file = new StreamReader(filePath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var delimiters = new[] { '\t', ' ' };
                    var parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    var i = 0;
                    foreach (var part in parts)
                    {
                        headerParts[j, i] = part;
                        i++;
                    }

                    j++;
                }
            }

            gridFloatHeader.ByteOrder = headerParts[6, 1];
            gridFloatHeader.CellSize = double.Parse(headerParts[4, 1]);
            gridFloatHeader.nCols = int.Parse(headerParts[0, 1]);
            gridFloatHeader.nRows = int.Parse(headerParts[1, 1]);
            gridFloatHeader.XLL = double.Parse(headerParts[2, 1]);
            gridFloatHeader.YLL = double.Parse(headerParts[3, 1]);

            return gridFloatHeader;
        }
    }
}