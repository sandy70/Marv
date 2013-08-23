using Caching;
using LibBn;
using LibPipeline;
using Smile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace Marv
{
    public partial class MainWindow : Window
    {
        public Dictionary<int, List<BnVertexValue>> DefaultVertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();
        public BnInputStore InputManager = new BnInputStore();
        public SensorListener SensorListener = new SensorListener();
        public Dictionary<int, List<BnVertexValue>> VertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();
        private object _lock = new object();
        private NearNeutralPhSccModel model = new NearNeutralPhSccModel();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();

            var cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MARV");
            MapControl.TileImageLoader.Cache = new ImageFileCache(MapControl.TileImageLoader.DefaultCacheName, cacheDirectory);

            this.MapView.TileLayer = TileLayers.MapBoxTerrain;
        }

        public void AddInput(BnVertexViewModel vertexViewModel)
        {
            var vertexInput = this.InputManager.GetVertexInput(BnInputType.User, this.SelectedYear, vertexViewModel.Key);
            vertexInput.FillFrom(vertexViewModel);
        }

        public async Task<LocationValue> GetLocationValueAsync(Location location)
        {
            Console.WriteLine("LocationValueStore: getting location value with id: " + location.Guid.ToInt64());

            var filesPerFolder = 1000;
            var extension = ".db";

            var dataBase = new ObjectDataBase<LocationValue>()
            {
                FileNamePredicate = () =>
                {
                    string folderName = (location.Guid.ToInt64() / filesPerFolder).ToString();
                    string fileName = (location.Guid.ToInt64() % filesPerFolder).ToString() + extension;

                    return Path.Combine(folderName, fileName);
                }
            };

            var locationValues = await dataBase.ReadValuesAsync(x => x.Id == location.Guid.ToInt64());

            LocationValue locationValue = null;

            if (locationValues != null && locationValues.Count() > 0)
            {
                Console.WriteLine("LocationValueStore: location value for id: " + location.Guid.ToInt64() + " found in database.");
                locationValue = locationValues.First();
            }
            else
            {
                Console.WriteLine("LocationValueStore: location value for id: " + location.Guid.ToInt64() + " NOT found in database.");
                this.PopupControl.ShowTextIndeterminate("Running model.");
                locationValue = await NearNeutralPhSccModel.RunAsync(location, this.Graphs, this.StartYear, this.EndYear);
                locationValue.Id = location.Guid.ToInt64();
                await dataBase.WriteAsync(locationValue);
                this.PopupControl.Hide();
            }

            return locationValue;
        }

        public LocationValue GetLocationValue(Location location)
        {
            Console.WriteLine("LocationValueStore: getting location value with id: " + location.Guid.ToInt64());

            var filesPerFolder = 1000;
            var extension = ".db";

            var dataBase = new ObjectDataBase<LocationValue>()
            {
                FileNamePredicate = () =>
                {
                    string folderName = (location.Guid.ToInt64() / filesPerFolder).ToString();
                    string fileName = (location.Guid.ToInt64() % filesPerFolder).ToString() + extension;

                    return Path.Combine(folderName, fileName);
                }
            };

            var locationValues = dataBase.ReadValues(x => x.Id == location.Guid.ToInt64());

            LocationValue locationValue = null;

            if (locationValues != null && locationValues.Count() > 0)
            {
                Console.WriteLine("LocationValueStore: location value for id: " + location.Guid.ToInt64() + " found in database.");
                locationValue = locationValues.First();
            }
            else
            {
                Console.WriteLine("LocationValueStore: location value for id: " + location.Guid.ToInt64() + " NOT found in database.");
                locationValue = NearNeutralPhSccModel.Run(location, this.Graphs, this.StartYear, this.EndYear);
                locationValue.Id = location.Guid.ToInt64();
                dataBase.Write(locationValue);
            }

            return locationValue;
        }

        public void RemoveInput(BnVertexViewModel vertexViewModel)
        {
            this.InputManager.RemoveVertexInput(BnInputType.User, this.SelectedYear, vertexViewModel.Key);
        }

        public bool TryUpdateNetwork()
        {
            try
            {
                var defaultInputs = this.InputManager.GetGraphInput(BnInputType.Default, this.SelectedYear);
                var userInputs = this.InputManager.GetGraphInput(BnInputType.User, this.SelectedYear);

                var bnUpdater = new BnUpdater();

                List<BnVertexValue> lastYearVertexValues = null;

                if (this.SelectedYear == Config.StartYear)
                {
                    lastYearVertexValues = null;
                }
                else
                {
                    lastYearVertexValues = this.VertexValuesByYear[this.SelectedYear - 1];
                }

                return true;
            }
            catch (SmileException exp)
            {
                return false;
            }
        }


    }
}