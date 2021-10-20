using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSystem.Domain
{
    public static class ArrayExtensions
    {
        public static bool IsSingle<T>(this T[] array)
            => array.Length == 1;

        public static bool IsNotSingle<T>(this T[] array)
            => !array.IsSingle();

        public static bool IsEmpty<T>(this IEnumerable<T> source)
            => !source.Any();

        public static bool IsNotEmpty<T>(this IEnumerable<T> source)
            => source.Any();

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}