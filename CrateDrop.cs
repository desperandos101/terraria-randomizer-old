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

namespace CrateDrop {
    public class ModifyCrates : GlobalItem {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot) //Fair warning, most of this shit is hardcoded sadness, taken from TML source.
        //I had no other option.
        {   
            LootSet mySet = ChestSpawn.mySet;
            if (mySet.crateSet.ContainsKey(item.type) && mySet.crateSet[item.type].IsEnabled) { //This block replaces unique crate items, like the sailfish boots.
                List<IItemDropRule> test = itemLoot.Get();
                itemLoot.Remove(test[0]); //hardcoding. crate specific accessories are always indexed
                itemLoot.Add(ItemDropRule.Common(2334, 1));
            }
        }
    }
}