using System;
using System.IO;

namespace Marv.Common
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
            using (var binaryReader = new BinaryReader(File.OpenRead(Path.ChangeExtension(this.FilePath, "flt"))))
            {
                binaryReader.BaseStream.Seek((((row - 1) * nCols) + (col - 1)) * 4, SeekOrigin.Begin);
                var floatBytes = binaryReader.ReadBytes(4);
                Array.Reverse(floatBytes);
                return BitConverter.ToSingle(floatBytes, 0);
            }
        }
    }
}