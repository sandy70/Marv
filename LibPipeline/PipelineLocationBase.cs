namespace LibPipeline
{
    public class PipelineLocationBase : LocationBase, IPipelineLocation
    {
        private double? distanceFromOrigin;

        public double? DistanceFromOrigin
        {
            get
            {
                return this.distanceFromOrigin;
            }

            set
            {
                if (value != this.distanceFromOrigin)
                {
                    this.distanceFromOrigin = value;
                    this.OnPropertyChanged("DistanceFromOrigin");
                }
            }
        }
    }
}