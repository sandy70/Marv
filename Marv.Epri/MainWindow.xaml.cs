using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CsvHelper;
using Marv.Common;
using Marv.Common.Types;
using Marv.Epri.Properties;
using Microsoft.Win32;
using Newtonsoft.Json;
using Telerik.Charting;
using Telerik.Windows.Controls;

namespace Marv.Epri
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private const string StreamId = "00000000-00000000-00409DFF-FF88AC6C";

        private readonly HttpClient httpClient = new HttpClient(new HttpClientHandler { Credentials = new NetworkCredential(Settings.Default.Login, Settings.Default.Password) });

        private readonly DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(60),
        };

        private ObservableCollection<DataPoint> dataPoints = new ObservableCollection<DataPoint>();
        private LineStringCollection locationCollections = new LineStringCollection();
        private NotificationCollection notifications = new NotificationCollection();
        private string selectedStream;
        private TimeSpan selectedTimeSpan;
        private ObservableCollection<string> streams;
        private Task<ObservableCollection<DataPoint>> task;

        private Dict<string, TimeSpan> timespans = new Dict<string, TimeSpan>
        {
            { "1 Hour", TimeSpan.FromHours(1) },
            { "6 Hours", TimeSpan.FromHours(6) },
            { "1 Day", TimeSpan.FromDays(1) },
            { "2 Day", TimeSpan.FromDays(2) }
        };

        public ObservableCollection<DataPoint> DataPoints
        {
            get { return this.dataPoints; }

            set
            {
                this.dataPoints = value;
                this.RaisePropertyChanged();
            }
        }

        public LineStringCollection LocationCollections
        {
            get { return this.locationCollections; }

            set
            {
                this.locationCollections = value;
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

        public string SelectedStream
        {
            get { return this.selectedStream; }

            set
            {
                if (value.Equals(this.selectedStream))
                {
                    return;
                }

                this.selectedStream = value;
                this.RaisePropertyChanged();
            }
        }

        public TimeSpan SelectedTimeSpan
        {
            get { return this.selectedTimeSpan; }

            set
            {
                if (value.Equals(this.selectedTimeSpan))
                {
                    return;
                }

                this.selectedTimeSpan = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> Streams
        {
            get { return this.streams; }

            set
            {
                if (value.Equals(this.streams))
                {
                    return;
                }

                this.streams = value;
                this.RaisePropertyChanged();
            }
        }

        public Dict<string, TimeSpan> TimeSpans
        {
            get { return this.timespans; }

            set
            {
                if (value.Equals(this.timespans))
                {
                    return;
                }

                this.timespans = value;
                this.RaisePropertyChanged();
            }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();

            this.timer.Tick += timer_Tick;

            this.timer.Start();
        }

        private void DateTimeContinuousAxis_OnActualVisibleRangeChanged(object sender, DateTimeRangeChangedEventArgs e)
        {
            Console.WriteLine("DateTimeContinuousAxis_OnActualVisibleRangeChanged");
        }

        private async Task<ObservableCollection<DataPoint>> DownloadDataPointsAsync()
        {
            const string server = @"http://devicecloud.digi.com";

            var uriEndPoint = server + "/ws/v1/streams/history/" + StreamId + "/" + this.SelectedStream + ".json";

            var uri = uriEndPoint + "?" + Utils.FormRestArgs(new
            {
                start_time = (DateTime.UtcNow - this.SelectedTimeSpan).ToString("o"),
                timeline = "server"
            });

            var downloadedPoints = new ObservableCollection<DataPoint>();

            while (true)
            {
                try
                {
                    var uri1 = uri;
                    var response = await Task.Run(async () => JsonConvert.DeserializeObject<Response<DataPoint>>(await this.httpClient.GetStringAsync(uri1)));

                    downloadedPoints.Add(response.list);

                    if (response.next_uri == null)
                    {
                        break;
                    }

                    uri = server + response.next_uri;
                }
                catch (HttpRequestException exception)
                {
                    Thread.Sleep(100);
                    Console.WriteLine(exception.Message);
                }
            }

            return downloadedPoints;
        }

        private async Task<ObservableCollection<string>> DownloadStreamsAsync()
        {
            if (httpClient != null)
            {
                this.httpClient.CancelPendingRequests();
            }

            const string server = @"http://devicecloud.digi.com";

            var uriEndPoint = server + "/ws/v1/streams/inventory.json?category=data";

            var downloadedStreams = new ObservableCollection<string>();

            while (true)
            {
                var point = uriEndPoint;
                var response = await Task.Run(async () => JsonConvert.DeserializeObject<Response<Stream>>(await httpClient.GetStringAsync(point)));

                downloadedStreams.Add(response.list.Select(x => x.id.Split("/".ToArray())[1]));

                if (response.next_uri == null)
                {
                    break;
                }

                uriEndPoint = server + response.next_uri;
            }

            return downloadedStreams;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var notification = new Notification
            {
                IsIndeterminate = true,
                Description = "Loading streams..."
            };

            this.Notifications.Add(notification);

            this.Streams = await this.DownloadStreamsAsync();

            this.Notifications.Remove(notification);

            await this.UpdateDataPoints();

            this.LocationCollections = LocationCollection.ReadKml(@"C:\Users\vkha\Data\EPRI\EpriPipes.kml");

            this.GraphControl.Open(@"C:\Users\vkha\Data\Misc\XivelyLight.net");
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null && propertyName.Length > 0)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Comma Sepated Values|*.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                var csvWriter = new CsvWriter(new StreamWriter(dialog.FileName));
                csvWriter.WriteRecords(this.DataPoints);
            }
        }

        private async void SteamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await this.UpdateDataPoints();
        }

        private async void TimeSpanComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await this.UpdateDataPoints();
        }

        private async Task UpdateDataPoints()
        {
            if (this.SelectedStream == null)
            {
                return;
            }

            var notification = new Notification
            {
                IsIndeterminate = true,
                Description = "Loading data..."
            };

            this.Notifications.Add(notification);

            if (this.task == null || this.task.IsCompleted)
            {
                this.DataPoints = await (this.task = this.DownloadDataPointsAsync());

                var vertexEvidences = new Dict<string, VertexEvidence>
                {
                    { "Value", this.GraphControl.Graph.Vertices["Value"].States.ParseEvidenceString(this.DataPoints.Last().Value.ToString()) }
                };

                this.GraphControl.Graph.Belief = this.GraphControl.Network.Run(vertexEvidences);
                this.GraphControl.Graph.SetEvidence(vertexEvidences);
            }

            this.Notifications.Remove(notification);
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            await this.UpdateDataPoints();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}