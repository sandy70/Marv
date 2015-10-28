using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Marv.Common.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using QuickGraph.Algorithms.RankedShortestPath;
using SharpKml.Dom;
using Point = System.Windows.Point;

namespace Marv.Common
{
    public static partial class Extensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }
            if (val.CompareTo(max) > 0)
            {
                return max;
            }
            return val;
        }

        public static string Dequote(this string str, char startChar, char endChar)
        {
            var nChars = str.Count();
            var startIndex = 0;
            var length = nChars;

            if (nChars > 0)
            {
                if (str[0] == startChar)
                {
                    startIndex = 1;
                    length -= 1;
                }

                if (str[nChars - 1] == endChar)
                {
                    length -= 1;
                }
            }

            return str.Substring(startIndex, length);
        }

        public static string Dequote(this string str, char padChar)
        {
            return str.Dequote(padChar, padChar);
        }

        public static string Dequote(this string str)
        {
            return str.Dequote('"');
        }

        public static string Enquote(this string str, char startChar, char endChar)
        {
            return startChar + str + endChar;
        }

        public static string Enquote(this string str, char padChar)
        {
            return str.Enquote(padChar, padChar);
        }

        public static string Enquote(this string str)
        {
            return str.Enquote('"');
        }

        public static double Entropy(this IEnumerable<double> array)
        {
            var values = array as IList<double> ?? array.ToList();
            return -values.Where(value => value > 0).Sum(value => value * Math.Log(value)) / Math.Log(values.Count());
        }

        public static Dict<object, VertexEvidence> GetEvidence(this ILineData lineData, string sectionId, IEnumerable<int> years, string nodeKey)
        {
            var nodeEvidences = new Dict<object, VertexEvidence>();
            var sectionEvidence = lineData.GetEvidence(sectionId);

            if (years == null)
            {
                years = sectionEvidence.Keys;
            }

            foreach (var year in years)
            {
                nodeEvidences[year] = sectionEvidence[year][nodeKey];
            }

            return nodeEvidences;
        }

        public static Dict<object, VertexEvidence> GetEvidence(this ILineData lineData, IEnumerable<string> sectionIds, int year, string nodeKey)
        {
            var nodeEvidences = new Dict<object, VertexEvidence>();

            if (sectionIds == null)
            {
                sectionIds = lineData.SectionIds;
            }

            foreach (var sectionId in sectionIds)
            {
                var sectionEvidence = lineData.GetEvidence(sectionId);
                nodeEvidences[sectionId] = sectionEvidence[year][nodeKey];
            }

            return nodeEvidences;
        }

        public static Point GetOffset(this Rect viewport, Rect bounds, double pad = 0)
        {
            var point = new Point();

            var left = bounds.Left - viewport.Left;
            var right = bounds.Right - viewport.Right;
            var top = bounds.Top - viewport.Top;
            var bottom = bounds.Bottom - viewport.Bottom;

            if (left > 0 && right > 0)
            {
                point.X = right + pad;
            }
            else if (left < 0 && right < 0)
            {
                point.X = left - pad;
            }

            if (top < 0 && bottom < 0)
            {
                point.Y = top - pad;
            }
            else if (top > 0 && bottom > 0)
            {
                point.Y = bottom + pad;
            }

            return point;
        }

        public static IEnumerable<double> Normalized(this IEnumerable<double> values)
        {
            var valueList = values as IList<double> ?? values.ToList();
            var sum = valueList.Sum();

            for (var i = 0; i < valueList.Count; i++)
            {
                if (sum == 0)
                {
                    valueList[i] = 0;
                }
                else
                {
                    valueList[i] /= sum;
                }
            }

            return valueList;
        }

        public static object Parse(this string str)
        {
            double doubleValue;

            if (double.TryParse(str, out doubleValue))
            {
                return doubleValue;
            }

            return str;
        }

        public static T ParseJson<T>(this string _string)
        {
            return JsonConvert.DeserializeObject<T>(_string);
        }

        public static IEnumerable<Point> Reduce(this IEnumerable<Point> points, double tolerance = 10)
        {
            var pointList = points as IList<Point> ?? points.ToList();

            if (pointList.Count <= 2)
            {
                return pointList;
            }

            var first = pointList.First();
            var last = pointList.Last();

            var maxDistance = double.MinValue;
            var maxDistancePoint = first;

            foreach (var point in pointList)
            {
                var distance = Utils.Distance(first, last, point);

                if (distance < maxDistance)
                {
                    continue;
                }

                maxDistance = distance;
                maxDistancePoint = point;
            }

            if (maxDistance > tolerance)
            {
                return pointList.TakeUntil(x => x == maxDistancePoint)
                                .Reduce(tolerance)
                                .Concat(pointList.SkipWhile(x => x != maxDistancePoint)
                                                 .Reduce(tolerance)
                                                 .Skip(1));
            }
            return pointList.Take(1)
                            .Concat(last);
        }

        public static void SetEvidence(this ILineData lineData, string sectionId, int year, string vertexKey, VertexEvidence vertexEvidence)
        {
            var sectionEvidence = lineData.GetEvidence(sectionId);

            if (vertexEvidence.Type == VertexEvidenceType.Null)
            {
                sectionEvidence[year][vertexKey] = null;
            }
            else
            {
                sectionEvidence[year][vertexKey] = vertexEvidence;
            }

            lineData.SetEvidence(sectionId, sectionEvidence);
        }

        public static string String<T>(this IEnumerable<T> items, string format = "{0}", string delim = ",")
        {
            var str = "";

            var itemList = items as IList<T> ?? items.ToList();

            if (itemList.Count == 0)
            {
                return str;
            }

            if (itemList.Count == 1)
            {
                str += System.String.Format(format, itemList.First());
                return str;
            }

            str = itemList.AllButLast().Aggregate(str, (current, item) => current + System.String.Format(format, item) + delim);

            str += System.String.Format(format, itemList.Last());

            return str;
        }

        public static string ToJson(this object _object, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(_object, formatting);
        }

        public static LocationCollection ToLocationCollection(this IEnumerable<Location> locations)
        {
            var locationCollection = new LocationCollection();

            foreach (var location in locations)
            {
                locationCollection.Add(location);
            }

            return locationCollection;
        }

        public static LocationCollection ToLocationCollection(this LineString lineString, bool assignIds = false)
        {
            var key = 0;
            var locationCollection = new LocationCollection();

            foreach (var coordinate in lineString.Coordinates)
            {
                locationCollection.Add(new Location
                {
                    Key = assignIds ? key++ + "" : null,
                    Latitude = coordinate.Latitude,
                    Longitude = coordinate.Longitude
                });
            }

            return locationCollection;
        }

        public static IEnumerable<string> Trimmed(this IEnumerable<string> untrimmed)
        {
            return untrimmed.Select(x => x.Trim());
        }

        public static void WriteBson(this object _object, string fileName, Formatting formatting = Formatting.Indented)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = formatting,
                TypeNameHandling = TypeNameHandling.Auto
            };

            using (var jsonTextWriter = new BsonWriter(File.Open(fileName, FileMode.Create)))
            {
                serializer.Serialize(jsonTextWriter, _object);
            }
        }

        public static void WriteHcs(this Dict<string, VertexEvidence> evidences, string filePath)
        {
            using (var hcsFile = new StreamWriter(filePath))
            {
                foreach (var nodeKey in evidences.Keys)
                {
                    var vertexEvidence = evidences[nodeKey];

                    if (vertexEvidence.Type == VertexEvidenceType.Null)
                    {
                        continue;
                    }

                    if (vertexEvidence.Value.Sum() > 0)
                    {
                        hcsFile.WriteLine("{0}: ({1})", nodeKey, evidences[nodeKey].Value.String(delim: " "));
                    }
                }
            }
        }

        public static void WriteHcs(this Dict<string, double[]> evidences, string filePath)
        {
            using (var hcsFile = new StreamWriter(filePath))
            {
                foreach (var nodeKey in evidences.Keys)
                {
                    if (evidences[nodeKey].Sum() > 0)
                    {
                        hcsFile.WriteLine("{0}: ({1})", nodeKey, evidences[nodeKey].String(delim: " "));
                    }
                }
            }
        }

        public static void WriteJson(this object _object, string fileName, Formatting formatting = Formatting.Indented)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = formatting,
                TypeNameHandling = TypeNameHandling.Auto
            };

            using (var streamWriter = new StreamWriter(fileName))
            {
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(jsonTextWriter, _object);
                }
            }
        }
    }
}