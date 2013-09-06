using System;
using System.IO;

namespace LibPipeline
{
    public class ElevationReader
    {
        //public double Read(double lat, double lon)
        //{
        //    string filename = String.Format("n{0:D2}w{1:D3}.hdr", (int)Math.Ceiling(lat), -(int)Math.Floor(lon));
        //    string filepath = Path.Combine(griddir, filename);

        //    GridFloatReader gridFloatReader = new GridFloatReader(filepath);
        //    GridFloatHeader gridFloatHeader = gridFloatReader.ReadHeader();

        //    int row = (int)Math.Round((lat - gridFloatHeader.YLL) / gridFloatHeader.CellSize);
        //    int col = (int)Math.Round((lon - gridFloatHeader.XLL) / gridFloatHeader.CellSize);
        //    row = gridFloatHeader.nRows - row;

        //    return gridFloatReader.Read(row, col, gridFloatHeader.nCols);
        //}
    }
}