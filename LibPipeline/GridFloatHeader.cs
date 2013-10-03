using System;
using System.IO;

namespace LibPipeline
{
    public class GridFloatHeader
    {
        public string ByteOrder;
        public double CellSize;
        public int nCols;
        public int NoDataValue = 9999;
        public int nRows;
        public double XLL;
        public double YLL;

        public static GridFloatHeader Read(string filePath)
        {
            GridFloatHeader gridFloatHeader = new GridFloatHeader();

            string line;
            string[,] headerParts = new string[7, 2];
            int j = 0;

            // Read the file and display it line by line.
            using (StreamReader file = new StreamReader(filePath))
            {
                while ((line = file.ReadLine()) != null)
                {
                    char[] delimiters = new char[] { '\t', ' ' };
                    string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    int i = 0;
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