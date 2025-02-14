using Humanizer;
using ItemSwapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using MyExtensions;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Sources;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using UtfUnknown.Core.Probers;
using static System.Random;
namespace LootClass {
    public class LootSet : TagSerializable
    {
        static Random rnd = new();
        #region Pool Sets
        public Dictionary<int, LootPool> chestSet = new Dictionary<int, LootPool>();
        public HashSet<DropRuleLootPool> dropRuleSet = new HashSet<DropRuleLootPool>();
        public Dictionary<int, LootPool> shopSet = new Dictionary<int, LootPool>();
        public HashSet<LootPool> fishSet = new HashSet<LootPool>();
        public HashSet<LootPool> smashSet = new HashSet<LootPool>();
        public List<LootPool> questSet = new List<LootPool>();
        #endregion
        #region Add to Pool Methods
        public void AddChestPool(int theRegion, int[] chestIDs, int[] itemList, int slots = 0) {
            LootPool newPool = new(theRegion, itemList, slots);
            foreach (int chestID in chestIDs) {
                chestSet[chestID] = newPool;
            }
        }
        public void AddRulePool(int theRegion, int[] npcIDs, int[] itemList, int slots = 0) {
            DropRuleLootPool newPool = new(theRegion, itemList, npcIDs, slots);
            dropRuleSet.Add(newPool);
        }
        public void AddShopPool(int theRegion, int npcID, int[] itemList, int slots = 0) {
            LootPool newPool = new(theRegion, itemList, slots);
            shopSet[npcID] = newPool;
        }
        public void AddFishPool(int theRegion, int[] itemList, int slots = 0) {
            LootPool newPool = new(theRegion, itemList, slots);
            fishSet.Add(newPool);
        }
        public void AddBiomeCratePool(int[] crateIDs, LootPool pool, int slots = 0) {
            foreach (int crateID in crateIDs)
                chestSet[crateID] = pool;
        }
        public void AddSmashPool(int theRegion, int[] itemList, int slots = 0) {
            LootPool newPool = new(theRegion, itemList, slots);
            smashSet.Add(newPool);
        }
        public void AddQuestPool(int theRegion, int[] itemList, int slots = 0) {
            LootPool newPool = new(theRegion, itemList, slots);
            questSet.Add(newPool);
        }
        #endregion
        #region Rule Collection Methods
        public DropRuleLootPool[] GetRulePools(int npcID) => (from pool in dropRuleSet where pool.registeredIDs.Contains(npcID) select pool).ToArray();
        public int[] GetInitialRuleOptions(int npcID) {
            LootPool[] pools = GetRulePools(npcID);
            List<int> options = new List<int>();
            foreach (LootPool pool in pools) 
                foreach(int itemType in pool.initialSet)
                    options.AddRange(ItemReference.GetItemSet(itemType));
            
            return options.ToArray();
        }
        public LootPool GetFishPool(int itemID) => fishSet.FirstOrDefault(pool => pool.initialSet.Contains(itemID));
        public LootPool GetOrbPool(int itemID) => smashSet.FirstOrDefault(pool => pool.initialSet.Contains(itemID));
        #endregion
        public void DisablePools(IEnumerable<LootPool> poolSet, Func<LootPool, bool> filter = null) {
            if (poolSet is Dictionary<int, LootPool> dict)
                poolSet = dict.Values;
            foreach (LootPool pool in poolSet)
                if (filter is null || filter(pool)) {
                    pool.IsEnabled = false;
                }
        }
        public void AddPoolItems(List<int> items, IEnumerable<LootPool> pools) {
            foreach(LootPool pool in pools)
                if (pool.IsEnabled)
                    items.AddRange(pool.initialSet);
        }
        public void Randomize() {
            List<int> totalPool = new();
            HashSet<LootPool> validChests = (from key in chestSet.Keys where key < 100 select chestSet[key]).ToHashSet(); //pretty dumb exclusion, but this should prevent specifically biome crates from reappearing
            AddPoolItems(totalPool, validChests);
            AddPoolItems(totalPool, dropRuleSet);
            AddPoolItems(totalPool, shopSet.Values);
            AddPoolItems(totalPool, fishSet);
            AddPoolItems(totalPool, smashSet);
            AddPoolItems(totalPool, questSet);

            if (totalPool.ContainsDuplicates())
                throw new Exception("CONTAINS DUPLICATES");

            int totalSlots = totalPool.Count();

            foreach (DropRuleLootPool pool in dropRuleSet) {
                totalSlots -= pool.Fill(totalPool, 1);
            }

            foreach (LootPool pool in smashSet) {
                totalSlots -= pool.Fill(totalPool, 3);
            }
            foreach (LootPool pool in shopSet.Values) {
                totalSlots -= pool.Fill(totalPool);
            }
            foreach (LootPool pool in fishSet) {
                totalSlots -= pool.Fill(totalPool);
            }
            foreach (LootPool pool in questSet) {
                totalSlots -= pool.Fill(totalPool);
            }

            int chestSlots = totalSlots / validChests.Count();
            int chestSlotRemainder = totalSlots % validChests.Count();

            foreach (LootPool pool in validChests) {
                if (chestSlotRemainder > 0) {
                    totalSlots -= pool.Fill(totalPool, chestSlots + 1);
                    chestSlotRemainder--;
                } else {
                    totalSlots -= pool.Fill(totalPool, chestSlots);
                }
            }

            if (totalSlots != 0) {
                throw new Exception("We fucked up! Back to the lab again");
            }

            for (int i = 0; i > 10000; i++)
                if (GetRulePools(i).Any(p => p.randomSet is null))
                    throw new Exception("fuck");
            Console.WriteLine(this);

        }
        private string GetSetNames(LootPool pool) {
            string theText = "";
            foreach (int item in pool.GetSet())
                theText += $"{Lang.GetItemName(item)}, ";
            return theText;
        }
        public override string ToString()
        {
            string theText = "";
            foreach (int key in chestSet.Keys) {
                theText += $"CHEST {key}: ";
                theText += $"{GetSetNames(chestSet[key])}\n";
            }
            foreach (DropRuleLootPool pool in dropRuleSet) {
                theText += $"NPCS ";
                foreach (int npc in pool.registeredIDs)
                    theText += $"{Lang.GetNPCName(npc)} ";
                theText += ": ";
                theText += $"{GetSetNames(pool)}\n";
            }
            foreach (int key in shopSet.Keys) {
                theText += $"SHOP {Lang.GetNPCName(key)}: ";
                theText += $"{GetSetNames(shopSet[key])}\n";
            }
            foreach (LootPool pool in fishSet) {
                theText += $"FISH ";
                foreach (int fish in pool.initialSet)
                    theText += $"{Lang.GetItemName(fish)} ";
                theText += ": ";
                theText += $"{GetSetNames(pool)}\n";
            }
            foreach (LootPool pool in smashSet) {
                theText += $"SMASHABLES ";
                foreach (int item in pool.initialSet)
                    theText += $"{Lang.GetItemName(item)} ";
                theText += ": ";
                theText += $"{GetSetNames(pool)}\n";
            }
            foreach (LootPool pool in questSet) {
                theText += $"QUEST FISH: ";
                theText += $"{GetSetNames(pool)}\n";
            }
            return theText;
        }
        #region TagSerialize Methods
        public static readonly Func<TagCompound, LootSet> DESERIALIZER = Load;
        public TagCompound SerializeData()
        {
            return new TagCompound 
            {
                ["chestSetKeys"] = chestSet.Keys.ToList(),
                ["chestSetValues"] = chestSet.Values.ToList(),
                ["dropRuleSet"] = dropRuleSet.ToList(),
                ["shopSetKeys"] = shopSet.Keys.ToList(),
                ["shopSetValues"] = shopSet.Values.ToList(),
                ["fishSet"] = fishSet.ToList(),
                ["smashSet"] = smashSet.ToList(),
                ["questSet"] = questSet,
            };
        }
        public static LootSet Load(TagCompound tag)
        {
            var lootset = new LootSet();
            List<int> chestKeys = tag.Get<List<int>>("chestSetKeys");
            List<LootPool> chestValues = tag.Get<List<LootPool>>("chestSetValues");
            for (int i = 0; i < chestKeys.Count; i++)
            {
                lootset.chestSet[chestKeys[i]] = chestValues[i];
            }
            lootset.dropRuleSet = tag.Get<List<DropRuleLootPool>>("dropRuleSet").ToHashSet();
            List<int> shopKeys = tag.Get<List<int>>("shopSetKeys");
            List<LootPool> shopValues = tag.Get<List<LootPool>>("shopSetValues");
            for (int i = 0; i < shopKeys.Count; i++)
            {
                lootset.shopSet[shopKeys[i]] = shopValues[i];
            }
            lootset.fishSet = tag.Get<List<LootPool>>("fishSet").ToHashSet();
            lootset.smashSet = tag.Get<List<LootPool>>("smashSet").ToHashSet();
            lootset.questSet = tag.Get<List<LootPool>>("questSet");
            return lootset;
        }
        #endregion
    }
    public class LootPool : TagSerializable {
            protected int counter;
            public int[] initialSet;
            public int[] randomSet {get; protected set;}
            public bool IsEnabled = true;
            public int region;
            public LootPool(int theRegion, int[] itemList, int slots) {
                region = theRegion;
                initialSet = itemList;
                randomSet = new int[slots];
            }
            public int Fill(List<int> items, int length = 0) {
                if (randomSet.Length != 0) {
                    length = randomSet.Length;
                } else if (length == 0) {
                    length = initialSet.Length;
                }
                randomSet = items.GetRandomSubset(length, true);
                return length;
            }
            public virtual int GetNext() {
                int item;
                if (IsEnabled && randomSet is not null) {
                    item = randomSet[counter];
                    counter = (counter + 1)%randomSet.Length;
                } else { //if the pool is not enabled, we just default to what is normally there
                    item = initialSet[counter];
                    counter = (counter + 1)%initialSet.Length;
                }
                return item;
            }
            public int[] GetSet() => IsEnabled ? randomSet : initialSet;
            #region TagSerialize Methods
            public static readonly Func<TagCompound, LootPool> DESERIALIZER = Load;
            public virtual TagCompound SerializeData()
            {
                return new TagCompound
                {
                    ["counter"] = counter,
                    ["initialSet"] = initialSet,
                    ["randomSet"] = randomSet,
                    ["IsEnabled"] = IsEnabled,
                    ["region"] = region,
                };
            }

