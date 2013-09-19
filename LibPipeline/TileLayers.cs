using MapControl;

namespace LibPipeline
{
    public static class TileLayers
    {
        public static TileLayer MapBoxSat = new TileLayer
        {
            SourceName = "MapBoxSat",
            Description = "MapBox Satellite",
            TileSource = new TileSource("http://a.tiles.mapbox.com/v3/vinodkhare.map-q9bgxflq/{z}/{x}/{y}.png")
        };

        public static TileLayer MapBoxTerrain = new TileLayer
        {
            SourceName = "MapBoxTerrain",
            Description = "MapBox Terrain",
            TileSource = new TileSource("http://a.tiles.mapbox.com/v3/vinodkhare.map-vu2qowlx/{z}/{x}/{y}.png")
        };

        public static TileLayer MapQuestMap = new TileLayer
        {
            SourceName = "MapQuestMap",
            Description = "MapQuest Map",
            TileSource = new TileSource("http://otile{n}.mqcdn.com/tiles/1.0.0/map/{z}/{x}/{y}.png")
        };

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