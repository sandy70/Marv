using edu.ohiou.icmt;
using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.modeling.param;
using edu.ohiou.icmt.multicorp.basemodel;
using edu.ohiou.icmt.multicorp.factory;

namespace Marv.MultiCorp
{
    public static class Utils
    {
        public static string ComputeFlowPattern(GasOilWaterFlowParameters parameters)
        {
            //Create new Case
            var caseFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.CORROSIONCASE) as CaseFactory;
            var cCase = caseFactory.createModel() as AbstractCase;

            //Set Corrosion Type
            (cCase.getParameter(NameList.CORROSION_TYPE) as OptionParameter).setOption((int) AbstractCase.CorrosionPosition.BLC);
            cCase.onCorrosionTypeChanged();

            //Set Flow type
            (cCase.getParameter(NameList.FLOW_TYPE) as OptionParameter).setOption((int) FlowModel.FlowType.Gas_Oil_Water_Flow);
            cCase.onFlowTypeChanged();

            //Set simulation Type
            (cCase.getParameter(NameList.SIMU_TYPE) as OptionParameter).setOption((int) CorrosionModel.SimulationModelType.Line_run);
            cCase.onSimulationTypeChanged();

            cCase.getParameter(NameList.GAS_DENSITY).setValue(parameters.GasDensity);
            cCase.getParameter(NameList.SECTION_DIAMETER).setValue(parameters.InternalDiameter);
            cCase.getParameter(NameList.SECTION_INCLINATION).setValue(parameters.Inclination);
            cCase.getParameter(NameList.MIXTURE_VELOCITY).setValue(parameters.MixtureVelocity);
            cCase.getParameter(NameList.OIL_DENSITY).setValue(parameters.OilDensity);
            cCase.getParameter(NameList.OIL_VISCOSITY).setValue(parameters.OilViscosity);
            cCase.getParameter(NameList.SUPERFICIAL_GAS_VELOCITY).setValue(parameters.SuperficialGasVelocity);
            cCase.getParameter(NameList.WATER_CUT).setValue(parameters.WaterCut);

            var flow = cCase.getModel(NameList.MODEL_NAME_FLOW_MODEL);

            if (flow != null)
            {
                flow.doCalculation();
            }

            return flow.getParameter(NameList.FLOW_PATTERN).getValue().ToString();
        }

        public static string ComputeFlowPattern(GasWaterFlowParameters parameters)
        {
            //Create new Case
            var caseFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.CORROSIONCASE) as CaseFactory;
            var cCase = caseFactory.createModel() as AbstractCase;

            //Set Corrosion Type
            (cCase.getParameter(NameList.CORROSION_TYPE) as OptionParameter).setOption((int) AbstractCase.CorrosionPosition.BLC);
            cCase.onCorrosionTypeChanged();

            //Set Flow type
            (cCase.getParameter(NameList.FLOW_TYPE) as OptionParameter).setOption((int) FlowModel.FlowType.Gas_Water_Flow);
            cCase.onFlowTypeChanged();

            //Set simulation Type
            (cCase.getParameter(NameList.SIMU_TYPE) as OptionParameter).setOption((int) CorrosionModel.SimulationModelType.Line_run);
            cCase.onSimulationTypeChanged();

            cCase.getParameter(NameList.SECTION_DIAMETER).setValue(parameters.InternalDiameter);
            cCase.getParameter(NameList.SECTION_INCLINATION).setValue(parameters.Inclination);
            cCase.getParameter(NameList.GAS_LIQUID_SURFACE_TENSION).setValue(parameters.GasLiquidSurfaceTension);
            cCase.getParameter(NameList.SUPERFICIAL_GAS_VELOCITY).setValue(parameters.SuperficialGasVelocity);
            cCase.getParameter(NameList.GAS_DENSITY).setValue(parameters.GasDensity);
            cCase.getParameter(NameList.WATER_DENSITY).setValue(parameters.WaterDensity);
            cCase.getParameter(NameList.WATER_VISCOSITY).setValue(parameters.WaterViscosity);
            cCase.getParameter(NameList.SUPERFICIAL_WATER_VELOCITY).setValue(parameters.SuperficialWaterVelocity);

            var flow = cCase.getModel(NameList.MODEL_NAME_FLOW_MODEL);

            if (flow != null)
            {
                flow.doCalculation();
            }

            return flow.getParameter(NameList.FLOW_PATTERN).getValue().ToString();
        }

        public static string ComputeFlowPattern(OilWaterFlowParameters parameters)
        {
            //Create new Case
            var caseFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.CORROSIONCASE) as CaseFactory;
            var cCase = caseFactory.createModel() as AbstractCase;

            //Set Corrosion Type
            (cCase.getParameter(NameList.CORROSION_TYPE) as OptionParameter).setOption((int) AbstractCase.CorrosionPosition.BLC);
            cCase.onCorrosionTypeChanged();

            //Set Flow type
            (cCase.getParameter(NameList.FLOW_TYPE) as OptionParameter).setOption((int) FlowModel.FlowType.Oil_Water_Flow);
            cCase.onFlowTypeChanged();

            //Set simulation Type
            (cCase.getParameter(NameList.SIMU_TYPE) as OptionParameter).setOption((int) CorrosionModel.SimulationModelType.Single_run);
            cCase.onSimulationTypeChanged();

            cCase.getParameter(NameList.INPUT_TYPE).setValue(FlowModel.InputTypes.Velocity);
            cCase.getParameter(NameList.VELOCITY_INPUT_TYPE).setValue(FlowModel.VelTypes.Mixture);

            cCase.getParameter(NameList.SECTION_DIAMETER).setValue(parameters.InternalDiameter);
            cCase.getParameter(NameList.SECTION_INCLINATION).setValue(parameters.Inclination);
            cCase.getParameter(NameList.INTERFICIAL_TENSION).setValue(parameters.InterfacialTension);
            cCase.getParameter(NameList.MIXTURE_VELOCITY).setValue(parameters.MixtureVelocity);
            cCase.getParameter(NameList.OIL_DENSITY).setValue(parameters.OilDensity);
            cCase.getParameter(NameList.OIL_VISCOSITY).setValue(parameters.OilViscosity);
            cCase.getParameter(NameList.WATER_CUT).setValue(parameters.WaterCut);

            var flow = cCase.getModel(NameList.MODEL_NAME_FLOW_MODEL);

            if (flow != null)
            {
                flow.doCalculation();
            }

            return flow.getParameter(NameList.FLOW_PATTERN).getValue().ToString();
        }

        public static void Initialize()
        {
            MulticorpRunner.initialize();
        }
    }
}