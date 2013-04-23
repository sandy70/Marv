namespace LibPipeline
{
    public class LocationBase : Location2DBase, ILocation
    {
        private double? elevation;

        public double? Elevation
        {
            get
            {
                return this.elevation;
            }

            set
            {
                if (value != this.elevation)
                {
                    this.elevation = value;
                    this.OnPropertyChanged("Elevation");
                }
            }
        }
    }
}