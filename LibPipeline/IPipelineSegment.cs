using Microsoft.Maps.MapControl.WPF;

namespace LibPipeline
{
    public interface IPipelineSegment
    {
        ILocation EndLocation { get; set; }

        LocationCollection Locations { get; }

        ILocation StartLocation { get; set; }
    }
}