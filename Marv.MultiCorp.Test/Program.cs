using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using edu.ohiou.icmt;
using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.modeling.param;
using edu.ohiou.icmt.multicorp.basemodel;
using edu.ohiou.icmt.multicorp.factory;

namespace Marv.MultiCorp.Test
{
    internal static class Program
    {
        private static void Main()
        {
            MulticorpRunner.initialize();

            Console.WriteLine(Utils.ComputeFlow(new FlowParameters
            {
                SuperficialGasVelocity = 0.1,
                MixtureVelocity = 10,
                WaterCut = 0.2
            }).Pattern);

            NameList.FLOW_PATTERN_ANNULAR

            Console.ReadKey();
        }
    }
}