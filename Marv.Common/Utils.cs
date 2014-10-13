using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Marv
{
    public static class Utils
    {
        public const double Epsilon = 10E-06;

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

        public static T Max<T>(T a, T b) where T : IComparable
        {
            return a.CompareTo(b) > 0 ? a : b;
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