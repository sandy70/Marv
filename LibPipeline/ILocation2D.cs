using System.ComponentModel;

namespace LibPipeline
{
    public interface ILocation2D : INotifyPropertyChanged
    {
        double? Latitude { get; set; }

        double? Longitude { get; set; }
    }
}