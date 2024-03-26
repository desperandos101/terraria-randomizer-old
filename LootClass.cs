using Humanizer;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Sources;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static System.Random;
namespace LootClass { //LOOK MABABA
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
            (3, new HashSet<int> {132, 186, 187, 188, 189, 200, 223, 161, 254, 255, 52, 53, 536, 319, 320, 321, 332, 436, 431, 432, 433, 434, 435, 331, 430, 590}),
            (1, new HashSet<int> {302, 333, 334, 335, 336}),
            (494, new HashSet<int> {495}),
            (496, new HashSet<int> {497}),
            (498, new HashSet<int> {499, 500, 501, 502, 503, 504, 505, 506})
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

        static Random rnd = new();
        public List<int> totalPool = [];
        public Dictionary<int, LootPool> chestSet = [];
        public HashSet<NPCLootPool> npcSet = [];

        public TagCompound SerializeData()
        {
            return new TagCompound 
            {
                ["totalPool"] = totalPool,
                ["chestSetKeys"] = chestSet.Keys.ToList(),
                ["chestSetValues"] = chestSet.Values.ToList(),
                ["npcSet"] = npcSet.ToList(),
            };
        }

        public static LootSet Load(TagCompound tag)
        {
            var lootset = new LootSet();
            lootset.totalPool = tag.Get<List<int>>("totalPool");
            List<int> chestKeys = tag.Get<List<int>>("chestSetKeys");
            List<LootPool> chestValues = tag.Get<List<LootPool>>("chestSetValues");
            for (int i = 0; i < chestKeys.Count; i++)
            {
                lootset.chestSet[chestKeys[i]] = chestValues[i];
            }
            lootset.npcSet = tag.Get<HashSet<NPCLootPool>>("npcSet");
            return lootset;
        }

        public void AddChestPool(int[] chestIDs, int[] itemList) {
            LootPool newPool = new(itemList);
            foreach (int chestID in chestIDs) {
                chestSet[chestID] = newPool;
            }
            totalPool.AddRange(itemList);
        }
        public void AddNPCPool(int[] npcIDs, int[] itemList) {
            NPCLootPool newPool = new(itemList, npcIDs);
            npcSet.Add(newPool);
            totalPool.AddRange(itemList);
        }
        public NPCLootPool[] GetNPCPools(int npcID) => (from pool in npcSet where pool.registeredIDs.Contains(npcID) select pool).ToArray();
        public void Randomize() {
            List<int> totalPoolCopy = new(totalPool);
            HashSet<LootPool> chestHashSet = chestSet.Values.Distinct().ToHashSet();
            HashSet<LootPool> npcHashSet = new(npcSet);
            HashSet<LootPool> allPools = chestHashSet.Union(npcHashSet).ToHashSet();

            foreach (LootPool pool in allPools) {
                int[] itemList = pool.randomSet;
                for (int i = 0; i < itemList.Length; i++) {
                    int randItem = totalPoolCopy[rnd.Next(totalPoolCopy.Count)];
                    itemList[i] = randItem;
                    totalPoolCopy.Remove(randItem);
                }
            }
        }
    }
    public class LootPool : TagSerializable {
            public static readonly Func<TagCompound, LootPool> DESERIALIZER = Load;
            public int counter;
            public readonly int[] initialSet;
            public int[] randomSet;
            public LootPool(int[] itemList) {
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

            public static LootPool Load(TagCompound tag)
            {
                var chestPool = new LootPool(tag.Get<int[]>("initialSet"));
                chestPool.counter = tag.GetInt("counter");
                chestPool.randomSet = tag.Get<int[]>("randomSet");
                return chestPool;
            }
    }
    public class NPCLootPool : LootPool { //We make a separate class for NPCS because there can be overlaps.
    //For example, all zombies drop shackles, and all slimes drop slime staffs. Slimed Zombies can drop both.
        public int[] registeredIDs;
        public NPCLootPool(int[] itemList, int[] npcIDset) : base(itemList) {
            registeredIDs = npcIDset;
        }
        public new TagCompound SerializeData()
            {
                return new TagCompound
                {
                    ["counter"] = counter,
                    ["initialSet"] = initialSet,
                    ["randomSet"] = randomSet,
                    ["registeredIDs"] = registeredIDs,
                };
            }

            public static new NPCLootPool Load(TagCompound tag)
            {
                var npcPool = new NPCLootPool(tag.Get<int[]>("initialSet"), tag.Get<int[]>("registeredIDs"));
                npcPool.counter = tag.GetInt("counter");
                npcPool.randomSet = tag.Get<int[]>("randomSet");
                return npcPool;
            }
    }
}