using MapControl;
using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace LibPipeline
{
    public class EarthquakeService
    {
        public static string GetEarthquakeColor(double magnitude)
        {
            if (magnitude < 5.0)
            {
                return "Yellow";
            }
            else if (magnitude < 7.0)
            {
                return "Orange";
            }
            else
            {
                return "Red";
            }
        }

        public static void GetRecentEarthquakes(EventHandler<EarthquakeEventArgs> callback)
        {
            WebClient client = new WebClient();

            client.OpenReadCompleted += (o, e) =>
            {
                XDocument doc = XDocument.Load(e.Result);

                var locations = doc.Element("rss")
                           .Element("channel")
                           .Elements("item")
                           .Select((eq) =>
                                {
                                    Location location = new Location();
                                    location.Latitude = Convert.ToDouble(eq.Element(XName.Get("lat", "http://www.w3.org/2003/01/geo/wgs84_pos#")).Value);
                                    location.Longitude = Convert.ToDouble(eq.Element(XName.Get("long", "http://www.w3.org/2003/01/geo/wgs84_pos#")).Value);
                                    (location as dynamic).Magnitude = Convert.ToDouble(eq.Element("title").Value.Substring(2, 3));
                                    (location as dynamic).Radius = (location as dynamic).Magnitude * 10;
                                    (location as dynamic).Title = eq.Element("title").Value;
                                    (location as dynamic).Description = eq.Element("description").Value;
                                    return location;
                                });

                //var multiLocation = new MultiLocation
                //    {
                //        Locations = locations
                //    };

                //callback(null, new EarthquakeEventArgs(multiLocation));
            };

            client.OpenReadAsync(new Uri("http://earthquake.usgs.gov/eqcenter/recenteqsww/catalogs/eqs7day-M2.5.xml"));
        }

        public class EarthquakeEventArgs : EventArgs
        {
            //public EarthquakeEventArgs(MultiLocation multiLocation)
            //{
            //    MultiLocation = multiLocation;
            //}

            //public MultiLocation MultiLocation { get; set; }
        }
    }
}