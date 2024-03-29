using System;
using System.Linq;

namespace MyExtensions {
    public static class MyExtensions {
        public static (T[], T[]) SplitArray<T>(this T[] array, int index) =>
        (array.Take(index).ToArray(), array.Skip(index).ToArray());
    }
}