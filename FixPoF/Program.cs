using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marv;

namespace FixPoF
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePaths = Directory.EnumerateFiles(@"C:\Users\vkha\Source\Marv\Marv.Input\bin\Debug\Data\LineData\SectionBeliefs", "*.marv-sectionbelief");

            foreach (var filePath in filePaths)
            {
                var sectionBelief = Utils.ReadJson<Dict<int, string, double[]>>(filePath);

                foreach (var year in sectionBelief.Keys)
                {
                    var nof = sectionBelief[year]["nof"];
                    var nofStates = new[] { 55000, 5500, 550, 55, 5.5, 0.5 };
                    var pgpc = sectionBelief[year]["exceed"];

                    var pof = new[] { 0.5, 0.5 };
                    pof[0] = nof.Select((x, i) => x * nofStates[i]).Normalized().Sum() * pgpc[0];
                    pof[1] = 1 - pof[0];

                    sectionBelief[year]["pof"] = pof;
                }

                sectionBelief.WriteJson(filePath);
                Console.WriteLine("Wrote file: " + filePath);
            }

            Console.ReadKey();
        }
    }
}
