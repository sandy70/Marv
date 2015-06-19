using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Marv.Common;
using Telerik.Charting;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public class CellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate InValidTemplate { get; set; }
        public DataTemplate SparkLineTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cell = container as GridViewCell;
            var evidenceRow = cell.ParentRow.Item as EvidenceRow;
            var column = cell.Column;
            
            try
            {
                if (column.UniqueName == "From")
                {
                    cell.Tag = evidenceRow.From;
                }

                else if (column.UniqueName == "To")
                {
                    cell.Tag = evidenceRow.To;
                }

                else
                {
                    var cellValue = evidenceRow[column.UniqueName];

                    if (cellValue == null)
                    {
                        return null;
                    }

                    if (cellValue is VertexEvidence)
                    {
                        var vertexEvidence = cellValue as VertexEvidence; 

                        if (vertexEvidence.Value == null)
                        {
                            return null;
                        }

                        cell.Tag = vertexEvidence.Value.Select((y, i) => new ScatterDataPoint
                        {
                            XValue = i,
                            YValue = y
                        });
                    }
                    else if (cellValue is double[])
                    {
                        cell.Tag = (cellValue as double[]).Select((y, i) => new ScatterDataPoint
                        {
                            XValue = i,
                            YValue = y
                        });
                        evidenceRow.IsActive = false;
                    }

                    return this.SparkLineTemplate;
                }

                if (evidenceRow.IsValid)
                {
                    return null;
                }

                return this.InValidTemplate;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
    }
}