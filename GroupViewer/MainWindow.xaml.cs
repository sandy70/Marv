using LibBn;
using LibPipeline;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Telerik.Windows.Controls;

namespace GroupViewer
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DisplayGraphProperty =
        DependencyProperty.Register("DisplayGraph", typeof(BnGraph), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty FileNameProperty =
        DependencyProperty.Register("FileName", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty IsGroupedProperty =
        DependencyProperty.Register("IsGrouped", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty SelectedGroupProperty =
        DependencyProperty.Register("SelectedGroup", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedVertexViewModelProperty =
        DependencyProperty.Register("SelectedVertexViewModel", typeof(BnVertexViewModel), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SourceGraphProperty =
        DependencyProperty.Register("SourceGraph", typeof(BnGraph), typeof(MainWindow), new PropertyMetadata(null));

        private ObservableCollection<int> groups = new ObservableCollection<int>();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BnGraph DisplayGraph
        {
            get { return (BnGraph)GetValue(DisplayGraphProperty); }
            set { SetValue(DisplayGraphProperty, value); }
        }

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public ObservableCollection<int> Groups
        {
            get
            {
                return this.groups;
            }

            set
            {
                if (value != this.groups)
                {
                    this.groups = value;
                    this.OnPropertyChanged("Groups");
                }
            }
        }

        public bool IsGrouped
        {
            get { return (bool)GetValue(IsGroupedProperty); }
            set { SetValue(IsGroupedProperty, value); }
        }

        public string SelectedGroup
        {
            get { return (string)GetValue(SelectedGroupProperty); }
            set { SetValue(SelectedGroupProperty, value); }
        }

        public BnVertexViewModel SelectedVertexViewModel
        {
            get { return (BnVertexViewModel)GetValue(SelectedVertexViewModelProperty); }
            set { SetValue(SelectedVertexViewModelProperty, value); }
        }

        public BnGraph SourceGraph
        {
            get { return (BnGraph)GetValue(SourceGraphProperty); }
            set { SetValue(SourceGraphProperty, value); }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}