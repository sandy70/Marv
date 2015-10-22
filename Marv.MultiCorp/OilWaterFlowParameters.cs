using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.multicorp.basemodel;

namespace Marv.MultiCorp
{
    public class OilWaterFlowParameters : IFlowParameters
    {
        public double Inclination = 0; // deg
        public double InterfacialTension = 0.04; // N/m
        public double InternalDiameter = 0.1; // m
        public double MixtureVelocity = 1; // m/s
        public double OilDensity = 825; // kg/m^3
        public double OilViscosity = 0.002; // Pa.s
        public double WaterCut = 0.05; // %

        public void Set(AbstractCase cCase)
        {
            cCase.getParameter(NameList.INPUT_TYPE).setValue(FlowModel.InputTypes.Velocity);
            cCase.getParameter(NameList.VELOCITY_INPUT_TYPE).setValue(FlowModel.VelTypes.Mixture);
            cCase.getParameter(NameList.SECTION_DIAMETER).setValue(this.InternalDiameter);
            cCase.getParameter(NameList.SECTION_INCLINATION).setValue(this.Inclination);
            cCase.getParameter(NameList.INTERFICIAL_TENSION).setValue(this.InterfacialTension);
            cCase.getParameter(NameList.MIXTURE_VELOCITY).setValue(this.MixtureVelocity);
            cCase.getParameter(NameList.OIL_DENSITY).setValue(this.OilDensity);
            cCase.getParameter(NameList.OIL_VISCOSITY).setValue(this.OilViscosity);
            cCase.getParameter(NameList.WATER_CUT).setValue(this.WaterCut);
        }
    }
}