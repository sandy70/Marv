using System;
using System.IO;
using System.Linq;
using Marv;
using Marv.Common;

namespace FixPoF
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // UpdatePoFFrancois();
           //  WritePofFrancois();

            var filePaths = Directory.EnumerateFiles(@"C:\Users\vkha\Downloads\Data\LineData WaterCut 1\SectionBeliefs", "*.marv-sectionbelief");
            // var locationValues = new Dict<string, int, double>();

            foreach (var filePath in filePaths)
            {
                var sectionBelief = Utils.ReadJson<Dict<int, string, double[]>>(filePath);
                var sectionId = Path.GetFileNameWithoutExtension(filePath);

                foreach (var year in sectionBelief.Keys)
                {
                    sectionBelief[year]["Temperature"] = new double[] { 1, 0, 0, 0 };
                }

                sectionBelief.WriteJson(filePath);
                Console.WriteLine("Read file: " + filePath);
            }

            // locationValues.WriteJson(@"C:\Users\vkha\Downloads\LocationValues.json");

            Console.ReadKey();
        }

        private static void WritePof()
        {
            using (var streamWriter = new StreamWriter(@"C:\Users\vkha\Downloads\pof.csv"))
            {
                var filePaths = Directory.EnumerateFiles(@"C:\Users\vkha\Source\Marv\Marv\bin\Debug\Data\LineData\SectionBeliefs", "*.marv-sectionbelief");

                foreach (var filePath in filePaths)
                {
                    var sectionBelief = Utils.ReadJson<Dict<int, string, double[]>>(filePath);

                    foreach (var year in sectionBelief.Keys)
                    {
                        streamWriter.Write(sectionBelief[year]["pof"][0] + ",");
                    }

                    streamWriter.WriteLine();
                    Console.WriteLine("Wrote: " + filePath);
                }
            }
        }

        private static void WritePofFrancois()
        {
            using (var streamWriter = new StreamWriter(@"C:\Users\vkha\Downloads\pof.csv"))
            {
                var filePaths = Directory.EnumerateFiles(@"C:\Users\vkha\Source\Marv\Marv\bin\Debug\Data\LineData\SectionBeliefs", "*.marv-sectionbelief");

                foreach (var filePath in filePaths)
                {
                    var sectionBelief = Utils.ReadJson<Dict<int, string, double[]>>(filePath);

                    foreach (var year in sectionBelief.Keys)
                    {
                        var nof = sectionBelief[year]["nof"];
                        // var nofStates = new[] { 55000, 5500, 550, 55, 5.5, 0.5 };
                        var nofStates = new[] { 0, 0, 100, 10, 1, 0 };
                        var pgpc = sectionBelief[year]["exceed"];

                        var pof = new[] { 0.5, 0.5 };
                        var sum = nof.Select((x, i) => x * nofStates[i]).Sum();
                        pof[0] = 1 - (1 - pgpc[0]) * sum;

                        if (pof[0] > 1)
                        {
                            Console.WriteLine("pof > 1");
                            pof[0] = 1;
                        }

                        pof[1] = 1 - pof[0];

                        sectionBelief[year]["pof"] = pof;

                        streamWriter.Write(sectionBelief[year]["pof"][0] + ",");
                    }

                    streamWriter.WriteLine();
                    Console.WriteLine("Wrote: " + filePath);
                }
            }
        }

        private static void UpdatePoF()
        {
            var filePaths = Directory.EnumerateFiles(@"C:\Users\vkha\Source\Marv\Marv\bin\Debug\Data\LineData\SectionBeliefs", "*.marv-sectionbelief");

            foreach (var filePath in filePaths)
            {
                var sectionBelief = Utils.ReadJson<Dict<int, string, double[]>>(filePath);

                foreach (var year in sectionBelief.Keys)
                {
                    var nof = sectionBelief[year]["nof"];
                    var nofStates = new[] { 55000, 5500, 550, 55, 5.5, 0.5 };
                    var pgpc = sectionBelief[year]["exceed"];

                    var pof = new[] { 0.5, 0.5 };
                    pof[0] = nof.Select((x, i) => x * nofStates[i]).Sum() * pgpc[0] / 4;

                    if (pof[0] > 1)
                    {
                        Console.WriteLine("pof > 1");
                        pof[0] = 1;
                    }

                    pof[1] = 1 - pof[0];

                    sectionBelief[year]["pof"] = pof;
                }

                sectionBelief.WriteJson(filePath);
                Console.WriteLine("Wrote file: " + filePath);
            }
        }

        private static void UpdatePoFFrancois()
        {
            var filePaths = Directory.EnumerateFiles(@"C:\Users\vkha\Source\Marv\Marv\bin\Debug\Data\LineData\SectionBeliefs", "*.marv-sectionbelief");

            foreach (var filePath in filePaths)
            {
                var sectionBelief = Utils.ReadJson<Dict<int, string, double[]>>(filePath);

                foreach (var year in sectionBelief.Keys)
                {
                    var nof = sectionBelief[year]["nof"];
                    // var nofStates = new[] { 55000, 5500, 550, 55, 5.5, 0.5 };
                    var nofStates = new[] { 0, 0, 100, 10, 1, 0 };
                    var pgpc = sectionBelief[year]["exceed"];

                    var pof = new[] { 0.5, 0.5 };
                    var sum = nof.Select((x, i) => x * nofStates[i]).Sum();
                    pof[0] = 1 - Math.Pow(1 - pgpc[0], sum);

                    if (pof[0] > 1)
                    {
                        Console.WriteLine("pof > 1");
                        pof[0] = 1;
                    }

                    pof[1] = 1 - pof[0];

                    sectionBelief[year]["pof"] = pof;
                }

                sectionBelief.WriteJson(filePath);
                Console.WriteLine("Wrote file: " + filePath);
            }
        }
    }
}