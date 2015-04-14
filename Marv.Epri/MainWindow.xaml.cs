using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Marv.Common;
using Marv.Epri.Properties;
using Newtonsoft.Json;

namespace Marv.Epri
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly HttpClient httpClient = new HttpClient(new HttpClientHandler { Credentials = new NetworkCredential(Settings.Default.Login, Settings.Default.Password) });

        private readonly DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(60),
        };

        private ObservableCollection<DataPoint> dataPoints = new ObservableCollection<DataPoint>();
        private string device;
        private bool isDownloading;
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
        };

        public ObservableCollection<DataPoint> DataPoints
        {
            get { return this.dataPoints; }

            set
            {
                if (value.Equals(this.dataPoints))
                {
                    return;
                }

                this.dataPoints = value;
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
            InitializeComponent();

            this.timer.Tick += timer_Tick;

            this.timer.Start();
        }

        private async Task<ObservableCollection<DataPoint>> DownloadDataPointsAsync()
        {
            const string server = @"http://devicecloud.digi.com";

            var uriEndPoint = server + "/ws/v1/streams/history/00000000-00000000-00409DFF-FF88AC6C/" + this.SelectedStream + ".json";

            var uri = uriEndPoint + "?" + Utils.FormRestArgs(new
            {
                start_time = (DateTime.UtcNow - this.SelectedTimeSpan).ToString("o"),
                timeline = "server"
            });

            var downloadedPoints = new ObservableCollection<DataPoint>();

            while (true)
            {
                var response = await Task.Run(async () => JsonConvert.DeserializeObject<Response<DataPoint>>(await this.httpClient.GetStringAsync(uri)));

                downloadedPoints.Add(response.list);

                if (response.next_uri == null)
                {
                    break;
                }

                uri = server + response.next_uri;
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
                var response = await Task.Run(async () => JsonConvert.DeserializeObject<Response<Stream>>(await httpClient.GetStringAsync(uriEndPoint)));

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
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null && propertyName.Length > 0)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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