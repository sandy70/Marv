using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Marv.Graph;
using Telerik.Charting;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public class CellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Template { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cell = container as GridViewCell;

            var cellModel = cell.ToModel();

            if (cellModel.IsColumnSectionId)
            {
                return null;
            }

            var evidence = cellModel.Data as VertexData;

            if (evidence == null || evidence.Evidence == null)
            {
                return null;
            }

            cell.Tag = evidence.Evidence.Select((y, i) => new ScatterDataPoint
            {
                XValue = i,
                YValue = y
            });

            return this.Template;
        }
    }
}