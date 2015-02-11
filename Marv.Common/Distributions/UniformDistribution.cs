namespace Marv
{
    public class UniformDistribution : IDistribution
    {
        private readonly double max;
        private readonly double min;

        public UniformDistribution(double theMin, double theMax)
        {
            if (theMin > theMax)
            {
                var temp = theMin;
                theMin = theMax;
                theMax = temp;
            }

            this.min = theMin;
            this.max = theMax;
        }

        public double Cdf(double x)
        {
            if (x < this.min)
            {
                return 0;
            }

            if (this.min <= x && x <= this.max)
            {
                return (x - this.min) / (this.max - this.min);
            }

            return 1;
        }

        public double Pdf(double x)
        {
            if (this.min <= x && x <= this.max)
            {
                return 1 / (this.max - this.min);
            }

            return 0;
        }
    }
}