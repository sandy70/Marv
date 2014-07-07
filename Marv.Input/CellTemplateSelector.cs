using System;
using System.Windows;
using System.Windows.Controls;
using Marv.Common.Graph;
using Telerik.Windows.Controls.GridView;

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

                if (evidence is VertexEvidence)
                {
                    return this.VertexEvidenceTemplate;
                }

                if (evidence is VertexEvidenceString)
                {
                    return this.VertexEvidenceStringTemplate;
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