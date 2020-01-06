using System;
using System.Collections.Generic;
using System.Linq;

namespace StringMath.Tests
{
    static class Extensions
    {
        public static T[] RemoveLast<T>(this IEnumerable<T> collection)
        {
            var arr = collection.ToArray();
            var result = new T[arr.Length - 1];
            Array.Copy(arr, 0, result, 0, arr.Length - 1);
            return result;
        }
    }
}