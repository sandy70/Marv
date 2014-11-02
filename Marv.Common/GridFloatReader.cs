using System;
using System.IO;

namespace Marv
{
    public class GridFloatReader
    {
        private readonly string filePath;

        public GridFloatReader(string filepath)
        {
            this.filePath = filepath;
        }

        public float Read(int row, int col, int nCols)
        {
            using (var binaryReader = new BinaryReader(File.OpenRead(Path.ChangeExtension(this.filePath, "flt"))))
            {
                binaryReader.BaseStream.Seek((((row - 1) * nCols) + (col - 1)) * 4, SeekOrigin.Begin);
                var floatBytes = binaryReader.ReadBytes(4);
                Array.Reverse(floatBytes);
                return BitConverter.ToSingle(floatBytes, 0);
            }
        }
    }
}