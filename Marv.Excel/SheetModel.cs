using System.Collections.Generic;
using Marv.Common.Graph;

namespace Marv_Excel
{
    public class SheetModel
    {
        private List<string> columnHeaders;
        private int endYear;
        private Dictionary<string, object> sheetHeaders = new Dictionary<string, object>();
        private int startYear;
        private IEnumerable<Vertex> vertices;

        public List<string> ColumnHeaders
        {
            get
            {
                return columnHeaders;
            }
            set
            {
                columnHeaders = value;
            }
        }

        public int EndYear
        {
            get
            {
                return endYear;
            }
            set
            {
                endYear = value;
            }
        }

        public Dictionary<string, object> SheetHeaders
        {
            get
            {
                return sheetHeaders;
            }

            set
            {
                sheetHeaders = value;
            }
        }

        public int StartYear
        {
            get
            {
                return startYear;
            }
            set
            {
                startYear = value;
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                return vertices;
            }
            set
            {
                vertices = value;
            }
        }
    }
}