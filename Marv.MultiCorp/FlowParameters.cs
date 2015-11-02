using edu.ohiou.icmt.multicorp.basemodel;

namespace Marv.MultiCorp
{
    public class FlowParameters
    {
        public FlowModel.FlowType FlowType = FlowModel.FlowType.Gas_Oil_Water_Flow;
        public double GasDensity = 25; // kg / m^3
        public double GasLiquidSurfaceTension = 0.0721; // N/m
        public FlowModel.GasPropertiesInput GasPropertiesInputType = FlowModel.GasPropertiesInput.Input;
        public double GasViscosity = 1.714E-5;
        public double Inclination = 0; // deg
        public double InterfacialTension = 0.04; // N/m
        public double InternalDiameter = 0.1; // m
        public double MixtureVelocity = 1; // m/s
        public double OilDensity = 825; // kg/m^3
        public double OilViscosity = 0.002; // Pa.s
        public double PipeConductivity = 60; // W / (m * K)
        public double PipeRoughness = 20E-6; // m
        public double PipeThickness = 0.02; // m
        public double SuperficialGasVelocity = 2; // m/s
        public double SuperficialWaterVelocity = 2; // m / s
        public FlowModel.VelTypes VelocityInputType = FlowModel.VelTypes.Mixture;
        public double WaterCut = 0.05; // %
        public double WaterDensity = 1000; // kg / m^3
        public double WaterViscosity = 0.001; // Pa.s
    }
}