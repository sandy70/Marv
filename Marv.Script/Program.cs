using System;
using Marv.Common;

namespace Marv.Script
{
    public static class Program
    {
        const string VertexKey = "CR";

        private static void Main(string[] args)
        {
            var lineData = LineData.Read(@"C:\Users\vkha\Downloads\Data.marv-linedata") as LineData;
            var network = Network.Read(@"C:\Users\vkha\Downloads\Network.net");

            using (var file = new System.IO.StreamWriter(@"C:\Users\vkha\Downloads\Percentiles.csv"))
            {
                foreach (var sectionId in lineData.SectionIds)
                {

                    var evidences = lineData.SectionEvidences[sectionId][2015];

                    var beliefs = network.Run(evidences);

                    var corrosionRate = beliefs[VertexKey];

                    if (corrosionRate == null)
                    {
                        continue;
                    }

                    var percentileComputer50 = new VertexPercentileComputer(0.5);
                    var percentileComputer99 = new VertexPercentileComputer(0.99);

                    var percentile50 = percentileComputer50.Compute(network.Vertices[VertexKey], corrosionRate, null);
                    var percentile99 = percentileComputer99.Compute(network.Vertices[VertexKey], corrosionRate, null);

                    file.WriteLine(sectionId + ", " + percentile50 + ", " + percentile99);
                }
            }


            Console.ReadKey();
        }
    }
}