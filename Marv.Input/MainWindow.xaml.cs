using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Marv.Common;
using Marv.Common.Types;
using Marv.Controls;
using Microsoft.Win32;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string chartTitle;
        private Graph graph;
        private HorizontalAxisQuantity horizontalAxisQuantity = HorizontalAxisQuantity.Section;
        private bool isGraphControlVisible = true;
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private ILineData lineData;
        private string lineDataFileName;
        private Network network;
        private NotificationCollection notifications = new NotificationCollection();
        private string selectedSectionId;
        private int selectedYear;

        public string ChartTitle
        {
            get { return this.chartTitle; }

            set
            {
                if (value.Equals(this.chartTitle))
                {
                    return;
                }

                this.chartTitle = value;
                this.RaisePropertyChanged();
            }
        }

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

        public HorizontalAxisQuantity HorizontalAxisQuantity
        {
            get { return this.horizontalAxisQuantity; }

            set
            {
                if (value.Equals(this.horizontalAxisQuantity))
                {
                    return;
                }

                this.horizontalAxisQuantity = value;
                this.RaisePropertyChanged();
            }
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

        public Network Network
        {
            get { return this.network; }

            set
            {
                if (value.Equals(this.network))
                {
                    return;
                }

                this.network = value;
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

        public string SelectedSectionId
        {
            get { return this.selectedSectionId; }

            set
            {
                if (value.Equals(this.selectedSectionId))
                {
                    return;
                }

                this.selectedSectionId = value;
                this.RaisePropertyChanged();
            }
        }

        public int SelectedYear
        {
            get { return this.selectedYear; }

            set
            {
                if (value.Equals(this.selectedYear))
                {
                    return;
                }

                this.selectedYear = value;
                this.RaisePropertyChanged();
            }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();
        }

        private object GetChartCategory()
        {
            return this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section ? this.SelectedSectionId : this.SelectedYear as object;
        }

        private object GetChartCategory(string sectionId, int year)
        {
            return this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section ? sectionId : year as object;
        }

        private void GraphControl_EvidenceEntered(object sender, VertexEvidence vertexEvidence)
        {
            if (this.SelectedSectionId != null && this.SelectedYear > 0 && this.Graph.SelectedVertex != null)
            {
                this.LineData.GetEvidence(this.SelectedSectionId)[this.SelectedYear][this.Graph.SelectedVertex.Key] = vertexEvidence;
            }

            if (vertexEvidence == null)
            {
                this.LineDataChart.RemoveUserEvidence(this.GetChartCategory());
                this.LineDataControl.ClearSelectedCell();
            }
            else
            {
                this.LineDataChart.SetUserEvidence(this.GetChartCategory(), vertexEvidence);
                this.LineDataControl.SetEvidence(vertexEvidence);
            }
        }

        private void GraphControl_GraphChanged(object sender, ValueChangedEventArgs<Graph> e)
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
                var selectedVertex = this.Graph.SelectedVertex;

                this.LineDataControl.ClearRows();

                foreach (var sectionId in this.LineData.SectionIds)
                {
                    var sectionEvidence = await this.LineData.GetEvidenceAsync(sectionId);

                    if (selectedVertex != null)
                    {
                        this.LineDataControl.AddRow(sectionId, sectionEvidence[null, selectedVertex.Key]);
                    }
                }

                if (selectedVertex == null)
                {
                    return;
                }

                var intervals = selectedVertex.Intervals.ToArray();

                this.LineDataChart.SetVerticalAxis(selectedVertex.SafeMax, selectedVertex.SafeMin, intervals);

                this.LineDataChart.SetUserEvidence(this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section
                                                       ? this.LineData.GetEvidence(null, this.SelectedYear, this.Graph.SelectedVertex.Key)
                                                       : this.LineData.GetEvidence(this.SelectedSectionId, null, this.Graph.SelectedVertex.Key));
            }
        }

        private void LineDataChart_EvidenceGenerated(object sender, EvidenceGeneratedEventArgs e)
        {
            var vertexEvidence = this.Graph.SelectedVertex.States.ParseEvidenceString(e.EvidenceString);

            var sectionId = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section ? e.Category as string : this.SelectedSectionId;
            var year = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section ? this.SelectedYear : (int) e.Category;

            if (vertexEvidence.Type != VertexEvidenceType.Invalid)
            {
                var sectionEvidence = this.LineData.GetEvidence(sectionId);
                sectionEvidence[year][this.Graph.SelectedVertex.Key] = vertexEvidence;
                this.LineData.SetEvidence(sectionId, sectionEvidence);

                this.LineDataChart.SetUserEvidence(e.Category, vertexEvidence);
                this.LineDataControl.SetEvidence(sectionId, year, vertexEvidence);
            }
        }

        private void LineDataChart_HorizontalAxisQuantityChanged(object sender, HorizontalAxisQuantity e)
        {
            this.LineDataChart.SetUserEvidence(this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section
                                                   ? this.LineData.GetEvidence(null, this.SelectedYear, this.Graph.SelectedVertex.Key)
                                                   : this.LineData.GetEvidence(this.SelectedSectionId, null, this.Graph.SelectedVertex.Key));
            this.UpdateChartTitle();
        }

        private void LineDataOpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Common.LineData.FileDescription + "|*." + Common.LineData.FileExtension,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                this.lineDataFileName = dialog.FileName;

                var directoryName = Path.GetDirectoryName(dialog.FileName);

                if (Directory.Exists(Path.Combine(directoryName, "SectionBeliefs")) && Directory.Exists(Path.Combine(directoryName, "SectionEvidences")))
                {
                    // This is a folder line data
                    this.LineData = LineDataFolder.Read(dialog.FileName);
                }
                else
                {
                    this.LineData = Common.LineData.Read(dialog.FileName);
                }
            }
        }

        private void LineDataSaveAs()
        {
            var dialog = new SaveFileDialog
            {
                Filter = Common.LineData.FileDescription + "|*." + Common.LineData.FileExtension,
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                this.lineDataFileName = dialog.FileName;
                this.LineData.Write(this.lineDataFileName);
            }
        }

        private void LineDataSaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.LineDataSaveAs();
        }

        private void LineDataSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.lineDataFileName == null)
            {
                this.LineDataSaveAs();
            }
            else
            {
                this.LineData.Write(this.lineDataFileName);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.PropertyChanged -= MainWindow_PropertyChanged;
            this.PropertyChanged += MainWindow_PropertyChanged;
        }

        private async void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LineData")
            {
                if (this.LineData != null)
                {
                    this.LineDataControl.ClearRows();

                    foreach (var sectionId in this.LineData.SectionIds)
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
            foreach (var sectionId in this.LineData.SectionIds)
            {
                var sectionEvidence = this.lineData.GetEvidence(sectionId);
                this.lineData.SetBelief(sectionId, this.Network.Run(sectionEvidence));
            }
        }

        private void RunSectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sectionEvidence = this.LineData.GetEvidence(this.SelectedSectionId);

            var sectionBelief = this.Network.Run(sectionEvidence);

            this.Graph.Belief = sectionBelief[this.SelectedYear];
            this.LineData.SetBelief(this.SelectedSectionId, sectionBelief);
        }

        private void UpdateChartTitle()
        {
            this.ChartTitle = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section ? "Year: " + this.SelectedYear : "Section: " + this.SelectedSectionId;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}