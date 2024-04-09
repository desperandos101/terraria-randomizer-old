using Humanizer;
using ItemSwapper;
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
    public class LootSet : TagSerializable
    {
        public static readonly Func<TagCompound, LootSet> DESERIALIZER = Load;

        static Random rnd = new();
        public List<int> totalPool = new List<int>();
        public Dictionary<int, LootPool> chestSet = new Dictionary<int, LootPool>();
        public HashSet<NPCLootPool> npcSet = new HashSet<NPCLootPool>();
        public Dictionary<int, LootPool> shopSet = new Dictionary<int, LootPool>();
        public HashSet<LootPool> fishSet = new HashSet<LootPool>();
        public TagCompound SerializeData()
        {
            return new TagCompound 
            {
                ["totalPool"] = totalPool,
                ["chestSetKeys"] = chestSet.Keys.ToList(),
                ["chestSetValues"] = chestSet.Values.ToList(),
                ["npcSet"] = npcSet.ToList(),
                ["shopSetKeys"] = shopSet.Keys.ToList(),
                ["shopSetValues"] = shopSet.Values.ToList(),
                ["fishSet"] = fishSet.ToList(),
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
            lootset.npcSet = tag.Get<List<NPCLootPool>>("npcSet").ToHashSet();
            List<int> shopKeys = tag.Get<List<int>>("shopSetKeys");
            List<LootPool> shopValues = tag.Get<List<LootPool>>("shopSetValues");
            for (int i = 0; i < shopKeys.Count; i++)
            {
                lootset.shopSet[shopKeys[i]] = shopValues[i];
            }
            lootset.fishSet = tag.Get<List<LootPool>>("fishSet").ToHashSet();
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
        public void AddShopPool(int npcID, int[] itemList) {
            LootPool newPool = new(itemList);
            shopSet[npcID] = newPool;
            totalPool.AddRange(itemList);
        }
        public void AddFishPool(int[] itemList) {
            LootPool newPool = new(itemList);
            fishSet.Add(newPool);
            totalPool.AddRange(itemList);
        }
        public NPCLootPool[] GetNPCPools(int npcID) => (from pool in npcSet where pool.registeredIDs.Contains(npcID) select pool).ToArray();
        public LootPool GetFishPool(int itemID) {
            foreach (LootPool pool in fishSet) {
                if (pool.initialSet.Contains(itemID)) {
                    return pool;
                }
            }
            return null;
        } 
        public void Randomize() { //TODO: make it so totalset is synthesized on method call, accounting for IsEnabled
            List<int> totalPoolCopy = new(totalPool);
            HashSet<LootPool> chestHashSet = chestSet.Values.Distinct().ToHashSet();
            HashSet<LootPool> npcHashSet = new(npcSet);
            HashSet<LootPool> shopHashSet = shopSet.Values.ToHashSet();
            HashSet<LootPool> fishHashSet = new(fishSet);
            HashSet<LootPool> allPools = ItemReference.THE_SETMIXER(new HashSet<LootPool>[] {chestHashSet, npcHashSet, shopHashSet, fishHashSet});

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
            public bool IsEnabled = true;
            public Random rnd = new Random(); //AAAAAUUUUUUGHHHHH
            public LootPool(int[] itemList) {
                initialSet = itemList;
                randomSet = new int[itemList.Length];
            }
            public virtual int GetNext() {
                int item;
                if (IsEnabled) {
                    item = randomSet[counter];
                } else { //if the pool is not enabled, we just default to what is normally there
                    item = initialSet[counter];
                }
                counter = (counter + 1)%randomSet.Length;
                return item;
            }
            public int[] GetSet() => IsEnabled ? randomSet : initialSet;
            public virtual int GetRandom() {
                int item;
                if (IsEnabled) {
                    item = randomSet[rnd.Next(randomSet.Length)];
                } else {
                    item = initialSet[rnd.Next(initialSet.Length)];
                }
                return item;
            }
            public virtual TagCompound SerializeData()
            {
                return new TagCompound
                {
                    ["counter"] = counter,
                    ["initialSet"] = initialSet,
                    ["randomSet"] = randomSet,
                    ["IsEnabled"] = IsEnabled,
                };
            }

            public static LootPool Load(TagCompound tag)
            {
                var chestPool = new LootPool(tag.Get<int[]>("initialSet"));
                chestPool.counter = tag.GetInt("counter");
                chestPool.randomSet = tag.Get<int[]>("randomSet");
                chestPool.IsEnabled = tag.GetBool("IsEnabled");
                return chestPool;
            }
    }
    public class NPCLootPool : LootPool { //We make a separate class for NPCS because there can be overlaps.
    //For example, all zombies drop shackles, and all slimes drop slime staffs. Slimed Zombies can drop both.
        public static readonly new Func<TagCompound, NPCLootPool> DESERIALIZER = Load;
        public int[] registeredIDs;
        public NPCLootPool(int[] itemList, int[] npcIDset) : base(itemList) {
            registeredIDs = npcIDset;
        }
        public override TagCompound SerializeData()
            {
                return new TagCompound
                {
                    ["counter"] = counter,
                    ["initialSet"] = initialSet,
                    ["randomSet"] = randomSet,
                    ["IsEnabled"] = IsEnabled,
                    ["registeredIDs"] = registeredIDs,
                };
            }

            public static new NPCLootPool Load(TagCompound tag)
            {
                var npcPool = new NPCLootPool(tag.GetIntArray("initialSet"), tag.GetIntArray("registeredIDs"));
                npcPool.counter = tag.GetInt("counter");
                npcPool.randomSet = tag.GetIntArray("randomSet");
                npcPool.IsEnabled = tag.GetBool("IsEnabled");
                return npcPool;
            }
        }
    }
