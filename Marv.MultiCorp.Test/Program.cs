using System;
using edu.ohiou.icmt;
using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.modeling.param;
using edu.ohiou.icmt.multicorp.basemodel;
using edu.ohiou.icmt.multicorp.factory;

namespace Marv.MultiCorp.Test
{
    internal static class Program
    {
        private static void Main()
        {
            MulticorpRunner.initialize();

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
            (abstractCase.getParameter(NameList.FLOW_TYPE) as OptionParameter).setOption((int) FlowModel.FlowType.Gas_Oil_Water_Flow);
            abstractCase.onFlowTypeChanged();

            // The order in which the parameters are set matters. We are going to follow the order in the MultiCorp GUI

            // Flow Velocity
            (abstractCase.getParameter(NameList.VELOCITY_INPUT_TYPE) as OptionParameter).setOption((int) FlowModel.VelTypes.Mixture);

            // Line Parameters
            abstractCase.getParameter(NameList.SECTION_DIAMETER).setValue(0.85);

            Console.WriteLine((abstractCase.getParameter(NameList.SECTION_DIAMETER) as Parameter).getBaseUnitName());

            abstractCase.getParameter(NameList.SUPERFICIAL_GAS_VELOCITY).setValue(10);
            abstractCase.getParameter(NameList.MIXTURE_VELOCITY).setValue(1);
            abstractCase.getParameter(NameList.WATER_CUT).setValue(0.2);

            var flowModel = abstractCase.getModel(NameList.MODEL_NAME_FLOW_MODEL) as FlowModel;

            if (flowModel != null)
            {
                flowModel.doCalculation();
            }

            Console.WriteLine(flowModel.getParameter(NameList.FLOW_PATTERN).getValue());

            Console.ReadKey();
        }
    }
}