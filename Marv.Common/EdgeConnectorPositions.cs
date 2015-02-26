namespace Marv.Common
{
    public class EdgeConnectorPositions : NotifyPropertyChanged
    {
        private string sourcePosition = "Auto";
        private string targetPosition = "Auto";

        public string SourcePosition
        {
            get { return this.sourcePosition; }

            set
            {
                if (value != this.sourcePosition)
                {
                    this.sourcePosition = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string TargetPosition
        {
            get { return this.targetPosition; }

            set
            {
                if (value != this.targetPosition)
                {
                    this.targetPosition = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}