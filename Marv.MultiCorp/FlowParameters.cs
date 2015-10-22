using edu.ohiou.icmt.multicorp.basemodel;

namespace Marv.MultiCorp
{
    public class FlowParameters
    {
        public FlowModel.FlowType FlowType = FlowModel.FlowType.Gas_Oil_Water_Flow;
        public double GasDensity = 7.54; // kg / m^3
        public double GasLiquidSurfaceTension = 0.0721; // N/m
        public double Inclination = 0; // deg
        public double InterfacialTension = 0.04; // N/m
        public double InternalDiameter = 0.1; // m
        public double MixtureVelocity = 1; // m/s
        public double OilDensity = 825; // kg/m^3
        public double OilViscosity = 0.002; // Pa.s
        public double PipeThickness = 0.02; // m
        public double SuperficialGasVelocity = 2; // m/s
        public double SuperficialWaterVelocity = 0.2; // m / s
        public double WaterCut = 0.05; // %
        public double WaterDensity = 1000; // kg / m^3
        public double WaterViscosity = 0.001; // Pa.s

        public void Set(AbstractCase abstractCase) {}
    }
}