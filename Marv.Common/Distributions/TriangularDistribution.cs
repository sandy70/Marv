using System;

namespace Marv.Common
{
    public class TriangularDistribution : IDistribution
    {
        private readonly double max;
        private readonly double min;
        private readonly double mode;

        public TriangularDistribution(double theMin, double theMode, double theMax)
        {
            this.min = theMin;
            this.mode = theMode;
            this.max = theMax;
        }

        public double Cdf(double x)
        {
            if (x < this.min)
            {
                return 0;
            }

            if (this.min <= x && x <= this.mode)
            {
                return Math.Pow(x - this.min, 2) / (this.max - this.min) / (this.mode - this.min);
            }

            if (this.mode <= x && x <= this.max)
            {
                return 1 - Math.Pow(this.max - x, 2) / (this.max - this.min) / (this.max - this.mode);
            }

            return 1;
        }

        public double Pdf(double x)
        {
            if (x < this.min)
            {
                return 0;
            }

            if (this.min <= x && x <= this.mode)
            {
                return 2 * (x - this.min) / (this.max - this.min) / (this.mode - this.min);
            }

            if (this.mode <= x && x <= this.max)
            {
                return 2 * (this.max - x) / (this.max - this.min) / (this.max - this.mode);
            }

            return 0;
        }
    }
}