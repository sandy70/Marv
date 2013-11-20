using Marv.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFileName = @"D:\Data\ADCO02\ADCO 7.xlsx";
            var networkFileName = @"D:\Data\ADCO02\ADCO_08.net";

            var pipelineInput = new PipelineInput(inputFileName);
            var line = pipelineInput.ReadPipelines()["BU-498"];
            var graph = Graph.Read(networkFileName);

            foreach(var location in line)
            {
                var graphEvidence = pipelineInput.GetGraphEvidence(graph, line.Name, location.Name);
            }

            Console.ReadKey();
        }
    }
}
