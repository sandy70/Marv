using System;
using edu.ohiou.icmt.multicorp.basemodel;

namespace Marv.MultiCorp.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Utils.Initialize();

            var gasOilWaterFlowParams = new FlowParameters();

            gasOilWaterFlowParams.FlowType = FlowModel.FlowType.Gas_Oil_Water_Flow;

            gasOilWaterFlowParams.InternalDiameter = 33.95 * 0.0254;
            gasOilWaterFlowParams.PipeThickness = 1.025 * 0.0254;

            gasOilWaterFlowParams.VelocityInputType = FlowModel.VelTypes.Mixture;

            gasOilWaterFlowParams.Inclination = 0;
            gasOilWaterFlowParams.SuperficialGasVelocity = 0.1;
            gasOilWaterFlowParams.MixtureVelocity = 1;
            gasOilWaterFlowParams.WaterCut = 20.0 / 100;

            gasOilWaterFlowParams.OilDensity = 825;
            gasOilWaterFlowParams.OilViscosity = 0.002;
            gasOilWaterFlowParams.InterfacialTension = 0.04;

            Console.WriteLine(Utils.ComputeFlow(gasOilWaterFlowParams).Pattern);



            gasOilWaterFlowParams.Inclination = 0;
            gasOilWaterFlowParams.SuperficialGasVelocity = 0.1;
            gasOilWaterFlowParams.MixtureVelocity = 10;
            gasOilWaterFlowParams.WaterCut = 20.0 / 100;

            Console.WriteLine(Utils.ComputeFlow(gasOilWaterFlowParams).Pattern);



            gasOilWaterFlowParams.Inclination = 0;
            gasOilWaterFlowParams.SuperficialGasVelocity = 10;
            gasOilWaterFlowParams.MixtureVelocity = 1;
            gasOilWaterFlowParams.WaterCut = 20.0 / 100;

            Console.WriteLine(Utils.ComputeFlow(gasOilWaterFlowParams).Pattern);

            Console.ReadKey();
        }
    }
}