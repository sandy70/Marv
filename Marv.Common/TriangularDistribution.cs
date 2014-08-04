namespace Marv.Common
{
    public class TriangularDistribution
    {
        public double Max { get; set; }
        public double Min { get; set; }
        public double Mode { get; set; }

        public double Pdf(double x)
        {
            if (x < this.Min)
            {
                return 0;
            }

            if (this.Min <= x && x <= this.Mode)
            {
                return 2 * (x - this.Min) / (this.Max - this.Min) / (this.Mode - this.Min);
            }

            if (this.Mode <= x && x <= this.Max)
            {
                return 2 * (this.Max - x) / (this.Max - this.Min) / (this.Max - this.Mode);
            }

            return 0;
        }
    }
}