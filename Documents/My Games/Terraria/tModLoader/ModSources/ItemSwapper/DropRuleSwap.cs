using System.Configuration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using ItemSwapper;
using Terraria.GameContent.ItemDropRules;
using System.Linq;
using LootClass;
using System.Collections.Generic;
using CustomDropRule;

namespace CrateDrop {
    public class ModifyCrates : GlobalItem {
        public static HashSet<int> mundaneCrateIDs = new HashSet<int>() {ItemID.WoodenCrate, ItemID.WoodenCrateHard, ItemID.IronCrate, ItemID.IronCrateHard, ItemID.GoldenCrate, ItemID.GoldenCrateHard};
        private static HashSet<int> biomeCrateIDs = new HashSet<int>() {ItemID.CorruptFishingCrate, ItemID.CorruptFishingCrateHard, ItemID.CrimsonFishingCrate, ItemID.CrimsonFishingCrateHard, ItemID.JungleFishingCrate, ItemID.JungleFishingCrateHard, ItemID.FrozenCrate, ItemID.FrozenCrateHard, ItemID.FloatingIslandFishingCrate, ItemID.FloatingIslandFishingCrateHard, ItemID.OceanCrate, ItemID.OceanCrateHard, ItemID.OasisCrate, ItemID.OasisCrateHard, ItemID.DungeonFishingCrate, ItemID.DungeonFishingCrateHard, ItemID.HallowedFishingCrate, ItemID.HallowedFishingCrateHard, ItemID.LavaCrate, ItemID.LavaCrateHard};
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {   
            if (mundaneCrateIDs.Contains(item.type)) { //This block replaces unique crate items, like the sailfish boots.
                List<IItemDropRule> test = itemLoot.Get();
                if (test[0] is AlwaysAtleastOneSuccessDropRule seqRule) {
                    seqRule.rules[0] = new LootsetDropRule(1);
                    if (item.type == ItemID.WoodenCrate || item.type == ItemID.WoodenCrateHard)
                        seqRule.rules[1] = new LootsetDropRule(1, true);
                }
            } else if (biomeCrateIDs.Contains(item.type)) { //This block replaces biome crate items.
                List<IItemDropRule> test = itemLoot.Get();
                if (test[0] is AlwaysAtleastOneSuccessDropRule seqRule)
                    seqRule.rules[0] = new LootsetDropRule(1, true);
            } else { //This block replaces bag items.
                LootSet mySet = ChestSpawn.mySet;
                int[] options = mySet.GetInitialRuleOptions(item.type); //Skipping the npc format step, we dont have to do this with boss bags
                if (options.Length != 0) {
                    itemLoot.RemoveWhere(rule => NPCDropRule.CheckRule(rule, options));
                    itemLoot.Add(new LootsetDropRule(1));
                }
            }
        }
    }
    public class NPCDropRule : GlobalNPC {
        public static bool CheckRule(IItemDropRule rule, int[] options) {
		return(rule is CommonDrop normalDropRule && options.Contains(normalDropRule.itemId)) 
		|| rule is OneFromOptionsDropRule seqRule && options.Intersect(seqRule.dropIds).Any() 
		|| rule is OneFromOptionsNotScaledWithLuckDropRule seqRule2 && options.Intersect(seqRule2.dropIds).Any();
	}
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            LootSet mySet = ChestSpawn.mySet;
            int npcTypeFormatted = ItemReference.IDNPC(npc.type);
            int[] itemsToRemove = mySet.GetInitialRuleOptions(npcTypeFormatted);
            if (itemsToRemove.Length != 0)
            {	
                npcLoot.RemoveWhere(rule => CheckRule(rule, itemsToRemove));
                npcLoot.RemoveWhere(rule => rule is DropBasedOnExpertMode seqRule && (CheckRule(seqRule.ruleForNormalMode, itemsToRemove) || CheckRule(seqRule.ruleForExpertMode, itemsToRemove)));
            }
            if (mySet.GetRulePools(npcTypeFormatted).Any())
                npcLoot.Add(new LootsetDropRule(25));
            
        }
    }
}