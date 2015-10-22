using edu.ohiou.icmt;
using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.modeling.param;
using edu.ohiou.icmt.multicorp.basemodel;
using edu.ohiou.icmt.multicorp.factory;

namespace Marv.MultiCorp
{
    public static class Utils
    {
        public static FlowResults ComputeFlow(IFlowParameters flowParameters)
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

            flowParameters.Set(cCase);

            return cCase.GetFlowResults();
        }

        public static void Initialize()
        {
            MulticorpRunner.initialize();
        }

        private static FlowResults GetFlowResults(this AbstractCase cCase)
        {
            var flowModel = cCase.getModel(NameList.MODEL_NAME_FLOW_MODEL) as FlowModel;

            if (flowModel != null)
            {
                flowModel.doCalculation();
            }

            return new FlowResults
            {
                Pattern = flowModel.getParameter(NameList.FLOW_PATTERN).getValue().ToString(),
                Wetting = flowModel.getParameter(NameList.WETTING_FACTOR).getValue().ToString()
            };
        }
    }
}