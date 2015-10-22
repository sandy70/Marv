using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.modeling.param;
using edu.ohiou.icmt.multicorp.basemodel;

namespace Marv.MultiCorp
{
    public class GasOilWaterFlowParameters : IFlowParameters
    {
        public double GasDensity = 7.54; // kg / m^3
        public double Inclination = 0; // deg
        public double InterfacialTension = 0.04; // N/m
        public double InternalDiameter = 0.1; // m
        public double MixtureVelocity = 1; // m/s
        public double OilDensity = 825; // kg/m^3
        public double OilViscosity = 0.002; // Pa.s
        public double PipeThickness = 0.02; // m
        public double SuperficialGasVelocity = 2; // m/s
        public double WaterCut = 0.05; // %

        public void Set(AbstractCase abstractCase)
        {
            //Set Flow type
            (abstractCase.getParameter(NameList.FLOW_TYPE) as OptionParameter).setOption((int)FlowModel.FlowType.Gas_Oil_Water_Flow);
            abstractCase.onFlowTypeChanged();

            abstractCase.getParameter(NameList.GAS_DENSITY).setValue(this.GasDensity);
            abstractCase.getParameter(NameList.SECTION_DIAMETER).setValue(this.InternalDiameter);
            abstractCase.getParameter(NameList.SECTION_INCLINATION).setValue(this.Inclination);
            abstractCase.getParameter(NameList.INTERFICIAL_TENSION).setValue(this.InterfacialTension);
            abstractCase.getParameter(NameList.MIXTURE_VELOCITY).setValue(this.MixtureVelocity);
            abstractCase.getParameter(NameList.OIL_DENSITY).setValue(this.OilDensity);
            abstractCase.getParameter(NameList.OIL_VISCOSITY).setValue(this.OilViscosity);
            abstractCase.getParameter(NameList.PIPE_THICKNESS).setValue(this.PipeThickness);
            abstractCase.getParameter(NameList.SUPERFICIAL_GAS_VELOCITY).setValue(this.SuperficialGasVelocity);
            abstractCase.getParameter(NameList.WATER_CUT).setValue(this.WaterCut);
        }
    }
}