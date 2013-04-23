namespace LibPipeline
{
    public interface ILocation : ILocation2D
    {
        double? Elevation { get; set; }
    }
}