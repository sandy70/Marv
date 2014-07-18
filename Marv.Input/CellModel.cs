using System;
using Marv.Common;
using Marv.Common.Graph;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public class CellModel
    {
        public VertexEvidence Data
        {
            get
            {
                return this.Row[this.YearString] as VertexEvidence;
            }

            set
            {
                this.Row[this.YearString] = value;
            }
        }

        public Dynamic Row { get; private set; }
        public string SectionId { get; private set; }
        public int Year { get; private set; }
        public string YearString { get; private set; }

        public CellModel(GridViewCell cell)
        {
            this.Row = cell.ParentRow.DataContext as Dynamic;
            this.SectionId = this.Row["Section ID"] as string;
            this.YearString = cell.Column.Header as string;
            this.Year = Convert.ToInt32(this.YearString);
        }
    }
}