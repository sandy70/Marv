using System.IO;
using System.Windows;
using System.Windows.Interactivity;
using Marv.Map;
using NLog;

namespace Marv
{
    public class MainWindowBehaviorPolylines : Behavior<MainWindow>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.PolylinesChanged += AssociatedObject_PolylinesChanged;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.Polylines = new KeyedCollection<LocationCollection>();

            try
            {
                var pipelineInput = new PipelineInput(window.InputFileName);
                var polylines = pipelineInput.ReadPipelines();

                window.Polylines.Add(polylines["BU-498"]);

                //window.Polylines.Select("BU-498");
                //window.Polylines["BU-498"].Select("50");

                window.MapView.ZoomTo(window.Polylines.GetBounds());

                window.ReadMultiLocationValueTimeSeriesForMultiLocation();
                window.UpdateMultiLocationValues();
            }
            catch (IOException exp)
            {
                Logger.Warn(exp.Message);

                var notification = new NotificationTimed
                {
                    Name = "Unable to read file.",
                    Description = exp.Message,
                };

                window.Notifications.Push(notification);
            }
        }

        private void AssociatedObject_PolylinesChanged(object sender, KeyedCollection<LocationCollection> e)
        {
            var window = this.AssociatedObject;

            if (window.Polylines == null || window.Polylines.Count <= 0)
            {
                return;
            }

            // Calculate start year
            //window.StartYear = window.Polylines.Min(multiLocation => (int)multiLocation.Properties["StartYear"]);
            window.SelectedYear = window.StartYear;
        }
    }
}