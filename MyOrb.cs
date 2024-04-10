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

namespace MyOrb {
    public class SwapOrb : GlobalTile {
        
        public override bool CanDrop(int i, int j, int type)
        {
            if (type == TileID.ShadowOrbs) {
                return false;
            }
            return true;
        }
        public override void Drop(int i, int j, int type)
        {
            if (type == TileID.ShadowOrbs) {
                LootSet mySet = ChestSpawn.mySet;
                LootPool pool = mySet.smashSet[TileID.ShadowOrbs];
                Item.NewItem(new EntitySource_TileBreak(i, j), i, j, 0, 0, pool.GetNext());
            }
        }
    }
}