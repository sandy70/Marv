using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using OxyPlot;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (NotificationCollection), typeof (MainWindow), new PropertyMetadata(new NotificationCollection()));

        private LineData lineData;

        public Graph Graph
        {
            get
            {
                return (Graph) GetValue(GraphProperty);
            }

            set
            {
                SetValue(GraphProperty, value);
            }
        }

        public LineData LineData
        {
            get
            {
                return this.lineData;
            }

            set
            {
                if (value.Equals(this.lineData))
                {
                    return;
                }

                this.lineData = value;
                this.RaisePropertyChanged();
            }
        }

        public NotificationCollection Notifications
        {
            get
            {
                return (NotificationCollection) GetValue(NotificationsProperty);
            }

            set
            {
                SetValue(NotificationsProperty, value);
            }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GraphControl_EvidenceEntered(object sender, Vertex vertex)
        {
            this.LineDataControl.SetSelectedCells(vertex.Data);
        }

        private void GraphControl_GraphChanged(object sender, ValueChangedArgs<Graph> e)
        {
            this.LineData = new LineData();
            this.LineData.Sections["Section 1"] = new Dict<int, string, VertexData>();
        }

        private void LineDataControl_CellChanged(object sender, CellModel cellModel)
        {
            if (cellModel.IsColumnSectionId)
            {
                return;
            }

            this.Graph.NetworkStructure.Run(this.LineData.Sections[cellModel.SectionId]);
            this.LineDataControl.UpdateCurrentGraphData();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;
            this.GraphControl.GraphChanged += GraphControl_GraphChanged;

            this.LineDataControl.CellChanged -= LineDataControl_CellChanged;
            this.LineDataControl.CellChanged += LineDataControl_CellChanged;

            this.VertexControl.EvidenceEntered += this.GraphControl_EvidenceEntered;
        }

        public bool IsSelectionSquare()
        {
            var rowIndices = new List<int>();
            var colIndices = new List<int>();

            //foreach (var selectedCell in this.InputGridView.SelectedCells)
            //{
            //    Common.Extensions.AddUnique(rowIndices, this.InputRows.IndexOf(selectedCell.Item as dynamic));
            //    colIndices.AddUnique(selectedCell.Column.DisplayIndex);
            //}

            //var total = (rowIndices.Max() - rowIndices.Min() + 1) * (colIndices.Max() - colIndices.Min() + 1);

            //return total == this.InputGridView.SelectedCells.Count;
            return true;
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}