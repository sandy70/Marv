namespace Marv.Common
{
    public interface IDistribution
    {
        double Cdf(double x);
        double Pdf(double x);
    }
}