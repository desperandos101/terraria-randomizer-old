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

namespace CustomDropRule {
    public class LootsetDropRule : IItemDropRule {
        public int npcID;
        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }
        public LootsetDropRule(int myNPC) {
            npcID = myNPC;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }
        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
            int npcIDformatted = ItemReference.IDNPC(info.npc.type);
            LootSet mySet = ChestSpawn.mySet;
            int[] options = [];
            foreach (LootPool pool in mySet.GetNPCPools(npcIDformatted)) {
                options.Union(pool.randomSet).ToArray();
            }
            int itemId = options[info.rng.Next(0, options.Length)];
            CommonCode.DropItem(info, itemId, 1);
            ItemDropAttemptResult result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.Success;
            return result;
        }
        public bool CanDrop(DropAttemptInfo info) => true;
        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
{
	Chains.ReportDroprates(ChainedRules, 1f, drops, ratesInfo);
}
    }
}