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
    }
}