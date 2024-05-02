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
        private bool isNotEaterSegment(DropAttemptInfo info) => info.npc is null || !ItemReference.eowIDs.Contains(info.npc.type) || info.npc.boss;
        public LootPool[] Pools(DropAttemptInfo info) {
            int id = info.npc is null ? info.item : info.npc.type;
            return biomeCrate ? new LootPool[] {ChestSpawn.mySet.chestSet[id]} : ChestSpawn.mySet.GetRulePools(ItemReference.IDNPC(id));
        }
        public int[] Options(DropAttemptInfo info) {
            LootPool[] pools = Pools(info);
            
            List<int> options = new List<int>();
            foreach (LootPool pool in pools) {
                options.AddRange(pool.GetSet());
            }
            return options.ToArray();
        }
        public LootsetDropRule(int myDenominator, bool isBiomeCrate = false) {
            
            ChainedRules = new List<IItemDropRuleChainAttempt>();
            biomeCrate = isBiomeCrate;
            denominator = myDenominator;
        }
        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
            ItemDropAttemptResult result;
            if (info.player.RollLuck(denominator) < 1 && isNotEaterSegment(info) || (info.npc is not null && info.npc.boss)) {
                int[] options = Options(info);
                int minDrop = 1;
                if (info.npc is not null && info.npc.boss)
                    minDrop = 3;
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
        public bool CanDrop(DropAttemptInfo info) {
            if (!isNotEaterSegment(info))
                return false;
            if (info.npc is not null && info.IsExpertMode && info.npc.boss)
                return false;
            return true;
        }
        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            Chains.ReportDroprates(ChainedRules, 1f, drops, ratesInfo);
        }
    }
}