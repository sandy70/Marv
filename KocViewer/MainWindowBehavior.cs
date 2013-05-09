using LibBn;
using LibPipeline;
using SharpKml.Engine;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace KocViewer
{
    internal class MainWindowBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Closed += AssociatedObject_Closed;
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.Drop += AssociatedObject_Drop;
        }

        private void AssociatedObject_Closed(object sender, EventArgs e)
        {
            BnGraphWriter.WritePositions(Config.NetworkFile, this.AssociatedObject.GraphControl.SourceGraph);
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            var fileNames = (string[])e.Data.GetData("FileNameW");

            var kmlFile = KmlFile.Load(fileNames[0]);

            var groundOverlay = kmlFile.Root.Flatten().OfType<SharpKml.Dom.GroundOverlay>().FirstOrDefault();
            groundOverlay.Icon.Href = new Uri(Path.Combine(Path.GetDirectoryName(fileNames[0]), groundOverlay.Icon.Href.ToString()));
            this.AssociatedObject.GroundOverlay = groundOverlay;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            window.FileName = Config.NetworkFile;

            // Read the KOC pipeline data
            //window.PipelineTally = MultiLocationReader.ReadExcel(Properties.Settings.Default.TallyFileName, "Sheet1");
            //window.PipelineProfile = MultiLocationReader.ReadExcel(Properties.Settings.Default.ProfileFileName, "Sheet1");

            window.ProfileLocations = ExcelReader.ReadLocations<Location>(Properties.Settings.Default.ProfileFileName);
            window.TallyLocations = ExcelReader.ReadLocations<Location>(Properties.Settings.Default.ProfileFileName);

            KocDataReader kocDataReader = new KocDataReader();
            window.VertexValuesByYear = kocDataReader.ReadVertexValuesForAllYears();
            window.SelectedVertexValues = window.VertexValuesByYear.First().Value;
            kocDataReader.ReadVertexInputsForAllYears(window.InputManager);

            window.SensorListener.NewEvidenceAvailable += SensorListener_NewEvidenceAvailable;
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