using System;
using System.Diagnostics;
using edu.ohiou.icmt;
using edu.ohiou.icmt.modeling.controller;
using edu.ohiou.icmt.modeling.globalresources;
using edu.ohiou.icmt.modeling.param;
using edu.ohiou.icmt.modeling.tracer;
using edu.ohiou.icmt.multicorp.basemodel;
using edu.ohiou.icmt.multicorp.corrosion.multiplex;
using edu.ohiou.icmt.multicorp.factory;
using edu.ohiou.icmt.multicorp.postprocessing;

namespace Marv.MultiCorp
{
    public class Program
    {
        private static void Main()
        {
            Utils.Initialize();

            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(Utils.ComputeFlowPattern(new GasOilWaterFlowParameters()));
                Console.WriteLine(Utils.ComputeFlowPattern(new GasWaterFlowParameters()));
                Console.WriteLine(Utils.ComputeFlowPattern(new OilWaterFlowParameters()));
            }

            Console.ReadKey();
        }

        private static void TestMultiCorp()
        {
            var tracer = new MConsoleTraceListener();

            //setup tracer to listen tracing information and output to console
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
            cCase.onCorrosionTypeChanged();

            //Set Flow type
            (cCase.getParameter(NameList.FLOW_TYPE) as OptionParameter).setOption((int) FlowModel.FlowType.Gas_Water_Flow);
            cCase.onFlowTypeChanged();

            //Set simulation Type
            (cCase.getParameter(NameList.SIMU_TYPE) as OptionParameter).setOption((int) CorrosionModel.SimulationModelType.Single_run);
            cCase.onSimulationTypeChanged();

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
            var mCorrosionResult = CorrosionResultManager.getInstance() as SinglePointModelResult;
            mCorrosionResult.readDefaultValue(DataController.getDefaultXDoc(), true);
            mCorrosionResult.listenToPrerequisiteModels();
            cCase.setModel(NameList.MODEL_NAME_CORROSION_RESULT, mCorrosionResult);

            //This is how any parameter value can be retrieved
            Trace.WriteLine("Total Pressure = " + cCase.getParameter(NameList.TOTAL_PRESSURE).getDoubleValue());

            //generate and save .MCCASE file (it is an optional feature)
            //cCase.generateCaseFile();

            //perform simulation
            var simulation = cCase.getModel(NameList.MODEL_NAME_CORROSION_MODEL) as CorrosionModel;

            if (simulation != null)
            {
                //Preliminary check for all other model's accuracy before proceeding for simulation
                if (simulation.isCorrosionPossible())
                {
                    simulation.makeOutputOptionList();

                    //if batch model has no parameters chosen then open param selection form (no need of this section of code as we are only executing point model)
                    if (simulation is BatchModel && !simulation.isReadyForSimulation())
                    {
                        // configure varying parameter
                    }

                    //Prepares the stage for simulation, send data to Fortran simulation engine, prepare listeners, reset counters and flags
                    if (simulation.prepareSimulation())
                    {
                        simulation.startSimulation();

                        while (true)
                        {
                            if (simulation.wasSimulationSuccess())
                            {
                                mCorrosionResult.doLoadData(cCase.fileManager.getLatestOutputFile());

                                for (var i = 0; i < mCorrosionResult.getNumOfLoops(); i++)
                                {
                                    var result = mCorrosionResult.getLoopResult(i);
                                    Trace.WriteLine("Simulation Time = " + result.Simulation_Time + " Corrosion Rate = " + result.Corrosion_Rate);
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}