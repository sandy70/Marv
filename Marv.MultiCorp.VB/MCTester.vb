Imports edu.ohiou.icmt.multicorp.factory
Imports edu.ohiou.icmt.multicorp.basemodel
Imports edu.ohiou.icmt.modeling.basemodel
Imports edu.ohiou.icmt.modeling.globalresources
Imports edu.ohiou.icmt.multicorp
Imports edu.ohiou.icmt.modeling.tracer
Imports edu.ohiou.icmt
Imports edu.ohiou.icmt.modeling.param
Imports edu.ohiou.icmt.multicorp.postprocessing
Imports edu.ohiou.icmt.modeling.controller

Namespace utc

    Public Class MCTester

        Shared Sub main()

            Dim tracer As New MConsoleTraceListener()
            ' setup tracer to listen tracing information and output to console
            tracer = New MConsoleTraceListener()
            Trace.Indent()
            Trace.Listeners.Add(tracer)
            Flags.DEBUG_LEVEL = 1
            Trace.WriteLine("Console test start", "->")
            MulticorpRunner.initialize()

            'Load existing case
            'Dim caseName As String = "utc1.mcorp"
            'Dim casepath As String = "C:\Users\sarkara1\Documents\MultiCorp\ioFiles\utc\"
            'Dim caseFactory As CaseFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.CORROSIONCASE)
            'Dim cCase As AbstractCase = caseFactory.createModel(caseName, casepath)

            'Create new Case
            Dim caseFactory As CaseFactory = AbstractModelFactory.getFactory(AbstractModelFactory.AbstractModels.CORROSIONCASE)
            Dim cCase As AbstractCase = caseFactory.createModel()

            'Set Corrosion Type
            CType(cCase.getParameter(NameList.CORROSION_TYPE), OptionParameter).setOption(AbstractCase.CorrosionPosition.BLC)
            cCase.onCorrosionTypeChanged()
            'Set Flow type
            CType(cCase.getParameter(NameList.FLOW_TYPE), OptionParameter).setOption(FlowModel.FlowType.Gas_Water_Flow)
            cCase.onFlowTypeChanged()
            'Set simulation Type
            CType(cCase.getParameter(NameList.SIMU_TYPE), OptionParameter).setOption(CorrosionModel.SimulationModelType.Single_run)
            cCase.onSimulationTypeChanged()

            cCase.getParameter(NameList.TOTAL_PRESSURE).setValue(100)

            'Calculate composition
            Dim composition As AbstractModel = cCase.getModel(NameList.MODEL_NAME_COMPOSITION)
            If Not composition Is Nothing Then
                composition.doCalculation()
            End If

            'Calculate flow
            Dim flow As AbstractModel = cCase.getModel(NameList.MODEL_NAME_FLOW_MODEL)
            If Not flow Is Nothing Then
                flow.doCalculation()
            End If

            'calcualte condensation
            Dim condensation As AbstractModel = cCase.getModel(NameList.MODEL_NAME_CONDENSATION)
            If Not condensation Is Nothing Then
                condensation.doCalculation()
            End If

            'calculate pipeline 
            Dim pipeline As AbstractModel = cCase.getModel(NameList.MODEL_NAME_LINE_MODEL)
            If Not pipeline Is Nothing Then
                pipeline.doCalculation()
            End If

            'set up post processing analysis manager
            Dim mCorrosionResult As CorrosionResult = CorrosionResultManager.getInstance()
            mCorrosionResult.readDefaultValue(DataController.getDefaultXDoc, True)
            mCorrosionResult.listenToPrerequisiteModels()
            cCase.setModel(NameList.MODEL_NAME_CORROSION_RESULT, mCorrosionResult)

            'This is how any parameter value can be retrieved
            Trace.WriteLine("Total Pressure = " + cCase.getParameter(NameList.TOTAL_PRESSURE).getDoubleValue.ToString)

            'generate and save .MCCASE file (it is an optional feature)
            cCase.generateCaseFile()

            'perform simulation
            Dim simulation As AbstractModel = cCase.getModel(NameList.MODEL_NAME_CORROSION_MODEL)
            If Not simulation Is Nothing Then

                'Preliminary check for all other model's accuracy before proceeding for simulation
                If CType(simulation, CorrosionModel).isCorrosionPossible() Then

                    'if batch model has no parameters chosen then open param selection form (no need of this section of code as we are only executing point model)
                    If TypeOf (simulation) Is corrosion.multiplex.BatchModel And
                        Not CType(simulation, CorrosionModel).isReadyForSimulation(True) Then
                        'configure varying parameter 


                    End If

                    'Prepares the stage for simulation, send data to Fortran simulation engine, prepare listeners, reset counters and flags
                    If CType(simulation, CorrosionModel).prepareSimulation() Then

                        'Add a handler to the following event to retrieve corrosion rate at every time interval
                        AddHandler CType(simulation, PointModel).onDataRetrivalInterval, AddressOf dataRetrieverHandler
                        CType(simulation, CorrosionModel).startSimulation()

                        'Waits till simulation is successful, once successful it saves the case in mcorp file 
                        While (1)
                            If CType(simulation, CorrosionModel).wasSimulationSuccess Then

                                'Save the mcorp file for loading hte case again from the file
                                'see the commented section at line 27 for way to load mcorp file
                                'change the file name and path 
                                cCase.fileManager.setFileStructure("TestCase1", "C:\Users\sarkara1\Documents\MultiCorp\ioFiles\DNV\")
                                cCase.save(False, False)
                                Exit While

                            End If

                        End While

                    End If

                End If

            End If

        End Sub

        Private Shared Sub dataRetrieverHandler(time As Double, cr As Double)
            Trace.WriteLine("Simulation Time = " + time.ToString + " Corrosion Rate = " + cr.ToString)
        End Sub

    End Class


End Namespace