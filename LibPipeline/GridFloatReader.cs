using System;
using System.IO;

namespace LibPipeline
{
    public class GridFloatReader
    {
        private string FilePath;

        public GridFloatReader(string filepath)
        {
            this.FilePath = filepath;
        }

        public float Read(int row, int col, int nCols)
        {
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(Path.ChangeExtension(this.FilePath, "flt"))))
            {
                binaryReader.BaseStream.Seek((((row - 1) * nCols) + (col - 1)) * 4, SeekOrigin.Begin);
                byte[] floatBytes = binaryReader.ReadBytes(4);
                Array.Reverse(floatBytes);
                return BitConverter.ToSingle(floatBytes, 0);
            }
        }

        public GridFloatHeader ReadHeader()
        {
            GridFloatHeader gridFloatHeader = new GridFloatHeader();

            string line;
            string[,] headerParts = new string[7, 2];
            int j = 0;

            // Read the file and display it line by line.
            using (StreamReader file = new StreamReader(this.FilePath))
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
                file.Close();
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