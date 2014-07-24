using System;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Input
{
    public class LineInput : NotifyPropertyChanged
    {
        private Dictionary<string, int, string, VertexEvidence> evidence = new Dictionary<string,int,string,VertexEvidence>();
        private Guid graphGuid;

        public Dictionary<string, int, string, VertexEvidence> Evidence
        {
            get
            {
                return this.evidence;
            }

            set
            {
                if (value != this.evidence)
                {
                    this.evidence = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Guid GraphGuid
        {
            get
            {
                return this.graphGuid;
            }

            set
            {
                if (value != this.graphGuid)
                {
                    this.graphGuid = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}