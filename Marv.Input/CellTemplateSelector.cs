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
        public DataTemplate SparkLineTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var theme = (Application.Current.MainWindow as MainWindow).SelectedTheme;

            if (theme == DataTheme.User)
            {
                return null;
            }
          
            var cell = container as GridViewCell;
            var evidenceRow = cell.ParentRow.Item as EvidenceRow;
            var column = cell.Column;

            
            try
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
                }
                else
                {
                    cell.Tag = cellValue;
                }

                DateTime dateTime;

                if (column.UniqueName.TryParse(out dateTime) )
                {
                    return this.SparkLineTemplate;
                }

                return null;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
    }
}