using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Marv.Common;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private void Plot(string columnName)
        {
            this.Chart.AddNodeStateLines(this.SelectedVertex, this.PipeLineData.BaseTableMax, this.PipeLineData.BaseTableMin);

            if (this.PipeLineData.CommentBlocks != null)
            {
                foreach (var row in this.PipeLineData.CommentBlocks)
                {
                    this.Chart.UpdateCommentBlocks(row, VerticalAxis);
                }
            }

            foreach (var row in this.Table)
            {
                this.Plot(row, columnName);
            }
        }

        private void Plot(EvidenceRow dataRow, string columnName)
        {
            var fillBrush = new SolidColorBrush(Colors.Goldenrod);
            var strokeBrush = new SolidColorBrush(Colors.Goldenrod);

            var from = (double) dataRow["From"];
            var to = (double) dataRow["To"];
            var vertexEvidence = dataRow[columnName] as VertexEvidence;

            if (vertexEvidence == null)
            {
                this.Chart.Annotations.Remove(annotation => ReferenceEquals(annotation.Tag, dataRow));
                return;
            }

            if (vertexEvidence.Type == VertexEvidenceType.Invalid)
            {
               
              //  MessageBox.Show("Invalid input entered");
                dataRow[columnName] = new VertexEvidence { Type = VertexEvidenceType.Null };
                return;
            }
            // Remove older annotations

            this.Chart.Annotations.Remove(annotation => ReferenceEquals(annotation.Tag, dataRow));

            this.Chart.AddNodeStateLines(this.SelectedVertex, this.PipeLineData.BaseTableMax, this.PipeLineData.BaseTableMin);

            if (selectedTheme == DataTheme.User)
            {
                if (vertexEvidence.Type == VertexEvidenceType.Number)
                {
                    if (from == to)
                    {
                        this.Chart.AddPointAnnotation(dataRow, columnName);
                    }
                    else
                    {
                        this.Chart.AddLineAnnotation(dataRow, columnName);
                    }
                }

                else if (vertexEvidence.Type == VertexEvidenceType.Range)
                {
                    this.Chart.AddRangeAnnotation(dataRow, columnName);
                }

                else if (vertexEvidence.Type == VertexEvidenceType.State)
                {
                    if (from == to)
                    {
                        this.Chart.AddPointAnnotation(dataRow, columnName);
                    }
                    else
                    {
                        this.Chart.AddLineAnnotation(dataRow,columnName);
                    }
                }

                else if (vertexEvidence.Type == VertexEvidenceType.Triangular)
                {
                    this.Chart.AddTriangularDistributionAnnotation(this.SelectedVertex, dataRow, columnName);
                }

                else if (vertexEvidence.Type != VertexEvidenceType.Null)
                {
                    this.Chart.AddInterpolatedDistributionAnnotation(this.SelectedVertex, dataRow, columnName);
                }
            }

            else if (vertexEvidence.Type != VertexEvidenceType.Null)
            {
                this.Chart.AddInterpolatedDistributionAnnotation(this.SelectedVertex, dataRow, columnName);
            }
        }
    }
}