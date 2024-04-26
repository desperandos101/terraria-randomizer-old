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
        private static HashSet<int> mundaneCrateIDs = new HashSet<int>() {ItemID.WoodenCrate, ItemID.WoodenCrateHard, ItemID.IronCrate, ItemID.IronCrateHard, ItemID.GoldenCrate, ItemID.GoldenCrateHard};
        private static HashSet<int> biomeCrateIDs = new HashSet<int>() {ItemID.JungleFishingCrate, ItemID.JungleFishingCrateHard, ItemID.FrozenCrate, ItemID.FrozenCrateHard, ItemID.FloatingIslandFishingCrate, ItemID.FloatingIslandFishingCrateHard, ItemID.OceanCrate, ItemID.OceanCrateHard, ItemID.OasisCrate, ItemID.OasisCrateHard};
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
            }
        }
    }
    public static class CrateReference { //lifted from TML source. ive separated it from my own code for reasons that should be somewhat apparent
            public static IItemDropRule[] woodenCrateDrop(IItemDropRule themed, IItemDropRule bc_surfaceLoot) => new IItemDropRule[]
            {
                ItemDropRule.SequentialRulesNotScalingWithLuck(1, themed),
                bc_surfaceLoot,
                ItemDropRule.SequentialRulesNotScalingWithLuck(7, coinWood),
                ItemDropRule.SequentialRulesNotScalingWithLuck(1, new OneFromRulesRule(7, oresWood), new OneFromRulesRule(8, barsWood)),
                new OneFromRulesRule(7, potionsWood),
            };
            public static IItemDropRule[] pearlwoodCrateDrop(IItemDropRule hardmodeThemed, IItemDropRule bc_surfaceLoot) => new IItemDropRule[]
            {
                ItemDropRule.SequentialRulesNotScalingWithLuck(1, hardmodeThemed),
                bc_surfaceLoot,
                ItemDropRule.SequentialRulesNotScalingWithLuck(7, coinWood),
                ItemDropRule.SequentialRulesNotScalingWithLuck(1,
                    ItemDropRule.SequentialRulesNotScalingWithLuck(7,
                        new OneFromRulesRule(2, hardmodeOresWood),
                        new OneFromRulesRule(1, oresWood)
                    ),
                    ItemDropRule.SequentialRulesNotScalingWithLuck(8,
                        new OneFromRulesRule(2, hardmodeBarsWood),
                        new OneFromRulesRule(1, barsWood)
                    )
                ),
                new OneFromRulesRule(7, potionsWood),
            };
            public static IItemDropRule[] ironCrateDrop(IItemDropRule themed) => new IItemDropRule[]
            {
                ItemDropRule.SequentialRulesNotScalingWithLuck(1, themed),
                ItemDropRule.NotScalingWithLuck(ItemID.GoldCoin, 4, 5, 10),
                ItemDropRule.SequentialRulesNotScalingWithLuck(1, new OneFromRulesRule(6, oresIron), new OneFromRulesRule(4, barsIron)),
                new OneFromRulesRule(4, potionsIron),
            };
            public static IItemDropRule[] mythrilCrate(IItemDropRule hardmodeThemed) => new IItemDropRule[]
            {
                ItemDropRule.SequentialRulesNotScalingWithLuck(1, hardmodeThemed),
                ItemDropRule.NotScalingWithLuck(ItemID.GoldCoin, 4, 5, 10),
                ItemDropRule.SequentialRulesNotScalingWithLuck(1,
                    ItemDropRule.SequentialRulesNotScalingWithLuck(6,
                        new OneFromRulesRule(2, hardmodeOresIron),
                        new OneFromRulesRule(1, oresIron)
                    ),
                    ItemDropRule.SequentialRulesNotScalingWithLuck(4,
                        new OneFromRulesRule(3, 2, hardmodeBarsIron),
                        new OneFromRulesRule(1, barsIron)
                    )
                ),
                new OneFromRulesRule(4, potionsIron),
            };
            #region FIRE IN THE HOLE
            private static IItemDropRule[] coinWood = new IItemDropRule[]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.GoldCoin, 3, 1, 5),
                ItemDropRule.NotScalingWithLuck(ItemID.SilverCoin, 1, 20, 90)
            };
            private static IItemDropRule[] oresWood = new IItemDropRule[]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.CopperOre, 1, 4, 15),
                ItemDropRule.NotScalingWithLuck(ItemID.TinOre, 1, 4, 15),
                ItemDropRule.NotScalingWithLuck(ItemID.IronOre, 1, 4, 15),
                ItemDropRule.NotScalingWithLuck(ItemID.LeadOre, 1, 4, 15)
            };
            private static IItemDropRule[] hardmodeOresWood = new IItemDropRule[] {
                ItemDropRule.NotScalingWithLuck(ItemID.CobaltOre, 1, 4, 15),
                ItemDropRule.NotScalingWithLuck(ItemID.PalladiumOre, 1, 4, 15)
            };
            private static IItemDropRule[] barsWood = new IItemDropRule[]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.CopperBar, 1, 2, 5),
                ItemDropRule.NotScalingWithLuck(ItemID.TinBar, 1, 2, 5),
                ItemDropRule.NotScalingWithLuck(ItemID.IronBar, 1, 2, 5),
                ItemDropRule.NotScalingWithLuck(ItemID.LeadBar, 1, 2, 5)
            };
            private static IItemDropRule[] hardmodeBarsWood = new IItemDropRule[] {
                ItemDropRule.NotScalingWithLuck(ItemID.CobaltBar, 1, 2, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.PalladiumBar, 1, 2, 3)
            };
            private static IItemDropRule[] potionsWood = new IItemDropRule[]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.ObsidianSkinPotion, 1, 1, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.SwiftnessPotion, 1, 1, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.IronskinPotion, 1, 1, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.NightOwlPotion, 1, 1, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.ShinePotion, 1, 1, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.HunterPotion, 1, 1, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.GillsPotion, 1, 1, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.MiningPotion, 1, 1, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.HeartreachPotion, 1, 1, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.TrapsightPotion, 1, 1, 3) // dangersense
            };
            private static IItemDropRule[] oresIron = new IItemDropRule[]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.CopperOre, 1, 12, 21),
                ItemDropRule.NotScalingWithLuck(ItemID.TinOre, 1, 12, 21),
                ItemDropRule.NotScalingWithLuck(ItemID.IronOre, 1, 12, 21),
                ItemDropRule.NotScalingWithLuck(ItemID.LeadOre, 1, 12, 21),
                ItemDropRule.NotScalingWithLuck(ItemID.SilverOre, 1, 12, 21),
                ItemDropRule.NotScalingWithLuck(ItemID.TungstenOre, 1, 12, 21)
            };
            private static IItemDropRule[] hardmodeOresIron = new IItemDropRule[]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.CobaltOre, 1, 12, 21),
                ItemDropRule.NotScalingWithLuck(ItemID.PalladiumOre, 1, 12, 21),
                ItemDropRule.NotScalingWithLuck(ItemID.MythrilOre, 1, 12, 21),
                ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumOre, 1, 12, 21)
            };
            private static IItemDropRule[] barsIron = new IItemDropRule[]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.CopperBar, 1, 4, 7),
                ItemDropRule.NotScalingWithLuck(ItemID.TinBar, 1, 4, 7),
                ItemDropRule.NotScalingWithLuck(ItemID.IronBar, 1, 4, 7),
                ItemDropRule.NotScalingWithLuck(ItemID.LeadBar, 1, 4, 7),
                ItemDropRule.NotScalingWithLuck(ItemID.SilverBar, 1, 4, 7),
                ItemDropRule.NotScalingWithLuck(ItemID.TungstenBar, 1, 4, 7)
            };
            private static IItemDropRule[] hardmodeBarsIron = new IItemDropRule[]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.CobaltBar, 1, 3, 7),
                ItemDropRule.NotScalingWithLuck(ItemID.PalladiumBar, 1, 3, 7),
                ItemDropRule.NotScalingWithLuck(ItemID.MythrilBar, 1, 3, 7),
                ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumBar, 1, 3, 7)
            };
            private static IItemDropRule[] potionsIron = new IItemDropRule[]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.ObsidianSkinPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.SpelunkerPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.HunterPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.GravitationPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.MiningPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.HeartreachPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.CalmingPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.FlipperPotion, 1, 2, 4)
            };
            #endregion
    }
}