using System;
using System.Collections.Generic;

namespace KasiopeaApi
{
    internal static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> e, Action<T> a) {
            foreach (var item in e) a(item);
        }
    }
}