            public static LootPool Load(TagCompound tag)
            {
                var chestPool = new LootPool(tag.GetInt("region"), tag.GetIntArray("initialSet"), 1);
                chestPool.counter = tag.GetInt("counter");
                chestPool.randomSet = tag.GetIntArray("randomSet");
                chestPool.IsEnabled = tag.GetBool("IsEnabled");
                return chestPool;
            }
            #endregion
    }
    public class DropRuleLootPool : LootPool { //A more modular LootPool data type that has tags that can be matched to.
    //This is used for NPC drops, crates and boss bags, since their IDs are unlikely to overlap. 
        public int[] registeredIDs;
        public DropRuleLootPool(int theRegion, int[] itemList, int[] npcIDset, int slots) : base(theRegion, itemList, slots) {
            registeredIDs = npcIDset;
        }
        #region TagSeralize Methods
        public static readonly new Func<TagCompound, DropRuleLootPool> DESERIALIZER = Load;
        public override TagCompound SerializeData()
            {
                return new TagCompound
                {
                    ["counter"] = counter,
                    ["initialSet"] = initialSet,
                    ["randomSet"] = randomSet,
                    ["IsEnabled"] = IsEnabled,
                    ["region"] = region,
                    ["registeredIDs"] = registeredIDs,
                };
            }

        public static new DropRuleLootPool Load(TagCompound tag)
        {
            var npcPool = new DropRuleLootPool(tag.GetInt("region"), tag.GetIntArray("initialSet"), tag.GetIntArray("registeredIDs"), 1);
            npcPool.counter = tag.GetInt("counter");
            npcPool.randomSet = tag.GetIntArray("randomSet");
            npcPool.IsEnabled = tag.GetBool("IsEnabled");
            return npcPool;
        }
        #endregion
    }
        
}
