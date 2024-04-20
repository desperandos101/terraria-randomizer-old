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
using System.Numerics;

namespace MyOrb {
    public class StopOrb : GlobalTile {
        
        public override bool CanDrop(int i, int j, int type)
        {
            if (type == TileID.ShadowOrbs) {
                return false;
            }
            return true;
        }
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (type == TileID.ShadowOrbs) {
                CheckOrb.lastOrbX = i;
                CheckOrb.lastOrbY = j;
            }
        }
    }
    public class CheckOrb : ModSystem {
        private static int localOrbCount;
        public static int lastOrbX;
        public static int lastOrbY;
        public override void OnWorldLoad()
        {
            localOrbCount = WorldGen.shadowOrbCount;
        }
        public override void PostUpdateWorld()
        {
            if (localOrbCount != WorldGen.shadowOrbCount) { //goes off whenever a change in orb count is detected
                localOrbCount = WorldGen.shadowOrbCount;
                
                int playerIndex = Player.FindClosest(new Microsoft.Xna.Framework.Vector2(lastOrbX, lastOrbY), 0, 0);
                LootPool pool = ChestSpawn.mySet.smashSet[TileID.ShadowOrbs];
                int[] itemSet = ItemReference.GetItemSet(pool.GetNext());
                foreach (int item in itemSet)
                    Main.player[playerIndex].QuickSpawnItem(new EntitySource_TileBreak(lastOrbX, lastOrbY), item, ItemReference.GetQuant(item));
            }
        }
    }
}