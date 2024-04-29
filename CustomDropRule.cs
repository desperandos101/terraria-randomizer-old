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
using MyExtensions;

namespace CustomDropRule {

    public class LootsetDropRule : IItemDropRule {
        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }
        public int denominator;
        public bool biomeCrate;
        public int minDrop;
        public int[] Options(DropAttemptInfo info) {
            int id = info.npc is null ? info.item : info.npc.type;
            LootPool[] pools = biomeCrate ? new LootPool[] {ChestSpawn.mySet.chestSet[id]} : ChestSpawn.mySet.GetRulePools(ItemReference.IDNPC(id));
            
            List<int> options = new List<int>();
            foreach (LootPool pool in pools) {
                options.AddRange(pool.GetSet());
            }
            return options.ToArray();
        }
        public LootsetDropRule(int myDenominator, bool isBiomeCrate = false, int theMinDrop = 1) {
            
            ChainedRules = new List<IItemDropRuleChainAttempt>();
            biomeCrate = isBiomeCrate;
            denominator = myDenominator;
            minDrop = theMinDrop;
        }
        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
            ItemDropAttemptResult result;
            if (info.player.RollLuck(denominator) < 1) {
                int[] options = Options(info);
                
                int[] newItemIDs = options.GetRandomSubset(minDrop);
                foreach(int itemID in newItemIDs) {
                    int[] itemSet = ItemReference.GetItemSet(itemID);
                    foreach (int id in itemSet) {
                        CommonCode.DropItem(info, id, ItemReference.GetQuant(id));
                    }
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