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

        public static T[] GetRandomSubset<T>(this IEnumerable<T> theEnum, int newArrayCount, bool removeSubsetFromList = false) {
            List<T> oldList = theEnum.ToList();
            if (oldList.Count() <= newArrayCount || newArrayCount == -1)
                return oldList.ToArray();
            T[] newArray = new T[newArrayCount];
            for (int i = 0; i < newArrayCount; i++) {
                T randItem = oldList[rnd.Next(oldList.Count())];
                newArray[i] = randItem;
                oldList.Remove(randItem);
                if (removeSubsetFromList && theEnum is List<T> theList) {
                    theList.Remove(randItem);
                } else if (removeSubsetFromList && theEnum is HashSet<T> theSet) {
                    theSet.Remove(randItem);
                } else if (removeSubsetFromList && theEnum is T[]) {
                    throw new Exception("GetRandomSubset can't remove elements from an array.");
                } else if (removeSubsetFromList) {
                    throw new Exception("GetRandomSubset can't remove elements from something that isn't a list or set.");
                }
            }
            return newArray;
        }
    }
}