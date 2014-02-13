using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace Marv.Common
{
    public static class Utils
    {
        public static double Distance(Location l1, Location l2)
        {
            double distance = 0;

            double dLat = (l2.Latitude - l1.Latitude) / 180 * Math.PI;
            double dLong = (l2.Longitude - l1.Longitude) / 180 * Math.PI;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                        + Math.Cos(l1.Latitude / 180 * Math.PI) * Math.Cos(l2.Latitude / 180 * Math.PI) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            double nr = Math.Pow(radiusE * radiusP * Math.Cos(l1.Latitude / 180 * Math.PI), 2);

            //Denominator part of the function
            double dr = Math.Pow(radiusE * Math.Cos(l1.Latitude / 180 * Math.PI), 2)
                            + Math.Pow(radiusP * Math.Sin(l1.Latitude / 180 * Math.PI), 2);

            double radius = Math.Sqrt(nr / dr);

            //Calaculate distance in metres.
            distance = radius * c;
            return distance;
        }

        public static double Distance(Point p1, Point p2, Point p)
        {
            var area = Math.Abs(.5 * (p1.X * p2.Y + p2.X * p.Y + p.X * p1.Y - p2.X * p1.Y - p.X * p2.Y - p1.X * p.Y));

            var bottom = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

            var height = 0.0;

            if (bottom == 0.0)
            {
                height = Math.Sqrt(Math.Pow(p1.X - p.X, 2) + Math.Pow(p1.Y - p.Y, 2));
            }
            else
            {
                height = area / bottom * 2;
            }

            return height;
        }

        public static Color DoubleToColor(double value)
        {
            double fourValue = 4 * value;
            double red = Math.Min(fourValue - 1.5, -fourValue + 4.5);
            double green = Math.Min(fourValue - 0.5, -fourValue + 3.5);
            double blue = Math.Min(fourValue + 0.5, -fourValue + 2.5);

            return Color.FromScRgb(1, (float)red.Clamp(0, 1), (float)green.Clamp(0, 1), (float)blue.Clamp(0, 1));
        }

        public static Location Mid(Location l1, Location l2)
        {
            if (l1 == null || l2 == null)
            {
                return null;
            }
            else
            {
                // This is technically not correct but should be okay for small distances
                return new Location
                {
                    Latitude = (l1.Latitude + l2.Latitude) / 2,
                    Longitude = (l1.Longitude + l2.Longitude) / 2,
                };
            }
        }

        public async static Task<IEnumerable<Location>> ReadEarthquakesAsync(IProgress<double> progress)
        {
            var webClient = new WebClient();

            var stream = await webClient.OpenReadTaskAsync(new Uri("http://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_week.atom"));

            var xDocument = XDocument.Load(stream);

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

        public static T Clamp<T>(T value, T minValue, T maxValue) where T : IComparable<T>
        {
            if (value.CompareTo(minValue) < 0) value = minValue;
            if (value.CompareTo(maxValue) > 0) value = maxValue;

            return value;
        }

        public static T ReadJson<T>(string fileName)
        {
            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (var streamWriter = new StreamReader(fileName))
            {
                using (var jsonTextWriter = new JsonTextReader(streamWriter))
                {
                    return serializer.Deserialize<T>(jsonTextWriter);
                }
            }
        }

        public static Guid ToGuid(this long n)
        {
            var guidBinary = new byte[16];

            Array.Copy(Guid.NewGuid().ToByteArray(), 0, guidBinary, 0, 8);
            Array.Copy(BitConverter.GetBytes(n), 0, guidBinary, 8, 8);

            return new Guid(guidBinary);
        }

        public static long ToInt64(this Guid guid)
        {
            return BitConverter.ToInt64(guid.ToByteArray(), 8);
        }

        public static double ParseDouble(this string str)
        {
            if (str.Trim().ToLower() == "infinity") return double.PositiveInfinity;
            else if (str.Trim().ToLower() == "-infinity") return double.NegativeInfinity;
            else return double.Parse(str);
        }
    }
}