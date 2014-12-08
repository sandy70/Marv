using MapControl;

namespace Marv.Controls.Map
{
    public static class TileLayers
    {
        public static TileLayer BingMapsAerial = new TileLayer
        {
            SourceName = "BingMapsAerial",
            Description = "Bing Maps Aerial",
            TileSource = new TileSource("http://ecn.t{i}.tiles.virtualearth.net/tiles/h{q}.png?g=0&amp;stl=h")
        };

        public static TileLayer BingMapsRoads = new TileLayer
        {
            SourceName = "BingMapsRoads",
            Description = "Bing Maps Roads",
            TileSource = new TileSource("http://ecn.t{i}.tiles.virtualearth.net/tiles/r{q}.png?g=0&amp;stl=h")
        };

        public static readonly TileLayer MapBoxAerial = new TileLayer
        {
            SourceName = "MapBoxAerial",
            Description = "MapBox Aerial",
            TileSource = new TileSource("http://a.tiles.mapbox.com/v3/vinodkhare.map-q9bgxflq/{z}/{x}/{y}.png")
        };

        public static TileLayer MapBoxRoads = new TileLayer
        {
            SourceName = "MapBoxRoads",
            Description = "MapBox Roads",
            TileSource = new TileSource("http://a.tiles.mapbox.com/v3/vinodkhare.map-chgilhh7/{z}/{x}/{y}.png")
        };

        public static TileLayer MapBoxTerrain = new TileLayer
        {
            SourceName = "MapBoxTerrain",
            Description = "MapBox Terrain",
            TileSource = new TileSource("http://a.tiles.mapbox.com/v3/vinodkhare.map-vu2qowlx/{z}/{x}/{y}.png")
        };

        public static TileLayer MapQuestAerial = new TileLayer
        {
            SourceName = "MapQuestAerial",
            Description = "MapQuest Aerial",
            TileSource = new TileSource("http://otile{n}.mqcdn.com/tiles/1.0.0/sat/{z}/{x}/{y}.png"),
        };

        public static TileLayer MapQuestRoads = new TileLayer
        {
            SourceName = "MapQuestRoads",
            Description = "MapQuest Roads",
            TileSource = new TileSource("http://otile{n}.mqcdn.com/tiles/1.0.0/map/{z}/{x}/{y}.png")
        };

        public static TileLayer OsmRoads = new TileLayer
        {
            SourceName = "OsmRoads",
            Description = "OpenStreetMap Roads",
            TileSource = new TileSource("http://{c}.tile.openstreetmap.org/{z}/{x}/{y}.png")
        };
    }
}