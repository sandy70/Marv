using System;
using System.Windows;
using Marv.Common;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
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
                    this.LineData.ReplaceSectionId(e.OldString, e.NewString);
                }

                e.CellModel.Data = e.NewString;
            }
            else
            {
                var vertexEvidence = e.VertexEvidence ?? this.Graph.SelectedVertex.States.ParseEvidenceString(e.NewString);

                e.CellModel.Data = vertexEvidence;

                this.LineData.SetEvidence(e.CellModel.SectionId, e.CellModel.Year, this.Graph.SelectedVertex.Key, vertexEvidence);

                this.LineDataChart.SetUserEvidence(this.GetChartCategory(), vertexEvidence);
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

        private void LineDataControl_RowSelected(object sender, CellModel e)
        {
            this.HorizontalAxisQuantity = HorizontalAxisQuantity.Years;

            this.LineDataChart.SetUserEvidence(this.GetChartEvidence());
        }

        private void LineDataControl_SelectedCellChanged(object sender, EventArgs e)
        {
            this.Graph.Belief = this.LineData.GetBelief(this.SelectedSectionId)[this.SelectedYear];
            this.Graph.SetEvidence(this.LineData.GetEvidence(this.SelectedSectionId)[this.SelectedYear]);

            this.LineDataChart.SetUserEvidence(this.GetChartEvidence());
        }

        private void MainWindow_Loaded_LineDataControl(object sender, RoutedEventArgs e)
        {
            this.LineDataControl.CellContentChanged -= LineDataControl_CellContentChanged;
            this.LineDataControl.CellContentChanged += LineDataControl_CellContentChanged;

            this.LineDataControl.CellValidating -= LineDataControl_CellValidating;
            this.LineDataControl.CellValidating += LineDataControl_CellValidating;

            this.LineDataControl.NotificationClosed -= LineDataControl_NotificationClosed;
            this.LineDataControl.NotificationClosed += LineDataControl_NotificationClosed;

            this.LineDataControl.NotificationOpened -= LineDataControl_NotificationOpened;
            this.LineDataControl.NotificationOpened += LineDataControl_NotificationOpened;

            this.LineDataControl.RowAdded -= LineDataControl_RowAdded;
            this.LineDataControl.RowAdded += LineDataControl_RowAdded;

            this.LineDataControl.RowSelected -= LineDataControl_RowSelected;
            this.LineDataControl.RowSelected += LineDataControl_RowSelected;

            this.LineDataControl.RowRemoved -= LineDataControl_RowRemoved;
            this.LineDataControl.RowRemoved += LineDataControl_RowRemoved;

            this.LineDataControl.SelectedCellChanged -= LineDataControl_SelectedCellChanged;
            this.LineDataControl.SelectedCellChanged += LineDataControl_SelectedCellChanged;
        }
    }
}