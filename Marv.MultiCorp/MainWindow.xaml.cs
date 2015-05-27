using System;
using System.Windows;
using edu.ohiou.icmt;
using edu.ohiou.icmt.modeling.basemodel;
using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.modeling.param;
using edu.ohiou.icmt.multicorp.basemodel;
using edu.ohiou.icmt.multicorp.factory;

namespace Marv.MultiCorp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("MainWindow_Loaded");

            MulticorpRunner.initialize();

            var caseFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.CORROSIONCASE) as CaseFactory;
            var corrosionCase = caseFactory.createModel() as AbstractCase;

            var compositionFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.COMPOSITION) as CompositionFactory;
            var composition = compositionFactory.createModel(corrosionCase) as AbstractModel;

            var flowFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.FLOW) as FlowFactory;
            var flow = flowFactory.createModel(corrosionCase, (int) FlowModel.FlowType.Gas_Water_Flow);

            composition.getParameter(NameList.CO2_GAS_CONTENT).setValue(1);

            composition.doCalculation();

            foreach (var parameter in composition.getAllParameters())
            {
                Console.WriteLine((parameter as AbstractParameter).DisplayName + ":" + (parameter as AbstractParameter).getValue());
            }
        }
    }
}