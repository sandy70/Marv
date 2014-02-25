using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Marv.Common
{
    public static class Utils
    {
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

        public static Color DoubleToColor(double value)
        {
            var fourValue = 4 * value;
            var red = Math.Min(fourValue - 1.5, -fourValue + 4.5);
            var green = Math.Min(fourValue - 0.5, -fourValue + 3.5);
            var blue = Math.Min(fourValue + 0.5, -fourValue + 2.5);

            return Color.FromScRgb(1, (float)red.Clamp(0, 1), (float)green.Clamp(0, 1), (float)blue.Clamp(0, 1));
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
            if (str.Trim().ToLower() == "-infinity") return double.NegativeInfinity;
            return double.Parse(str);
        }
    }
}