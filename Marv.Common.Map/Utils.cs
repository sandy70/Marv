using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Marv.Common.Map
{
    public static class Utils
    {
        public static double Distance(Location l1, Location l2)
        {
            var dLat = (l2.Latitude - l1.Latitude) / 180 * Math.PI;
            var dLong = (l2.Longitude - l1.Longitude) / 180 * Math.PI;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                        + Math.Cos(l1.Latitude / 180 * Math.PI) * Math.Cos(l2.Latitude / 180 * Math.PI) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            var nr = Math.Pow(radiusE * radiusP * Math.Cos(l1.Latitude / 180 * Math.PI), 2);

            //Denominator part of the function
            var dr = Math.Pow(radiusE * Math.Cos(l1.Latitude / 180 * Math.PI), 2)
                            + Math.Pow(radiusP * Math.Sin(l1.Latitude / 180 * Math.PI), 2);

            var radius = Math.Sqrt(nr / dr);

            //Calaculate distance in metres.
            return radius * c;
        }

        public static Location Mid(Location l1, Location l2)
        {
            if (l1 == null || l2 == null)
            {
                return null;
            }
            // This is technically not correct but should be okay for small distances
            return new Location
            {
                Latitude = (l1.Latitude + l2.Latitude) / 2,
                Longitude = (l1.Longitude + l2.Longitude) / 2,
            };
        }

        public async static Task<IEnumerable<Location>> ReadEarthquakesAsync(IProgress<double> progress)
        {
            var webClient = new WebClient();

            var stream = await webClient.OpenReadTaskAsync(new Uri("http://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_week.atom"));

            var xDocument = XDocument.Load(stream);

            if (xDocument.Root != null)
            {
                return xDocument.Root.Elements("{http://www.w3.org/2005/Atom}entry")
                    .Select(entry =>
                    {
                        var location = Location.Parse(entry.Element("{http://www.georss.org/georss}point").Value);

                        location.Value = double.Parse(entry.Element("{http://www.w3.org/2005/Atom}title").Value.Substring(2, 3));

                        // location["Date"] = entry.Element("{http://www.w3.org/2005/Atom}updated").Value;
                        // location["Title"] = entry.Element("{http://www.w3.org/2005/Atom}title").Value;

                        return location;
                    });
            }

            return null;
        }
    }
}
