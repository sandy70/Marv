using System;
using System.Diagnostics;
using System.Windows;
using edu.ohiou.icmt;
using edu.ohiou.icmt.modeling.basemodel;
using edu.ohiou.icmt.modeling.controller;
using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.modeling.param;
using edu.ohiou.icmt.modeling.tracer;
using edu.ohiou.icmt.multicorp.basemodel;
using edu.ohiou.icmt.multicorp.factory;
using edu.ohiou.icmt.multicorp.postprocessing;

namespace Marv.MultiCorp
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private static void MultiCorpOne()
        {
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

        private static void MultiCorpTwo()
        {
            var tracer = new MConsoleTraceListener();
            Trace.Indent();
            Trace.Listeners.Add(tracer);
            Flags.DEBUG_LEVEL = 1;
            Trace.WriteLine("Console test start", "->");
            MulticorpRunner.initialize();

            //Load existing case
            //Dim caseName As String = "utc1.mcorp"
            //Dim casepath As String = "C:\Users\sarkara1\Documents\MultiCorp\ioFiles\utc\"
            //Dim caseFactory As CaseFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.CORROSIONCASE)
            //Dim cCase As AbstractCase = caseFactory.createModel(caseName, casepath)

            //Create new Case
            var caseFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.CORROSIONCASE) as CaseFactory;
            var cCase = caseFactory.createModel() as AbstractCase;

            //Set Corrosion Type
            (cCase.getParameter(NameList.CORROSION_TYPE) as OptionParameter).setOption((int) AbstractCase.CorrosionPosition.BLC);
            // cCase.onCorrosionTypeChanged()

            //Set Flow type
            (cCase.getParameter(NameList.FLOW_TYPE) as OptionParameter).setOption((int) FlowModel.FlowType.Gas_Water_Flow);
            //cCase.onFlowTypeChanged()

            //Set simulation Type
            (cCase.getParameter(NameList.SIMU_TYPE) as OptionParameter).setOption((int) CorrosionModel.SimulationModelType.Single_run);
            //cCase.onSimulationTypeChanged()

            cCase.getParameter(NameList.TOTAL_PRESSURE).setValue(100);

            //Calculate composition
            var composition = cCase.getModel(NameList.MODEL_NAME_COMPOSITION);

            if (composition != null)
            {
                composition.doCalculation();
            }

            //Calculate flow
            var flow = cCase.getModel(NameList.MODEL_NAME_FLOW_MODEL);

            if (flow != null)
            {
                flow.doCalculation();
            }

            //calcualte condensation
            var condensation = cCase.getModel(NameList.MODEL_NAME_CONDENSATION);
            if (condensation != null)
            {
                condensation.doCalculation();
            }

            //calculate pipeline 
            var pipeline = cCase.getModel(NameList.MODEL_NAME_LINE_MODEL);

            if (pipeline != null)
            {
                pipeline.doCalculation();
            }

            //set up post processing analysis manager
            var mCorrosionResult = CorrosionResultManager.getInstance();

            mCorrosionResult.readDefaultValue(DataController.getDefaultXDoc(), true);
            mCorrosionResult.listenToPrerequisiteModels();
            cCase.setModel(NameList.MODEL_NAME_CORROSION_RESULT, mCorrosionResult);

            //This is how any parameter value can be retrieved
            Trace.WriteLine("Total Pressure = " + cCase.getParameter(NameList.TOTAL_PRESSURE).getDoubleValue());

            //generate and save .MCCASE file (it is an optional feature)
            cCase.generateCaseFile();

            //perform simulation
            var simulation = cCase.getModel(NameList.MODEL_NAME_CORROSION_MODEL);

            if (simulation != null)
            {
                //Preliminary check for all other model's accuracy before proceeding for simulation
                if ((simulation as CorrosionModel).isCorrosionPossible())
                {
                    //if batch model has no parameters chosen then open param selection form (no need of this section of code as we are only executing point model)
                    //if (simulation is BatchModel && !(simulation as CorrosionModel).isReadyForSimulation(true))
                    //{
                    //    'c
                    //    onfigure varying 
                    //    parameter

                    //}

                    //Prepares the stage for simulation, send data to Fortran simulation engine, prepare listeners, reset counters and flags
                    if ((simulation as CorrosionModel).prepareSimulation())
                    {
                        //Add a handler to the following event to retrieve corrosion rate at every time interval
                        (simulation as PointModel).onDataRetrivalInterval += dataRetrieverHandler;
                        (simulation as CorrosionModel).startSimulation();

                        //Waits till simulation is successful, once successful it saves the case in mcorp file 
                        while (true)
                        {
                            if ((simulation as CorrosionModel).wasSimulationSuccess())
                            {
                                //Save the mcorp file for loading hte case again from the file
                                //see the commented section at line 27 for way to load mcorp file
                                //change the file name and path 
                                cCase.fileManager.setFileStructure("TestCase1", @"C:\Users\Vinod\Downloads\MultiCorp\ioFiles\DNV\");
                                cCase.save(false, false);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static void dataRetrieverHandler(double time, double cr)
        {
            Trace.WriteLine("Simulation Time = " + time + " Corrosion Rate = " + cr);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("MainWindow_Loaded");

            // MultiCorpOne();
            MultiCorpTwo();
        }
    }
}