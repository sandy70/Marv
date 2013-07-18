using LibBn;
using LibPipeline;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Marv
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty CacheDirectoryProperty =
        DependencyProperty.Register("CacheDirectory", typeof(string), typeof(MainWindow), new PropertyMetadata(".\\"));

        public static readonly DependencyProperty EndYearProperty =
        DependencyProperty.Register("EndYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2010));

        public static readonly DependencyProperty GraphsProperty =
        DependencyProperty.Register("Graphs", typeof(GraphCollection), typeof(MainWindow), new PropertyMetadata(new GraphCollection()));

        public static readonly DependencyProperty IsGroupButtonVisibleProperty =
        DependencyProperty.Register("IsGroupButtonVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsMapVisibleProperty =
        DependencyProperty.Register("IsMapVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsMenuVisibleProperty =
        DependencyProperty.Register("IsMenuVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsProfileSelectedProperty =
        DependencyProperty.Register("IsProfileSelected", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsSensorButtonVisibleProperty =
        DependencyProperty.Register("IsSensorButtonVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsSettingsControlVisibleProperty =
        DependencyProperty.Register("IsSettingsControlVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsSettingsVisibleProperty =
        DependencyProperty.Register("IsSettingsVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTallySelectedProperty =
        DependencyProperty.Register("IsTallySelected", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsYearSliderVisibleProperty =
        DependencyProperty.Register("IsYearSliderVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty LocationValueStoreProperty =
        DependencyProperty.Register("LocationValueStore", typeof(LocationValueStore), typeof(MainWindow), new PropertyMetadata(new LocationValueStore()));

        public static readonly DependencyProperty MultiLocationsProperty =
        DependencyProperty.Register("MultiLocations", typeof(SelectableCollection<MultiLocation>), typeof(MainWindow), new PropertyMetadata(null, ChangedMultiLocations));

        public static readonly DependencyProperty MultiPointsProperty =
        DependencyProperty.Register("MultiPoints", typeof(ObservableCollection<MultiPoint>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<MultiPoint>()));

        public static readonly DependencyProperty NetworkFileNamesProperty =
        DependencyProperty.Register("NetworkFileNames", typeof(SelectableStringCollection), typeof(MainWindow), new PropertyMetadata(null, ChangedNetworkFileNames));

        public static readonly DependencyProperty SelectedLocationValueProperty =
        DependencyProperty.Register("SelectedLocationValue", typeof(LocationValue), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedVertexValuesProperty =
        DependencyProperty.Register("SelectedVertexValues", typeof(IEnumerable<BnVertexValue>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedYearProperty =
        DependencyProperty.Register("SelectedYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2000, ChangedSelectedYear));

        public static readonly DependencyProperty StartingGroupProperty =
        DependencyProperty.Register("StartingGroup", typeof(string), typeof(MainWindow), new PropertyMetadata("all"));

        public static readonly DependencyProperty StartYearProperty =
        DependencyProperty.Register("StartYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2000));

        public string CacheDirectory
        {
            get { return (string)GetValue(CacheDirectoryProperty); }
            set { SetValue(CacheDirectoryProperty, value); }
        }

        public int EndYear
        {
            get { return (int)GetValue(EndYearProperty); }
            set { SetValue(EndYearProperty, value); }
        }

        public GraphCollection Graphs
        {
            get { return (GraphCollection)GetValue(GraphsProperty); }
            set { SetValue(GraphsProperty, value); }
        }

        public bool IsGroupButtonVisible
        {
            get { return (bool)GetValue(IsGroupButtonVisibleProperty); }
            set { SetValue(IsGroupButtonVisibleProperty, value); }
        }

        public bool IsMapVisible
        {
            get { return (bool)GetValue(IsMapVisibleProperty); }
            set { SetValue(IsMapVisibleProperty, value); }
        }

        public bool IsMenuVisible
        {
            get { return (bool)GetValue(IsMenuVisibleProperty); }
            set { SetValue(IsMenuVisibleProperty, value); }
        }

        public bool IsSensorButtonVisible
        {
            get { return (bool)GetValue(IsSensorButtonVisibleProperty); }
            set { SetValue(IsSensorButtonVisibleProperty, value); }
        }

        public bool IsTallySelected
        {
            get { return (bool)GetValue(IsTallySelectedProperty); }
            set { SetValue(IsTallySelectedProperty, value); }
        }

        public bool IsYearSliderVisible
        {
            get { return (bool)GetValue(IsYearSliderVisibleProperty); }
            set { SetValue(IsYearSliderVisibleProperty, value); }
        }

        public LocationValueStore LocationValueStore
        {
            get { return (LocationValueStore)GetValue(LocationValueStoreProperty); }
            set { SetValue(LocationValueStoreProperty, value); }
        }

        public SelectableCollection<MultiLocation> MultiLocations
        {
            get { return (SelectableCollection<MultiLocation>)GetValue(MultiLocationsProperty); }
            set { SetValue(MultiLocationsProperty, value); }
        }

        public ObservableCollection<MultiPoint> MultiPoints
        {
            get { return (ObservableCollection<MultiPoint>)GetValue(MultiPointsProperty); }
            set { SetValue(MultiPointsProperty, value); }
        }

        public NearNeutralPhSccModel NearNeutralPhSccModel
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
            }
        }

        public SelectableStringCollection NetworkFileNames
        {
            get { return (SelectableStringCollection)GetValue(NetworkFileNamesProperty); }
            set { SetValue(NetworkFileNamesProperty, value); }
        }

        public LocationValue SelectedLocationValue
        {
            get
            {
                return (LocationValue)GetValue(SelectedLocationValueProperty);
            }

            set
            {
                SetValue(SelectedLocationValueProperty, value);

                var modelValue = this.SelectedLocationValue[this.SelectedYear];

                foreach (var graph in this.Graphs)
                {
                    var graphValue = modelValue[graph.Name];
                    graph.Value = graphValue;
                }
            }
        }

        public IEnumerable<BnVertexValue> SelectedVertexValues
        {
            get { return (IEnumerable<BnVertexValue>)GetValue(SelectedVertexValuesProperty); }
            set { SetValue(SelectedVertexValuesProperty, value); }
        }

        public int SelectedYear
        {
            get { return (int)GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        public string StartingGroup
        {
            get { return (string)GetValue(StartingGroupProperty); }
            set { SetValue(StartingGroupProperty, value); }
        }

        public int StartYear
        {
            get { return (int)GetValue(StartYearProperty); }
            set { SetValue(StartYearProperty, value); }
        }

        private static async void ChangedMultiLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;

            if (window.MultiLocations != null)
            {
                window.MultiLocations.SelectionChanged += async (s1, e1) =>
                    {
                        var multiLocation = e1.Value;

                        if (multiLocation != null)
                        {
                            multiLocation.SelectionChanged += async (s2, e2) =>
                                {
                                    var location = e2.Value;

                                    if (location != null)
                                    {
                                        window.SelectedLocationValue = await window.GetLocationValueAsync(location);
                                    }
                                };

                            var loc = multiLocation.SelectedItem;

                            if (loc != null)
                            {
                                window.SelectedLocationValue = await window.GetLocationValueAsync(loc);
                            }
                        }
                    };

                var mLocation = window.MultiLocations.SelectedItem;

                if (mLocation != null)
                {
                    var location = mLocation.SelectedItem;

                    if (location != null)
                    {
                        window.SelectedLocationValue = await window.GetLocationValueAsync(location);
                    }
                }
            }
        }

        private static async void ChangedNetworkFileNames(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;

            window.NetworkFileNames.CollectionChanged += async (sender, ev) =>
                {
                    if (ev.NewItems != null)
                    {
                        foreach (var fileName in ev.NewItems)
                        {
                            window.Graphs.Add(await BnGraph.ReadAsync<BnVertexViewModel>(fileName as string));
                        }
                    }

                    if (ev.OldItems != null)
                    {
                        foreach (var fileName in ev.OldItems)
                        {
                            var graphToRemove = window.Graphs.Where(x => x.FileName.Equals(fileName as string)).FirstOrDefault();

                            if (graphToRemove != null)
                            {
                                window.Graphs.Remove(graphToRemove);
                            }
                        }
                    }
                };

            var newGraphs = new GraphCollection();

            foreach (var fileName in window.NetworkFileNames)
            {
                var graph = await BnGraph.ReadAsync<BnVertexViewModel>(fileName);

                if (graph.Vertices.Count() > 0)
                {
                    newGraphs.Add(graph);
                }
                else
                {
                    window.PopupControl.ShowText("File not found: " + fileName);
                }
            }

            window.Graphs = newGraphs;
        }
    }
}