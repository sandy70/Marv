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
                var window = Application.Current.MainWindow as MainWindow;
                var lineEvidence = window.LineEvidence;
                var selectedVertex = window.SelectedVertex;

                var row = item as dynamic;
                var sectionId = row["Section ID"] as string;

                var cell = container as GridViewCell;
                var year = Convert.ToInt32((string) cell.Column.Header);

                var evidence = lineEvidence[sectionId][year][selectedVertex.Key];

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