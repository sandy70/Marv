using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Marv.Common;
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
            var cell = container as GridViewCell;
            var cellModel = cell.ToModel();

            if (cellModel.IsColumnSectionId) return null;

            var evidence = cellModel.Data as VertexData;

            if (evidence == null || evidence.Evidence == null) return null;

            cell.Tag = evidence.Evidence.Select((val, i) => new ScatterPoint {XValue = i, YValue = val}).ToList();
            return this.VertexEvidenceTemplate;
        }
    }
}