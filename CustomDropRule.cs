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
        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }
        public bool biomeCrate;
        public int ID(DropAttemptInfo info) => info.npc.type == 0 ? info.item : info.npc.type;
        public LootPool[] Pools(int ID) => biomeCrate ? new LootPool[] {ChestSpawn.mySet.chestSet[ID]} : ChestSpawn.mySet.GetNPCPools(ID);
        public LootsetDropRule(int myDenominator, bool isBiomeCrate = false) {
            
            ChainedRules = new List<IItemDropRuleChainAttempt>();
            biomeCrate = isBiomeCrate;
            denominator = myDenominator;
        }
        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
            ItemDropAttemptResult result;
            if (info.player.RollLuck(denominator) < 1) {
                LootPool[] pools = Pools(ID(info));
                List<int> options = new List<int>();
                foreach (LootPool pool in pools) {
                    options = options.Concat(pool.GetSet()).ToList();
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