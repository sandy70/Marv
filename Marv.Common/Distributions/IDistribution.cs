namespace Marv.Common.Distributions
{
    public interface IDistribution
    {
        double Cdf(double x);
        double Pdf(double x);
    }
}