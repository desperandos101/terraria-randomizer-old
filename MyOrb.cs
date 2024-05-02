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
    public class EntitySource_TileBreak_Rando : IEntitySource {
        public string? Context { get; }
    }
    public class CheckTileDrop : GlobalItem {
        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (source is EntitySource_TileBreak && item.type == ItemID.MusketBall && item.stack == 100) { //specifically to get rid of musket balls that always drop from shadow orbs
                item.TurnToAir();
            } else if (source is EntitySource_TileBreak tileSource) {
                LootPool pool = ChestSpawn.mySet.GetOrbPool(item.type);
                if (pool is not null) {
                    item.TurnToAir();
                    int[] newItemSet = ItemReference.GetItemSet(pool.GetNext());
                    Microsoft.Xna.Framework.Vector2 tilePos = new(tileSource.TileCoords.X, tileSource.TileCoords.Y);
                    foreach (int newItem in newItemSet) {
                        Item.NewItem(new EntitySource_TileBreak_Rando(), tilePos.ToWorldCoordinates(), 0, 0, newItem, ItemReference.GetQuant(newItem));
                    }
                }
            }
        }
    }
}