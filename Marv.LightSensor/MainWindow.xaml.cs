using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Marv.Common;
using Marv.Common.Types;
using Marv.LightSensor.Properties;
using Newtonsoft.Json;
using Telerik.Windows.Controls;

namespace Marv.LightSensor
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private Graph graph;
        private NotificationCollection notifications = new NotificationCollection();
        private double value;

        public Graph Graph
        {
            get { return this.graph; }

            set
            {
                if (value.Equals(this.graph))
                {
                    return;
                }

                this.graph = value;
                this.RaisePropertyChanged();
            }
        }

        public NotificationCollection Notifications
        {
            get { return this.notifications; }

            set
            {
                if (value.Equals(this.notifications))
                {
                    return;
                }

                this.notifications = value;
                this.RaisePropertyChanged();
            }
        }

        public double Value
        {
            get { return this.value; }

            set
            {
                if (value.Equals(this.value))
                {
                    return;
                }

                this.value = value;
                this.RaisePropertyChanged();
            }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15)
            };

            timer.Tick += timer_Tick;

            timer.Start();
        }

        private async Task<XivelyFeed> GetXivelyFeedAsync()
        {
            const string url = @"https://api.xively.com/v2/feeds/1758963438.json?datastreams=Sensor03&X-ApiKey=iH1MTJKiDhHyAjqWgQ54j3ljdXSgYodF4UoITAGL4A6QhHhs";

            using (var webClient = new HttpClient(new HttpClientHandler { Credentials = new NetworkCredential(Settings.Default.UserName, Settings.Default.Password) }))
            {
                return JsonConvert.DeserializeObject<XivelyFeed>(await webClient.GetStringAsync(url));
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.GraphControl.Open(Settings.Default.NetworkFileName);
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null && propertyName.Length > 0)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            var notification = new Notification
            {
                IsIndeterminate = true,
                Description = "Loading data..."
            };

            this.Notifications.Add(notification);

            this.Value = (await this.GetXivelyFeedAsync()).DataStreams[0].current_value;

            this.Notifications.Remove(notification);

            if (this.Graph != null)
            {
                var vertexEvidences = new Dict<string, VertexEvidence>
                {
                    { "Value", this.Graph.Vertices["Value"].States.ParseEvidenceString(this.Value.ToString()) }
                };

                this.Graph.Belief = this.Graph.Network.Run(vertexEvidences);
                this.Graph.SetEvidence(vertexEvidences);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}