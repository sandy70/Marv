namespace LibPipeline
{
    public interface IPipelineLocation : ILocation
    {
        double? DistanceFromOrigin { get; set; }
    }
}