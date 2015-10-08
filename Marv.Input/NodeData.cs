using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Marv.Common.Types;
using Newtonsoft.Json;

namespace Marv.Input
{
    public class NodeData
    {
        
        private Dict<string, InterpolationData> interpolatedNodeData;
        private EvidenceTable userTable;
        
        [JsonProperty]
        public Dict<string, InterpolationData> InterpolatedNodeData
        {
            get { return interpolatedNodeData; }
            set
            {
                interpolatedNodeData = value;
                this.RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public EvidenceTable UserTable
        {
            get { return userTable; }
            set
            {
                userTable = value;
                this.RaisePropertyChanged();
            }
        }

        public NodeData()
        {
            this.InterpolatedNodeData = new Dict<string, InterpolationData>();
            this.UserTable = new EvidenceTable();
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}