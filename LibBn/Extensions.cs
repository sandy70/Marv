﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LibBn
{
    public static class Extensions
    {
        public static BnVertexInput Find(this Dictionary<int, List<BnVertexInput>> vertexInputsByYear, int year, string key)
        {
            var vertexInputs = vertexInputsByYear.SingleOrDefault(x => x.Key == year).Value;

            if (vertexInputs == null)
            {
                return null;
            }
            else
            {
                var vertexInput = vertexInputs.SingleOrDefault(x => x.Key.Equals(key));
                return vertexInput;
            }
        }

        public static BnVertex GetVertex(this IEnumerable<BnVertex> vertices, string key)
        {
            foreach (var vertex in vertices)
            {
                if (vertex.Key.Equals(key)) return vertex;
            }

            return null;
        }

        public static void Remove(this Dictionary<int, List<BnVertexInput>> vertexInputsByYear, int year, string key)
        {
            var vertexInputs = vertexInputsByYear.SingleOrDefault(x => x.Key == year).Value;

            if (vertexInputs == null)
            {
                // do nothing
            }
            else
            {
                vertexInputs.RemoveAll(x => x.Key == key);
            }
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
    }
}