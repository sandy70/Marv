using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace Marv.Common
{
    public static class Utils
    {
        public const double Epsilon = 10E-03;

        public static T Clamp<T>(T value, T minValue, T maxValue) where T : IComparable<T>
        {
            if (value.CompareTo(minValue) < 0)
            {
                value = minValue;
            }
            if (value.CompareTo(maxValue) > 0)
            {
                value = maxValue;
            }

            return value;
        }

        public static T Create<T>()
        {
            var constructor = (typeof (T)).GetConstructor(Type.EmptyTypes);

            if (ReferenceEquals(constructor, null))
            {
                //there is no default constructor
                return default(T);
            }
            //there is a default constructor
            //you can invoke it like so:
            return (T) constructor.Invoke(new object[0]);
            //return constructor.Invoke(new object[0]) as T; //If T is class
        }

        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public static double Distance(Point p1, Point p2, Point p)
        {
            var area = Math.Abs(.5 * (p1.X * p2.Y + p2.X * p.Y + p.X * p1.Y - p2.X * p1.Y - p.X * p2.Y - p1.X * p.Y));

            var bottom = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

            double height;

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

        public static Point? Intersection(LineSegment line1, LineSegment line2)
        {
            var a1 = line1.P2.Y - line1.P1.Y; // y2 - y1
            var b1 = -(line1.P2.X - line1.P1.X); // -(x2 - x1)
            var c1 = line1.P1.Y * (line1.P2.X - line1.P1.X) - line1.P1.X * (line1.P2.Y - line1.P1.Y);

            var a2 = line2.P2.Y - line2.P1.Y; // y2 - y1
            var b2 = -(line2.P2.X - line2.P1.X); // -(x2 - x1)
            var c2 = line2.P1.Y * (line2.P2.X - line2.P1.X) - line2.P1.X * (line2.P2.Y - line2.P1.Y);

            var delta = a1 * b2 - a2 * b1;

            if (Math.Abs(delta) < 0.0001)
            {
                return null;
            }

            var intersection = new Point
            {
                X = (b2 * c1 - b1 * c2) / delta,
                Y = (a1 * c2 - a2 * c1) / delta
            };

            var distance1 = Distance(line1.P1, line1.P2, intersection);
            var distance2 = Distance(line2.P1, line2.P2, intersection);

            if (distance1 > Epsilon || distance2 > Epsilon)
            {
                return null;
            }

            return intersection;
        }

        public static T Max<T>(T a, T b) where T : IComparable
        {
            return a.CompareTo(b) > 0 ? a : b;
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

        public static T Min<T>(T a, T b) where T : IComparable
        {
            return a.CompareTo(b) < 0 ? a : b;
        }

        public static double ParseDouble(this string str)
        {
            if (str.Trim().ToLower() == "infinity")
            {
                return double.PositiveInfinity;
            }

            if (str.Trim().ToLower() == "-infinity")
            {
                return double.NegativeInfinity;
            }

            return double.Parse(str);
        }

        public static T ReadJson<T>(string fileName)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects
            };

            using (var streamWriter = new StreamReader(fileName))
            {
                using (var jsonTextReader = new JsonTextReader(streamWriter))
                {
                    jsonTextReader.FloatParseHandling = FloatParseHandling.Double;
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public static object ReadJson(string fileName)
        {
            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (var streamWriter = new StreamReader(fileName))
            {
                using (var jsonTextWriter = new JsonTextReader(streamWriter))
                {
                    return serializer.Deserialize(jsonTextWriter);
                }
            }
        }

        public static void Schedule(TimeSpan timeSpan, Action action)
        {
            var timer = new DispatcherTimer
            {
                Interval = timeSpan
            };

            timer.Tick += (timerSender, timerEvent) =>
            {
                timer.Stop();
                action();
            };

            timer.Start();
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
    }
}