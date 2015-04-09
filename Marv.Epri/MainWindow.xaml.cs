using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Marv.Epri
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            const string uri = @"http://devicecloud.digi.com/ws/v1/streams/history/00000000-00000000-00409DFF-FF88AC6C/Sensor01.json?timeline=server&order=desc&size=50";

            using (var httpClient = new HttpClient(new HttpClientHandler { Credentials = new NetworkCredential("vkhare13", "!Ncc1764") }))
            {
                var response = await JsonConvert.DeserializeObjectAsync<Response>(await httpClient.GetStringAsync(uri));
                Console.WriteLine(response);
            }
        }
    }
}
