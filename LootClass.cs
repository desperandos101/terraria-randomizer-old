using Humanizer;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Sources;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static System.Random;
namespace LootClass { //LOOK MA IM PUSHED
    public static class ItemReference {
        private static Random rnd = new Random();

                private static readonly Dictionary<int, int[]> ItemSetDict = new Dictionary<int, int[]> {
            {930, new int[] {931}},
            {848, new int[] {866}}
        };

        private static readonly Dictionary<int, (int, int)> ItemQuantDict = new Dictionary<int, (int, int)> {
            {931, (25, 50)}
        };

        public static int GetQuant(int itemID) {
            if (ItemQuantDict.ContainsKey(itemID)) {
                    int lowerBound = ItemQuantDict[itemID].Item1;
                    int upperBound = ItemQuantDict[itemID].Item2;
                    return rnd.Next(lowerBound, upperBound);
                }
            return 1;
        }

        public static List<int> GetItemSet(int itemID)
        {
            List<int> ItemSetsNew = new List<int> {itemID};
            
            if (ItemSetDict.ContainsKey(itemID)) {
                int[] ItemSets = ItemSetDict[itemID];
                ItemSetsNew.Concat(ItemSets);
            }
            
            return ItemSetsNew;
        }
        private static readonly Dictionary<int, int> ChestDict = new Dictionary<int, int> {
            {21, 0},
            {467, 52}
        };
        private static readonly Dictionary<(int, int), int> WallOverride = new Dictionary<(int, int), int> {
            {(1, 187), 69} //Pyramid Chests
        };
        
        public static int IDChest(int TileID, int TileFrameX, int TileWall) {
            if (!ChestDict.ContainsKey(TileID)) {
                return -1;
            }
            if (TileFrameX % 36 != 0) {
                throw new Exception($"TileFrameX, {TileFrameX}, is not divisible by 36.");
            }
            int ChestID = TileFrameX / 36 + ChestDict[TileID];
            if (WallOverride.Keys.Contains((ChestID, TileWall))) {
                return WallOverride[(ChestID, TileWall)];
            }
            return ChestID;
        }
        private static readonly (int, HashSet<int>)[] NPCIDSets = {
            (3, new HashSet<int> {132, 186, 187, 188, 189, 200, 223, 161, 254, 255, 52, 53, 536, 319, 320, 321, 332, 436, 431, 432, 433, 434, 435, 331, 430, 590})
            };
        
        public static int IDNPC(int id) {
            foreach((int, HashSet<int>) idSet in NPCIDSets) {
                if(idSet.Item2.Contains(id)) {
                    return idSet.Item1;
                }
            }
            return id;
        }
    }
    public class LootSet : TagSerializable
    {
        public static readonly Func<TagCompound, LootSet> DESERIALIZER = Load;

        static Random rnd = new Random();
        public List<int> totalPool = new List<int>();
        public Dictionary<int, ChestPool> chestSet = new Dictionary<int, ChestPool>();
        public Dictionary<int, ChestPool> npcSet = new Dictionary<int, ChestPool>();

        public TagCompound SerializeData()
        {
            return new TagCompound 
            {
                ["totalPool"] = totalPool,
                ["chestSetKeys"] = chestSet.Keys.ToList(),
                ["chestSetValues"] = chestSet.Values.ToList(),
                ["npcSetKeys"] = npcSet.Keys.ToList(),
                ["npcSetValues"] = npcSet.Values.ToList(),
            };
        }

        public static LootSet Load(TagCompound tag)
        {
            var lootset = new LootSet();
            lootset.totalPool = tag.Get<List<int>>("totalPool");
            List<int> chestKeys = tag.Get<List<int>>("chestSetKeys");
            List<ChestPool> chestValues = tag.Get<List<ChestPool>>("chestSetValues");
            for (int i = 0; i < chestKeys.Count; i++)
            {
                lootset.chestSet[chestKeys[i]] = chestValues[i];
            }
            List<int> npcKeys = tag.Get<List<int>>("npcSetKeys");
            List<ChestPool> npcValues = tag.Get<List<ChestPool>>("npcSetValues");
            for (int i = 0; i < npcKeys.Count; i++)
            {
                lootset.npcSet[npcKeys[i]] = npcValues[i];
            }
            return lootset;
        }

        public void AddChestPool(int[] chestIDs, int[] itemList) {
            ChestPool newPool = new(itemList);
            foreach (int chestID in chestIDs) {
                chestSet[chestID] = newPool;
            }
            totalPool.AddRange(itemList);
        }
        public void AddNPCPool(int[] npcIDs, int[] itemList) {
            ChestPool newPool = new(itemList);
            foreach (int npcID in npcIDs) {
                npcSet[npcID] = newPool;
            }
            totalPool.AddRange(itemList);
        }
        public void Randomize() {
            List<int> totalPoolCopy = new(totalPool);
            HashSet<ChestPool> chestHashSet = chestSet.Values.Distinct().ToHashSet();
            HashSet<ChestPool> npcHashSet = npcSet.Values.Distinct().ToHashSet();
            HashSet<ChestPool> allPools = chestHashSet.Union(npcHashSet).ToHashSet();

            foreach (ChestPool pool in allPools) {
                int[] itemList = pool.randomSet;
                for (int i = 0; i < itemList.Length; i++) {
                    int randItem = totalPoolCopy[rnd.Next(totalPoolCopy.Count)];
                    itemList[i] = randItem;
                    totalPoolCopy.Remove(randItem);
                }
            }
        }
    }
    public class ChestPool : TagSerializable {
            public static readonly Func<TagCompound, ChestPool> DESERIALIZER = Load;
            public int counter;
            public readonly int[] initialSet;
            public int[] randomSet;
            public ChestPool(int[] itemList) {
                initialSet = itemList;
                randomSet = new int[itemList.Length];
            }
            public int GetNext() {
                int item = randomSet[counter];
                counter = (counter + 1)%randomSet.Length;
                return item;
            }
            public TagCompound SerializeData()
            {
                return new TagCompound
                {
                    ["counter"] = counter,
                    ["initialSet"] = initialSet,
                    ["randomSet"] = randomSet,
                };
            }

            public static ChestPool Load(TagCompound tag)
            {
                var chestPool = new ChestPool(tag.Get<int[]>("initialSet"));
                chestPool.counter = tag.GetInt("counter");
                chestPool.randomSet = tag.Get<int[]>("randomSet");
                return chestPool;
            }
    }
}