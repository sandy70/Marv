using Marv.Common;
using NLog;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Interactivity;

using System.Linq;

namespace Marv
{
    public class MainWindowBehaviorPolylines : Behavior<MainWindow>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.PolylinesChanged += AssociatedObject_PolylinesChanged;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.Polylines = new ViewModelCollection<LocationCollection>();

            try
            {
                var pipelineInput = new PipelineInput(window.InputFileName);
                var polylines = pipelineInput.ReadPipelines();
                
                window.Polylines.Add(polylines["BU-498"]);

                window.Polylines.Select("BU-498");
                window.Polylines["BU-498"].Select("50");
                
                window.MapView.ZoomTo(window.Polylines.GetBounds());

                window.ReadMultiLocationValueTimeSeriesForMultiLocation();
                window.UpdateMultiLocationValues();
            }
            catch (IOException exp)
            {
                logger.Warn(exp.Message);

                var notification = new NotificationTimed
                {
                    Name = "Unable to read file.",
                    Description = exp.Message,
                };

                window.Notifications.Push<INotification>(notification);
            }
        }

        private void AssociatedObject_PolylinesChanged(object sender, ViewModelCollection<LocationCollection> e)
        {
            var window = this.AssociatedObject;

            if (window.Polylines != null)
            {
                window.Polylines.CollectionChanged += Polylines_CollectionChanged;

                this.PolylinesAttachHandlers(window.Polylines);

                if (window.Polylines.Count > 0)
                {
                    // Calculate start year
                    window.StartYear = window.Polylines.Min(multiLocation => (int)multiLocation.Properties["StartYear"]);
                    window.SelectedYear = window.StartYear;
                }
            }
        }

        private void polyline_SelectionChanged(object sender, Location e)
        {
            var window = this.AssociatedObject;

            window.ReadGraphValues();
            window.UpdateGraphValue();

            if(window.SynergiViewModel.Sections != null)
            {
                window.SynergiViewModel.Sections.SelectedItem = window.SynergiViewModel.Sections.FirstOrDefault(x => x.Name == e.Key);
            }
        }

        private void Polylines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var polylines = sender as ViewModelCollection<LocationCollection>;
            this.PolylinesAttachHandlers(polylines);
        }

        private void PolylinesAttachHandlers(ViewModelCollection<LocationCollection> polylines)
        {
            foreach (var polyline in polylines)
            {
                // Attach event so that we can load data when selection changes
                // The -= ensures that events aren't subscribed twice
                polyline.SelectionChanged -= polyline_SelectionChanged;
                polyline.SelectionChanged += polyline_SelectionChanged;
            }
        }
    }
}