using System;
using Marv.Common;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Marv
{
    public class CellModel
    {
        public const string SectionIdHeader = "Section ID";

        private int year;

        public object Data
        {
            get { return this.Row[this.Header]; }

            set { this.Row[this.Header] = value; }
        }

        public string Header { get; private set; }
        public bool IsColumnSectionId { get; private set; }
        public Dynamic Row { get; private set; }
        public string SectionId { get; private set; }

        public int Year
        {
            get
            {
                if (this.year < 0)
                {
                    throw new InvalidValueException(String.Format("CellModel.Header [{0}] cannot be parsed to a valid Year.", this.Header));
                }

                return this.year;
            }

            private set { this.year = value; }
        }

        public CellModel(Dynamic row, string header)
        {
            this.Row = row;
            this.SectionId = this.Row[SectionIdHeader] as string;
            this.Header = header;

            int result;
            this.IsColumnSectionId = !Int32.TryParse(this.Header, out result);
            this.Year = !this.IsColumnSectionId ? result : int.MinValue;
        }

        public CellModel(GridViewCellInfo cellInfo) : this(cellInfo.Item as Dynamic, cellInfo.Column.Header as string) {}

        public CellModel(GridViewCell cell) : this(cell.ParentRow.DataContext as Dynamic, cell.Column.Header as string) {}
    }
}