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
        public DataTemplate Template { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cell = container as GridViewCell;

            try
            {
                var cellModel = cell.ToModel();

                if (cellModel.IsColumnSectionId)
                {
                    return null;
                }

                var evidence = cellModel.Data as NodeEvidence;

                if (evidence == null || evidence.Value == null)
                {
                    return null;
                }

                cell.Tag = evidence.Value.Select((y, i) => new ScatterDataPoint
                {
                    XValue = i,
                    YValue = y
                });

                return this.Template;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
    }
}