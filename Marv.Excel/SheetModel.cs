using System.Collections.Generic;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv_Excel
{
    public class SheetModel
    {
        private List<string> columnHeaders = new List<string>();
        private int endYear;
        private Dictionary<int, string, IEvidence> modelEvidence = new Dictionary<int, string, IEvidence>();
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

        public Dictionary<int, string, IEvidence> ModelEvidence
        {
            get
            {
                return modelEvidence;
            }
            set
            {
                modelEvidence = value;
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