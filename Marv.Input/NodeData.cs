using System.ComponentModel;
using System.Runtime.CompilerServices;
using Marv.Common.Types;

namespace Marv.Input
{
    internal class NodeData
    {
        private Dict<string, InterpolationData> nodeInterpolatedData;
        private EvidenceTable userTable;

        public Dict<string, InterpolationData> NodeInterpolatedData
        {
            get { return nodeInterpolatedData; }
            set
            {
                nodeInterpolatedData = value;
                this.RaisePropertyChanged();
            }
        }

        public EvidenceTable Table
        {
            get { return userTable; }
            set
            {
                userTable = value;
                this.RaisePropertyChanged();
            }
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