using Smile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace LibBn
{
    public static class Extensions
    {
        public static IEnumerable<T> AllButLast<T>(this IEnumerable<T> source)
        {
            var it = source.GetEnumerator();
            bool hasRemainingItems = false;
            bool isFirst = true;
            T item = default(T);

            do
            {
                hasRemainingItems = it.MoveNext();
                if (hasRemainingItems)
                {
                    if (!isFirst) yield return item;
                    item = it.Current;
                    isFirst = false;
                }
            } while (hasRemainingItems);
        }

        public static IEnumerable<T> AllButLastN<T>(this IEnumerable<T> source, int n)
        {
            var it = source.GetEnumerator();
            bool hasRemainingItems = false;
            var cache = new Queue<T>(n + 1);

            do
            {
                if (hasRemainingItems = it.MoveNext())
                {
                    cache.Enqueue(it.Current);
                    if (cache.Count > n)
                        yield return cache.Dequeue();
                }
            } while (hasRemainingItems);
        }

        public static VertexEvidence GetVertexEvidence(this IEnumerable<VertexEvidence> graphEvidence, string vertexKey)
        {
            return graphEvidence.SingleOrDefault(x => x.Key.Equals(vertexKey));
        }

        public static ObservableCollection<State> ParseStates(this Network network, string key)
        {
            var states = new ObservableCollection<State>();

            var nodeHandle = network.GetNodeHandle(key);

            int nStates = network.GetOutcomeCount(nodeHandle);
            string[] names = new string[nStates];
            double[] values = network.GetNodeValue(nodeHandle);

            if (key.Equals("cdc"))
            {
                var prop = network.GetNodeType(nodeHandle);
            }

            foreach (var prop in network.GetNodeUserProperties(nodeHandle))
            {
                if (prop.name.Contains("HR_State_"))
                {
                    int stateIndex;
                    int.TryParse(prop.name.Substring(9), out stateIndex);

                    if (stateIndex < nStates)
                    {
                        names[stateIndex] = prop.value;
                    }
                }
            }

            for (int s = 0; s < nStates; s++)
            {
                states.Add(new State
                {
                    Key = names[s],
                    Value = values[s]
                });
            }

            return states;
        }

        public static string String(this IEnumerable<string> strings)
        {
            var str = "";

            if (strings.Count() == 0)
            {
                return str;
            }
            else if (strings.Count() == 1)
            {
                str += strings.First();
                return str;
            }

            foreach (var s in strings.AllButLast())
            {
                str += s + ",";
            }

            str += strings.Last();

            return str;
        }

        public static string String(this Dictionary<string, Point> positionsByGroup)
        {
            var str = "";

            if (positionsByGroup.Count == 0)
            {
                return str;
            }
            else if (positionsByGroup.Count == 1)
            {
                var kvpFirst = positionsByGroup.First();
                str += kvpFirst.Key + "," + kvpFirst.Value.X + "," + kvpFirst.Value.Y;
                return str;
            }

            foreach (var kvp in positionsByGroup.AllButLast())
            {
                str += kvp.Key + "," + kvp.Value.X + "," + kvp.Value.Y + ",";
            }

            var kvpLast = positionsByGroup.Last();
            str += kvpLast.Key + "," + kvpLast.Value.X + "," + kvpLast.Value.Y;

            return str;
        }

        public static IEnumerable<string> Trimmed(this IEnumerable<string> untrimmed)
        {
            return untrimmed.Select(x => x.Trim());
        }

        private static T DeepClone<T>(this T original, Dictionary<Object, Object> copies, params Object[] args)
        {
            T result;
            Type t = original.GetType();

            Object tmpResult;

            // Check if the object already has been copied
            if (copies.TryGetValue(original, out tmpResult))
            {
                return (T)tmpResult;
            }
            else
            {
                if (!t.IsArray)
                {
                    /* Create new instance, at this point you pass parameters to
                        * the constructor if the constructor if there is no default constructor
                        * or you change it to Activator.CreateInstance<T>() if there is always
                        * a default constructor */
                    result = (T)Activator.CreateInstance(t, args);
                    copies.Add(original, result);

                    // Maybe you need here some more BindingFlags
                    foreach (FieldInfo field in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance))
                    {
                        /* You can filter the fields here ( look for attributes and avoid
                            * unwanted fields ) */

                        Object fieldValue = field.GetValue(original);

                        // Check here if the instance should be cloned
                        Type ft = field.FieldType;

                        /* You can check here for ft.GetCustomAttributes(typeof(SerializableAttribute), false).Length != 0 to
                            * avoid types which do not support serialization ( e.g. NetworkStreams ) */
                        if (fieldValue != null && !ft.IsValueType && ft != typeof(String))
                        {
                            fieldValue = fieldValue.DeepClone(copies);
                            /* Does not support parameters for subobjects nativly, but you can provide them when using
                                * a delegate to create the objects instead of the Activator. Delegates should not work here
                                * they need some more love */
                        }

                        field.SetValue(result, fieldValue);
                    }
                }
                else
                {
                    // Handle arrays here
                    Array originalArray = (Array)(Object)original;
                    Array resultArray = (Array)originalArray.Clone();
                    copies.Add(original, resultArray);

                    // If the type is not a value type we need to copy each of the elements
                    if (!t.GetElementType().IsValueType)
                    {
                        Int32[] lengths = new Int32[t.GetArrayRank()];
                        Int32[] indicies = new Int32[lengths.Length];

                        // Get lengths from original array
                        for (int i = 0; i < lengths.Length; i++)
                        {
                            lengths[i] = resultArray.GetLength(i);
                        }

                        Int32 p = lengths.Length - 1;

                        /* Now we need to iterate though each of the ranks
                            * we need to keep it generic to support all array ranks */
                        while (Increment(indicies, lengths, p))
                        {
                            Object value = resultArray.GetValue(indicies);
                            if (value != null)
                                resultArray.SetValue(value.DeepClone(copies), indicies);
                        }
                    }
                    result = (T)(Object)resultArray;
                }
                return result;
            }
        }

        private static Boolean Increment(Int32[] indicies, Int32[] lengths, Int32 p)
        {
            if (p > -1)
            {
                indicies[p]++;
                if (indicies[p] < lengths[p])
                {
                    return true;
                }
                else
                {
                    if (Increment(indicies, lengths, p - 1))
                    {
                        indicies[p] = 0;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }

    

    public class VertexValue : Dictionary<string, double> { }

    
}