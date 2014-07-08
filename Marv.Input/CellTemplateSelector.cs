using System;
using System.Windows;
using System.Windows.Controls;
using Marv.Common;
using Marv.Common.Graph;
using Telerik.Windows.Controls.GridView;
using System.Linq;

namespace Marv.Input
{
    internal class CellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate VertexEvidenceStringTemplate { get; set; }
        public DataTemplate VertexEvidenceTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            try
            {
                var row = item as dynamic;

                var cell = container as GridViewCell;
                var year = (string) cell.Column.Header;

                var evidence = row[year];

                var gridViewCell = container as GridViewCell;

                if (evidence is VertexEvidence)
                {
                    gridViewCell.Tag = (evidence as VertexEvidence).Evidence.Select((val, i) => new ScatterPoint {XValue = i, YValue = val.Value}).ToList();
                    return this.VertexEvidenceTemplate;
                }
            }
            catch(Exception exp)
            {
                Console.WriteLine(exp);
            }

            return null;
        }
    }
}