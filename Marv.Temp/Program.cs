using CsvHelper;
using Marv.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var marvToSynergi = Utils.ReadJson<Dict<string, string>>(@"D:\Data\ADCO02\MarvToSynergiMap.json");

            var location = line["BU-498"];
            var graphEvidence = pipelineInput.GetGraphEvidence(graph, line.Name, location.Name);

            foreach (var vertexKey in marvToSynergi.Keys)
            {
                 Console.WriteLine("{0} {1}", marvToSynergi[vertexKey], graphEvidence[vertexKey].SynergiString);
            }

            Console.ReadKey();
        }
    }
}
