namespace Marv.Common
{
    public class DeltaDistribution : IDistribution
    {
        private readonly double mean;

        public DeltaDistribution(double theMean)
        {
            this.mean = theMean;
        }

        public double Cdf(double x)
        {
            return x < this.mean ? 0 : 1;
        }

        public double Pdf(double x)
        {
            return x == this.mean ? 1 : 0;
        }
    }
}