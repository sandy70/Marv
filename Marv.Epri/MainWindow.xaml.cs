using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Marv.Common;
using Marv.Common.Types;
using Marv.Epri.Properties;
using Newtonsoft.Json;

namespace Marv.Epri
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1),
        };

        private ObservableCollection<DataPoint> dataPoints = new ObservableCollection<DataPoint>();
        private Task<ObservableCollection<DataPoint>> downloadDataPointsTask;
        private NotificationCollection notifications = new NotificationCollection();
        private TimeSpan selectedTimeSpan;

        private Dict<string, TimeSpan> timespans = new Dict<string, TimeSpan>
        {
            { "1 Hour", TimeSpan.FromHours(1) },
            { "6 Hours", TimeSpan.FromHours(6) },
            { "1 Day", TimeSpan.FromDays(1) },
            { "7 Days", TimeSpan.FromDays(7) }
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

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await this.UpdateDataPoints();
        }

        private async Task<ObservableCollection<DataPoint>> DownloadDataPointsAsync()
        {
            const string server = @"http://devicecloud.digi.com";

            const string uriEndPoint = server + "/ws/v1/streams/history/00000000-00000000-00409DFF-FF88AC6C/Sensor01.json";

            var uri = uriEndPoint + "?" + Utils.FormRestArgs(new
            {
                start_time = (DateTime.UtcNow - this.SelectedTimeSpan).ToString("o"),
                timeline = "server"
            });

            using (var httpClient = new HttpClient(new HttpClientHandler { Credentials = new NetworkCredential(Settings.Default.Login, Settings.Default.Password) }))
            {
                var downloadedPoints = new ObservableCollection<DataPoint>();

                while (true)
                {
                    var response = await Task.Run(async () => JsonConvert.DeserializeObject<Response>(await httpClient.GetStringAsync(uri)));

                    downloadedPoints.Add(response.list);

                    if (response.next_uri == null)
                    {
                        break;
                    }

                    uri = server + response.next_uri;
                }

                return downloadedPoints;
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await this.UpdateDataPoints();
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null && propertyName.Length > 0)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async Task UpdateDataPoints()
        {
            if (this.downloadDataPointsTask != null && this.downloadDataPointsTask.Status == TaskStatus.Running)
            {
                this.downloadDataPointsTask.Wait();
            }

            var notification = new Notification
            {
                IsIndeterminate = true,
                Description = "Loading..."
            };

            this.Notifications.Add(notification);

            this.DataPoints = await (this.downloadDataPointsTask = this.DownloadDataPointsAsync());

            this.Notifications.Remove(notification);
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            await this.UpdateDataPoints();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}