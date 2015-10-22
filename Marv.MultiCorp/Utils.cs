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

            //Set simulation Type
            (cCase.getParameter(NameList.SIMU_TYPE) as OptionParameter).setOption((int) CorrosionModel.SimulationModelType.Single_run);
            cCase.onSimulationTypeChanged();

            flowParameters.Set(cCase);

            var flowModel = cCase.getModel(NameList.MODEL_NAME_FLOW_MODEL) as FlowModel;

            if (flowModel != null)
            {
                flowModel.doCalculation();
            }

            var flowResults = new FlowResults
            {
                Pattern = flowModel.getParameter(NameList.FLOW_PATTERN).getValue().ToString(),
                Wetting = flowModel.getParameter(NameList.WETTING_PHASE).getValue().ToString()
            };

            flowModel.resetModel();
            cCase.resetModel();

            return flowResults;
        }

        public static void Initialize()
        {
            MulticorpRunner.initialize();
        }
    }
}