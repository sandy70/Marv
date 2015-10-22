using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.multicorp.basemodel;

namespace Marv.MultiCorp
{
    public class GasWaterFlowParameters : IFlowParameters
    {
        public double GasDensity = 7.54; // kg / m^3
        public double GasLiquidSurfaceTension = 0.0721; // N/m
        public double Inclination = 0; // deg
        public double InternalDiameter = 0.1; // m
        public double SuperficialGasVelocity = 2; // m/s
        public double SuperficialWaterVelocity = 0.2; // m / s
        public double WaterDensity = 1000; // kg / m^3
        public double WaterViscosity = 0.001; // Pa.s

        public void Set(AbstractCase abstractCase)
        {
            abstractCase.getParameter(NameList.SECTION_DIAMETER).setValue(this.InternalDiameter);
            abstractCase.getParameter(NameList.SECTION_INCLINATION).setValue(this.Inclination);
            abstractCase.getParameter(NameList.GAS_LIQUID_SURFACE_TENSION).setValue(this.GasLiquidSurfaceTension);
            abstractCase.getParameter(NameList.SUPERFICIAL_GAS_VELOCITY).setValue(this.SuperficialGasVelocity);
            abstractCase.getParameter(NameList.GAS_DENSITY).setValue(this.GasDensity);
            abstractCase.getParameter(NameList.WATER_DENSITY).setValue(this.WaterDensity);
            abstractCase.getParameter(NameList.WATER_VISCOSITY).setValue(this.WaterViscosity);
            abstractCase.getParameter(NameList.SUPERFICIAL_WATER_VELOCITY).setValue(this.SuperficialWaterVelocity);
        }
    }
}