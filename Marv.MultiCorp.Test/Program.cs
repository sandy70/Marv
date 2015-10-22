using System;

namespace Marv.MultiCorp.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Utils.Initialize();

            for (var i = 0; i < 100; i++)
            {
                Console.WriteLine(Utils.ComputeFlow(new FlowParameters
                {
                    MixtureVelocity = 0.2372,
                    OilViscosity = 0.0268,
                    OilDensity = 805.19,
                    InternalDiameter = 16 / 39.37,
                    InterfacialTension = 0.125,
                    Inclination = -1.4038,
                    WaterCut = 3.0267 / 100
                }).Wetting);
            }

            Console.ReadKey();
        }
    }
}