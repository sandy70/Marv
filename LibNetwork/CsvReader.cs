using System;
using System.IO;
using System.Threading.Tasks;

namespace LibNetwork
{
    public class CsvReader
    {
        public char Delimiter = ',';
        private string FilePath;

        public CsvReader(string filePath)
        {
            this.FilePath = filePath;
        }

        public string[,] Read()
        {
            // We have to do one pass to determine the number of rows and columns
            StreamReader sr = new StreamReader(this.FilePath);

            string[,] strArray = null;

            int nRows = 0;
            int nCols = 0;

            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split(this.Delimiter);
                if (nCols < line.Length)
                {
                    nCols = line.Length;
                }

                nRows++;
            }

            // Allocate object
            strArray = new String[nRows, nCols];
            sr.Close();

            // We do pass 2 to read the values.
            sr = new StreamReader(this.FilePath);
            for (int r = 0; r < nRows; r++)
            {
                string[] line = sr.ReadLine().Split(this.Delimiter);

                for (int c = 0; c < line.Length; c++)
                {
                    strArray[r, c] = line[c];
                }
            }

            return strArray;
        }

        public async Task<string[,]> ReadAsync()
        {
            // We have to do one pass to determine the number of rows and columns
            StreamReader sr = new StreamReader(this.FilePath);

            string[,] strArray = null;

            int nRows = 0;
            int nCols = 0;

            while (!sr.EndOfStream)
            {
                string lines = await sr.ReadLineAsync();
                string[] line = lines.Split(this.Delimiter);

                if (nCols < line.Length)
                {
                    nCols = line.Length;
                }

                nRows++;
            }

            // Allocate object
            strArray = new String[nRows, nCols];
            sr.Close();

            // We do pass 2 to read the values.
            sr = new StreamReader(this.FilePath);
            for (int r = 0; r < nRows; r++)
            {
                string lines = await sr.ReadLineAsync();
                string[] line = lines.Split(this.Delimiter);

                for (int c = 0; c < line.Length; c++)
                {
                    strArray[r, c] = line[c];
                }
            }

            return strArray;
        }

        public double[,] ReadDouble()
        {
            string[,] data = this.Read();

            int nRows = data.GetLength(0);
            int nCols = data.GetLength(1);

            double[,] doubleData = new double[nRows, nCols];

            for (int r = 0; r < nRows; r++)
            {
                for (int c = 0; c < nCols; c++)
                {
                    Double.TryParse(data[r, c], out doubleData[r, c]);
                }
            }

            return doubleData;
        }
    }
}