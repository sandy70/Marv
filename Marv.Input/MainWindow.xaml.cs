using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Marv.Common;
using Microsoft.Win32;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (NotificationCollection), typeof (MainWindow), new PropertyMetadata(new NotificationCollection()));

        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof (int), typeof (MainWindow), new PropertyMetadata(int.MinValue));

        private bool isGraphControlVisible = true;
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private bool isVertexControlVisible = true;
        private ILineData lineData;
        private string lineDataFileName;
        private LocationCollection locations;
        private Location selectedLocation;

        public Graph Graph
        {
            get { return (Graph) GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        public bool IsGraphControlVisible
        {
            get { return this.isGraphControlVisible; }

            set
            {
                if (value.Equals(this.isGraphControlVisible))
                {
                    return;
                }

                this.isGraphControlVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsLineDataChartVisible
        {
            get { return this.isLineDataChartVisible; }

            set
            {
                if (value.Equals(this.isLineDataChartVisible))
                {
                    return;
                }

                this.isLineDataChartVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsLineDataControlVisible
        {
            get { return this.isLineDataControlVisible; }

            set
            {
                if (value.Equals(this.isLineDataControlVisible))
                {
                    return;
                }

                this.isLineDataControlVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsVertexControlVisible
        {
            get { return this.isVertexControlVisible; }

            set
            {
                if (value.Equals(this.isVertexControlVisible))
                {
                    return;
                }

                this.isVertexControlVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public ILineData LineData
        {
            get { return this.lineData; }

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

        public LocationCollection Locations
        {
            get { return this.locations; }

            set
            {
                if (value.Equals(this.locations))
                {
                    return;
                }

                this.locations = value;
                this.RaisePropertyChanged();
            }
        }

        public NotificationCollection Notifications
        {
            get { return (NotificationCollection) GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        public Location SelectedLocation
        {
            get { return this.selectedLocation; }

            set
            {
                if (value.Equals(this.selectedLocation))
                {
                    return;
                }

                this.selectedLocation = value;
                this.RaisePropertyChanged();
            }
        }

        public string SelectedSectionId
        {
            get { return (string) GetValue(SelectedSectionIdProperty); }
            set { SetValue(SelectedSectionIdProperty, value); }
        }

        public int SelectedYear
        {
            get { return (int) GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void GraphControl_EvidenceEntered(object sender, VertexEvidence vertexEvidence)
        {
            if (vertexEvidence == null)
            {
                this.LineDataChart.RemoveSelectedEvidence();
                this.LineDataControl.ClearSelectedCell();
            }
            else
            {
                this.LineDataChart.SetEvidence(vertexEvidence);
                this.LineDataControl.SetSelectedCells(vertexEvidence);
            }
        }

        private void GraphControl_GraphChanged(object sender, Graph oldGraph, Graph newGraph)
        {
            if (this.LineData == null)
            {
                this.LineData = new LineData();
                this.LineData.SetEvidence("Section 1", new Dict<int, string, VertexEvidence>());
            }
        }

        private async void GraphControl_SelectionChanged(object sender, Vertex e)
        {
            if (this.LineData != null)
            {
                this.LineDataControl.ClearRows();

                foreach (var sectionId in this.LineData.GetSectionIds())
                {
                    this.LineDataControl.AddRow(sectionId, (await this.LineData.GetEvidenceAsync(sectionId))[null, this.Graph.SelectedVertex.Key]);
                }
            }
        }

        private void LineDataChart_EvidenceGenerated(object sender, EvidenceGeneratedEventArgs e)
        {
            var vertexEvidence = this.Graph.SelectedVertex.States.ParseEvidenceString(e.EvidenceString);

            if (vertexEvidence.Type != VertexEvidenceType.Invalid)
            {
                var sectionEvidence = this.LineData.GetEvidence(e.SectionId);
                sectionEvidence[e.Year][this.Graph.SelectedVertex.Key] = vertexEvidence;
                this.LineData.SetEvidence(e.SectionId, sectionEvidence);

                this.LineDataControl.SetCell(e.SectionId, e.Year, vertexEvidence);
            }
        }

        private void LineDataControl_CellChanged(object sender, CellChangedEventArgs e)
        {
            if (e.CellModel.IsColumnSectionId)
            {
                if (e.OldString == null)
                {
                    this.LineData.SetEvidence(e.NewString, new Dict<int, string, VertexEvidence>());
                }
                else
                {
                    this.LineData.ReplaceSectionId(e.OldString, e.NewString);
                }

                e.CellModel.Data = e.NewString;
            }
            else
            {
                var vertexEvidence = e.VertexEvidence ?? this.Graph.SelectedVertex.States.ParseEvidenceString(e.NewString);

                e.CellModel.Data = vertexEvidence;

                this.LineData.SetEvidence(e.CellModel.SectionId, e.CellModel.Year, this.Graph.SelectedVertex.Key, vertexEvidence);
                this.LineDataChart.SetEvidence(e.CellModel.SectionId, e.CellModel.Year, vertexEvidence);
            }
        }

        private void LineDataControl_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            var vertexEvidence = this.Graph.SelectedVertex.States.ParseEvidenceString(e.NewValue as string);

            if (vertexEvidence.Type == VertexEvidenceType.Invalid)
            {
                e.IsValid = false;
                e.ErrorMessage = "Not a correct value or range of values. Press ESC to cancel.";
            }
        }

        private void LineDataControl_NotificationClosed(object sender, Notification notification)
        {
            this.Notifications.Remove(notification);
        }

        private void LineDataControl_NotificationOpened(object sender, Notification notification)
        {
            this.Notifications.Add(notification);
        }

        private void LineDataControl_RowAdded(object sender, string sectionId)
        {
            this.LineData.SetEvidence(sectionId, new Dict<int, string, VertexEvidence>());
        }

        private void LineDataControl_RowRemoved(object sender, string sectionId)
        {
            this.LineData.RemoveSection(sectionId);
        }

        private void LineDataControl_SectionEvidencesChanged(object sender, EventArgs e)
        {
            this.LineDataChart.UpdateBasePoints();
        }

        private void LineDataControl_SelectedCellChanged(object sender, EventArgs e)
        {
            this.Graph.Belief = this.LineData.GetBelief(this.SelectedSectionId)[this.SelectedYear];
            this.Graph.SetEvidence(this.LineData.GetEvidence(this.SelectedSectionId)[this.SelectedYear]);
        }

        private void LineDataOpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Marv.LineData.FileDescription + "|*." + Marv.LineData.FileExtension,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                this.lineDataFileName = dialog.FileName;

                var directoryName = Path.GetDirectoryName(dialog.FileName);

                if (Directory.Exists(Path.Combine(directoryName, "SectionBeliefs")) && Directory.Exists(Path.Combine(directoryName, "SectionEvidences")))
                {
                    // This is a folder line data
                    this.LineData = FolderLineData.Read(dialog.FileName);
                }
                else
                {
                    this.LineData = Marv.LineData.Read(dialog.FileName);
                }
            }
        }

        private void LineDataSaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = Marv.LineData.FileDescription + "|*." + Marv.LineData.FileExtension,
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                this.lineDataFileName = dialog.FileName;
                this.LineData.Write(this.lineDataFileName);
            }
        }

        private void LineDataSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.LineData.Write(this.lineDataFileName);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.PropertyChanged -= MainWindow_PropertyChanged;
            this.PropertyChanged += MainWindow_PropertyChanged;

            this.GraphControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;

            this.GraphControl.GraphChanged -= GraphControl_GraphChanged;
            this.GraphControl.GraphChanged += GraphControl_GraphChanged;

            this.GraphControl.SelectionChanged -= GraphControl_SelectionChanged;
            this.GraphControl.SelectionChanged += GraphControl_SelectionChanged;

            this.LineDataChart.EvidenceGenerated -= LineDataChart_EvidenceGenerated;
            this.LineDataChart.EvidenceGenerated += LineDataChart_EvidenceGenerated;

            this.LineDataControl.CellChanged -= LineDataControl_CellChanged;
            this.LineDataControl.CellChanged += LineDataControl_CellChanged;

            this.LineDataControl.CellValidating -= LineDataControl_CellValidating;
            this.LineDataControl.CellValidating += LineDataControl_CellValidating;

            this.LineDataControl.NotificationClosed -= LineDataControl_NotificationClosed;
            this.LineDataControl.NotificationClosed += LineDataControl_NotificationClosed;

            this.LineDataControl.NotificationOpened -= LineDataControl_NotificationOpened;
            this.LineDataControl.NotificationOpened += LineDataControl_NotificationOpened;

            this.LineDataControl.RowAdded -= LineDataControl_RowAdded;
            this.LineDataControl.RowAdded += LineDataControl_RowAdded;

            this.LineDataControl.RowRemoved -= LineDataControl_RowRemoved;
            this.LineDataControl.RowRemoved += LineDataControl_RowRemoved;

            this.LineDataControl.SectionEvidencesChanged -= LineDataControl_SectionEvidencesChanged;
            this.LineDataControl.SectionEvidencesChanged += LineDataControl_SectionEvidencesChanged;

            this.LineDataControl.SelectedCellChanged -= LineDataControl_SelectedCellChanged;
            this.LineDataControl.SelectedCellChanged += LineDataControl_SelectedCellChanged;

            this.VertexControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.VertexControl.EvidenceEntered += GraphControl_EvidenceEntered;
        }

        private async void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LineData")
            {
                if (this.LineData != null)
                {
                    this.LineDataControl.ClearRows();

                    foreach (var sectionId in this.LineData.GetSectionIds())
                    {
                        this.LineDataControl.AddRow(sectionId, (await this.LineData.GetEvidenceAsync(sectionId))[null, this.Graph.SelectedVertex.Key]);
                    }
                }
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RunLineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sectionIds = this.LineData.GetSectionIds().ToList();

            foreach (var sectionId in sectionIds)
            {
                var sectionEvidence = lineData.GetEvidence(sectionId);
                lineData.SetBelief(sectionId, this.Graph.Network.Run(sectionEvidence));
            }
        }

        private void RunSectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sectionEvidence = this.LineData.GetEvidence(this.SelectedSectionId);

            var sectionBelief = this.Graph.Network.Run(sectionEvidence);

            this.Graph.Belief = sectionBelief[this.SelectedYear];
            this.LineData.SetBelief(this.SelectedSectionId, sectionBelief);
        }

        private void UpdateLineDataControl()
        {
            if (this.LineData != null)
            {
                this.LineDataControl.ClearRows();

                foreach (var sectionId in this.LineData.GetSectionIds())
                {
                    this.LineDataControl.AddRow(sectionId, this.LineData.GetEvidence(sectionId)[null, this.Graph.SelectedVertex.Key]);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}