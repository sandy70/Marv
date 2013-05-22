using LibBn;
using LibPipeline;
using LibPipline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Marv
{
    internal class MainWindowBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Closing += AssociatedObject_Closing;
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        private void AssociatedObject_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.AssociatedObject.GraphControl.SourceGraph.Write(this.AssociatedObject.FileName);
            Properties.Settings.Default.Save();
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.S) &&
                (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) &&
                (e.KeyboardDevice.IsKeyDown(Key.LeftAlt) || e.KeyboardDevice.IsKeyDown(Key.RightAlt)))
            {
                this.AssociatedObject.IsSettingsVisible = !this.AssociatedObject.IsSettingsVisible;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.R) &&
                   (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
                Console.WriteLine(DateTime.Now);
                this.RunModel();
                Console.WriteLine(DateTime.Now);
            }
        }

        private async void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.PopupControl.ShowTextIndeterminate("Reading Profile");
            window.ProfileLocations = await ExcelReader.ReadPropertyLocationsAsync<PropertyLocation>(Properties.Settings.Default.ProfileFileName);
            window.PopupControl.Hide();

            window.PopupControl.ShowTextIndeterminate("Reading tally");
            window.TallyLocations = await ExcelReader.ReadPropertyLocationsAsync<PropertyLocation>(Properties.Settings.Default.TallyFileName);
            window.PopupControl.Hide();

            window.MapView.ZoomToExtent(window.ProfileLocations.Bounds());

            KocDataReader kocDataReader = new KocDataReader();
            window.VertexValuesByYear = kocDataReader.ReadVertexValuesForAllYears();
            window.SelectedVertexValues = window.VertexValuesByYear.First().Value;
            kocDataReader.ReadVertexInputsForAllYears(window.InputManager);

            window.SensorListener.NewEvidenceAvailable += SensorListener_NewEvidenceAvailable;
        }

        private void RunModel()
        {
            var window = this.AssociatedObject;

            // This will need to be initialized or read from file later.
            var inputStore = new InputStore();
            var valueStore = new ValueStore();

            int startYear = 1990;
            int endYear = 2020;

            for (int year = startYear; year <= endYear; year++)
            {
                var graphInput = inputStore.GetGraphInput(year);
                var graphEvidence = new Dictionary<string, VertexEvidence>();

                var fixedVariables = new Dictionary<string, int>
                {
                    { "dia", 6 },
                    { "t", 5 },
                    { "coattype", 2 },
                    { "surfaceprep", 4 },
                    { "C8", 2 },
                    { "Kd", 0 },
                    { "Cs", 5 },
                    { "Rs", 4 },
                    { "pratio", 3 },
                    { "freq", 3 },
                    { "Kd_w", 10 },
                    { "Kd_b", 10 },
                    { "CP", 5 },
                    { "rho", 4 },
                    { "Co2", 3 },
                    { "millscale", 1 },
                    { "wd", 2 },
                    { "T", 5 },
                    { "P", 5 }
                };

                foreach (var variable in fixedVariables)
                {
                    graphEvidence[variable.Key] = new VertexEvidence
                    {
                        EvidenceType = EvidenceType.StateSelected,
                        StateIndex = variable.Value
                    };
                }

                var stateValues = new List<int> { 1000, 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50, 45, 40, 35, 30, 25, 20, 15, 10, 5, 0 };

                var location = this.AssociatedObject.SelectedProfileLocation as dynamic;
                var mean = location.Pressure;
                var variance = Math.Pow(location.Pressure - location.Pressure_Min, 2);
                var normalDistribution = new NormalDistribution(mean, variance);

                graphEvidence["P"] = new VertexEvidence 
                {
                    Evidence = new double[stateValues.Count - 1],
                    EvidenceType = EvidenceType.SoftEvidence
                };

                for(int i = 0; i < stateValues.Count - 1;i++)
                {
                    graphEvidence["P"].Evidence[i] = normalDistribution.CDF(stateValues[i]) - normalDistribution.CDF(stateValues[i + 1]); 
                }

                this.AssociatedObject.GraphControl.SourceGraph.SetEvidence(graphEvidence);
                this.AssociatedObject.GraphControl.SourceGraph.UpdateBeliefs();
                this.AssociatedObject.GraphControl.SourceGraph.UpdateValue();
            }

            Console.WriteLine("Model run.");
        }

        private void SensorListener_NewEvidenceAvailable(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var window = this.AssociatedObject;
            window.AddInput(e.Value);

            var success = window.TryUpdateNetwork();

            if (!success)
            {
                window.PopupControl.ShowText("Inconsistent evidence entered.");
                window.InputManager.RemoveVertexInput(BnInputType.User, window.SelectedYear, e.Value.Key);
                window.TryUpdateNetwork();
            }
        }
    }
}