using System;
using System.Linq;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;

namespace MyExtensions {
    public static class MyExtensions {
        static Random rnd = new();
        public static (T[], T[]) SplitArray<T>(this T[] array, int index) =>
        (array.Take(index).ToArray(), array.Skip(index).ToArray());

        public static T[] GetRandomSubset<T>(this IEnumerable<T> theEnum, int newArrayCount) {
            T[] oldArray = theEnum.ToArray();
            if (oldArray.Length >= newArrayCount || newArrayCount == -1)
                return oldArray;
            T[] newArray = new T[newArrayCount];
            for (int i = 0; i < newArrayCount; i++) {
                newArray[i] = oldArray[rnd.Next(oldArray.Length)];
            }
            return newArray;
        }
    }
}