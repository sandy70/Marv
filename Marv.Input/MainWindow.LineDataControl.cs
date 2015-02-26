using System;
using Marv.Common;
using Marv.Common.Types;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private bool isAxisAutoSwitched;
        private string lastSectionId;
        private int lastYear;
        private HorizontalAxisQuantity previousAxisQuantity;

        private void LineDataControl_CellContentChanged(object sender, CellChangedEventArgs e)
        {
            if (e.CellModel.IsColumnSectionId)
            {
                if (e.OldString == null)
                {
                    this.LineData.SetEvidence(e.NewString, new Dict<int, string, VertexEvidence>());
                }
                else
                {
                    this.LineData.ChangeSectionId(e.OldString, e.NewString);
                }

                e.CellModel.Data = e.NewString;
            }
            else
            {
                var vertexEvidence = e.VertexEvidence ?? this.Graph.SelectedVertex.States.ParseEvidenceString(e.NewString);

                e.CellModel.Data = vertexEvidence;

                var sectionId = e.CellModel.SectionId;
                var year = e.CellModel.Year;

                this.LineData.SetEvidence(sectionId, year, this.Graph.SelectedVertex.Key, vertexEvidence);
                this.LineDataChart.SetUserEvidence(this.GetChartCategory(sectionId, year), vertexEvidence);
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

        private void LineDataControl_RowAdded(object sender, string sectionId)
        {
            this.LineData.SetEvidence(sectionId, new Dict<int, string, VertexEvidence>());
        }

        private void LineDataControl_RowRemoved(object sender, string sectionId)
        {
            this.LineData.RemoveSection(sectionId);
        }

        private void LineDataControl_RowSelected(object sender, string sectionId)
        {
            this.isAxisAutoSwitched = true;
            this.previousAxisQuantity = this.HorizontalAxisQuantity;

            this.HorizontalAxisQuantity = HorizontalAxisQuantity.Year;

            this.LineDataChart.SetUserEvidence(this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section
                                                   ? this.LineData.GetEvidence(null, this.SelectedYear, this.Graph.SelectedVertex.Key)
                                                   : this.LineData.GetEvidence(this.SelectedSectionId, null, this.Graph.SelectedVertex.Key));
        }

        private void LineDataControl_SectionIdPasting(object sender, GridViewCellClipboardEventArgs e)
        {
            if (this.LineData.SectionIds.Contains(e.Value as string))
            {
                this.Notifications.Add(new Notification
                {
                    Description = "Cannot paste because line data already contains section " + e.Value,
                    Duration = TimeSpan.FromSeconds(15),
                    IsTimed = true,
                    IsWarning = true
                });

                e.Cancel = true;
            }
        }

        private void LineDataControl_SectionIdValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            var cellModel = e.Cell.ToModel();

            if (this.LineData.SectionIds.Contains(cellModel.SectionId))
            {
                e.IsValid = false;
                e.ErrorMessage = "The line data already contains section " + cellModel.SectionId;
            }
        }

        private void LineDataControl_SelectedCellChanged(object sender, EventArgs e)
        {
            if (this.isAxisAutoSwitched)
            {
                this.isAxisAutoSwitched = false;
                this.HorizontalAxisQuantity = this.previousAxisQuantity;
            }

            this.Graph.Belief = this.LineData.GetBelief(this.SelectedSectionId)[this.SelectedYear];
            this.Graph.Evidence = this.LineData.GetEvidence(this.SelectedSectionId)[this.SelectedYear];

            var isSectionChanged = this.SelectedSectionId != this.lastSectionId;
            var isYearChanged = this.SelectedYear != this.lastYear;

            if ((isSectionChanged && this.HorizontalAxisQuantity == HorizontalAxisQuantity.Year) ||
                (isYearChanged && this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section))
            {
                this.LineDataChart.SetUserEvidence(this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section
                                                       ? this.LineData.GetEvidence(null, this.SelectedYear, this.Graph.SelectedVertex.Key)
                                                       : this.LineData.GetEvidence(this.SelectedSectionId, null, this.Graph.SelectedVertex.Key));
                this.UpdateChartTitle();
            }

            this.lastSectionId = this.SelectedSectionId;
            this.lastYear = this.SelectedYear;
        }
    }
}