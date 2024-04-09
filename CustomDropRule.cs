using System;
using System.Collections.Generic;
using System.Linq;
using static System.Random;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent.ItemDropRules;

using LootClass;
using ItemSwapper;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Cryptography;
using System.IO.Pipelines;

namespace CustomDropRule {

    public class LootsetDropRule : IItemDropRule {
        public int denominator;
        public int chestIDforBiomeCrates = -1;
        public bool crate;
        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }
        public LootsetDropRule(int myDenominator, bool isCrate = false, int chestID = -1) {
            ChainedRules = new List<IItemDropRuleChainAttempt>();
            denominator = myDenominator;
            chestIDforBiomeCrates = chestID;
            crate = isCrate;
        }
        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
            ItemDropAttemptResult result;
            if (info.player.RollLuck(denominator) < 1) {
                LootSet mySet = ChestSpawn.mySet;
                List<int> options = new List<int>();
                if (chestIDforBiomeCrates > -1) {
                    LootPool myPool = mySet.chestSet[chestIDforBiomeCrates];
                    options = myPool.GetSet().ToList();
                } else {
                    int ID;
                    if (crate) {
                        ID = info.item;
                    } else {
                        ID = ItemReference.IDNPC(info.npc.type);
                    }
                    NPCLootPool[] myPools = mySet.GetNPCPools(ID);
                    foreach (LootPool pool in myPools) {
                        options = options.Concat(pool.GetSet()).ToList();
                    }
                }
                
                int itemId = options[info.rng.Next(options.Count)];
                int[] itemSet = ItemReference.GetItemSet(itemId);
                foreach (int id in itemSet) {
                    CommonCode.DropItem(info, id, ItemReference.GetQuant(id));
                }
                result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }
            result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.FailedRandomRoll;
            return result;
            
        }
        public bool CanDrop(DropAttemptInfo info) => true;
        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            Chains.ReportDroprates(ChainedRules, 1f, drops, ratesInfo);
        }
    }
}