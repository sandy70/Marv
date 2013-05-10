using MapControl;

namespace LibPipeline
{
    public static class TileLayers
    {
        public static TileLayer MapQuestSat = new TileLayer
        {
            SourceName = "MapQuestSat",
            Description = "MapQuest Satellite",
            TileSource = new TileSource("http://otile{n}.mqcdn.com/tiles/1.0.0/sat/{z}/{x}/{y}.png")
        };

        public static TileLayer OsmMap = new TileLayer
        {
            SourceName = "OsmMap",
            Description = "OpenStreetMap Map",
            TileSource = new TileSource("http://{c}.tile.openstreetmap.org/{z}/{x}/{y}.png")
        };
    }
}