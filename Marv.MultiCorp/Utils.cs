using System;
using System.Linq;
using edu.ohiou.icmt;
using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.modeling.param;
using edu.ohiou.icmt.multicorp.basemodel;
using edu.ohiou.icmt.multicorp.factory;

namespace Marv.MultiCorp
{
    public static class Utils
    {
        public static FlowResults ComputeFlow(FlowParameters flowParameters)
        {
            //Create new Case
            var caseFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.CORROSIONCASE) as CaseFactory;
            var abstractCase = caseFactory.createModel() as AbstractCase;

            //Set Corrosion Type
            (abstractCase.getParameter(NameList.CORROSION_TYPE) as OptionParameter).setOption((int) AbstractCase.CorrosionPosition.BLC);
            abstractCase.onCorrosionTypeChanged();

            //Set simulation Type
            (abstractCase.getParameter(NameList.SIMU_TYPE) as OptionParameter).setOption((int) CorrosionModel.SimulationModelType.Single_run);
            abstractCase.onSimulationTypeChanged();

            //Set Flow type
            (abstractCase.getParameter(NameList.FLOW_TYPE) as OptionParameter).setOption((int) flowParameters.FlowType);
            abstractCase.onFlowTypeChanged();

            // Line Parameters
            abstractCase.getParameter(NameList.SECTION_DIAMETER).setValue(flowParameters.InternalDiameter);
            abstractCase.getParameter(NameList.SECTION_INCLINATION).setValue(flowParameters.Inclination);
            abstractCase.getParameter(NameList.PIPE_ROUGHNESS).setValue(flowParameters.PipeRoughness);
            abstractCase.getParameter(NameList.PIPE_THICKNESS).setValue(flowParameters.PipeThickness);
            abstractCase.getParameter(NameList.PIPE_CONDUCTIVITY).setValue(flowParameters.PipeConductivity);

            // Flow Velocity
            abstractCase.getParameter(NameList.VELOCITY_INPUT_TYPE).setValue((int) flowParameters.VelocityInputType);

            if (flowParameters.VelocityInputType == FlowModel.VelTypes.Mixture)
            {
                abstractCase.getParameter(NameList.SUPERFICIAL_GAS_VELOCITY).setValue(flowParameters.SuperficialGasVelocity);
                abstractCase.getParameter(NameList.MIXTURE_VELOCITY).setValue(flowParameters.MixtureVelocity);
                abstractCase.getParameter(NameList.WATER_CUT).setValue(flowParameters.WaterCut);
            }
            else
            {
                abstractCase.getParameter(NameList.SUPERFICIAL_WATER_VELOCITY).setValue(flowParameters.SuperficialWaterVelocity);
                abstractCase.getParameter(NameList.SUPERFICIAL_GAS_VELOCITY).setValue(flowParameters.SuperficialGasVelocity);
            }

            // Water Properties at Operating Conditions
            abstractCase.getParameter(NameList.WATER_DENSITY).setValue(flowParameters.WaterDensity);
            abstractCase.getParameter(NameList.WATER_VISCOSITY).setValue(flowParameters.WaterViscosity);

            // Oil Properties
            abstractCase.getParameter(NameList.OIL_DENSITY).setValue(flowParameters.OilDensity);
            abstractCase.getParameter(NameList.OIL_VISCOSITY).setValue(flowParameters.OilViscosity);
            abstractCase.getParameter(NameList.INTERFICIAL_TENSION).setValue(flowParameters.InterfacialTension);

            // Gas Properties at Operating Conditions
            abstractCase.getParameter(NameList.GAS_PROPERTIES_INPUT).setValue((int) flowParameters.GasPropertiesInputType);

            if (flowParameters.GasPropertiesInputType == FlowModel.GasPropertiesInput.Input)
            {
                abstractCase.getParameter(NameList.GAS_DENSITY).setValue(flowParameters.GasDensity);
                abstractCase.getParameter(NameList.GAS_VISCOSITY).setValue(flowParameters.GasViscosity);
            }

            abstractCase.getParameter(NameList.GAS_LIQUID_SURFACE_TENSION).setValue(flowParameters.GasLiquidSurfaceTension);

            var flowModel = abstractCase.getModel(NameList.MODEL_NAME_FLOW_MODEL) as FlowModel;

            if (flowModel != null)
            {
                flowModel.doCalculation();
            }

            return new FlowResults
            {
                Pattern = flowModel.getParameter(NameList.FLOW_PATTERN).getValue().ToString(),
                Wetting = flowModel.getParameter(NameList.WETTING_PHASE).getValue().ToString()
            };
        }

        public static void Initialize()
        {
            MulticorpRunner.initialize();
        }
    }
}