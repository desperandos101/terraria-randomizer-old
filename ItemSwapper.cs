using System;
using System.Collections.Generic;
using System.Linq;
using static System.Random;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using LootClass;
using Terraria.GameContent.ItemDropRules;
using ItemSwapper;
using System.Linq.Expressions;
using ReLogic.Content;

namespace ItemSwapper
{	
	public class ChestSpawn : ModSystem
	{
		public static LootSet mySet = new LootSet();
		public override void PostWorldGen()
		{
            /*
			21, 0: Surface
			21, 1: Underground
			21, 10: Ivy
			21, 11: Ice
			21, 13: Sky
			21, 15: Web
			21, 17: Water
			467, 10: Desert

			*/
            mySet.AddChestPool(new int[] { 0 }, new int[] { 280, 281, 284, 285, 953, 946, 3068, 3069, 3084, 4341 });
			mySet.AddChestPool(new int[] {1, 8, 32, 50, 51, 54}, new int[] {49, 50, 53, 54, 975, 930, 997, 906, 947});
			mySet.AddChestPool(new int[] {10}, new int[] {211, 212, 213, 964, 3017, 2292, 753});
			mySet.AddChestPool(new int[] {11}, new int[] {670, 724, 950, 1319, 987, 1579, 669});
			mySet.AddChestPool(new int[] {13}, new int[] {159, 65, 158, 2219});
			mySet.AddChestPool(new int[] {15}, new int[] {939});
			mySet.AddChestPool(new int[] {17}, new int[] {186, 187, 277, 4404, 863});
			mySet.AddChestPool(new int[] {62, 69}, new int[] {934, 857, 4061, 4062, 4263, 4262, 4056, 4055, 4276});

			mySet.AddNPCPool(new int[] {3}, new int[] {216});

			mySet.Randomize();

			var chestList = from chest in Main.chest
							where chest != null
							select chest;
			
			foreach (Chest chest in chestList) {
				Tile mainTile = Main.tile[chest.x, chest.y];

				int chestTileID = mainTile.TileType;
				int chestType = mainTile.TileFrameX;
				int chestWall = mainTile.WallType;

				int chestKey = ItemReference.IDChest(chestTileID, chestType, chestWall);
				if (chestKey == -1) {
					continue;
				}

				if (mySet.chestSet.Keys.Contains(chestKey)) {
					int oldItem = chest.item[0].type;
					int newItem = mySet.chestSet[chestKey].GetNext();
					Console.WriteLine($"COMPATIBLE CHEST {chestKey}: {chest.item[0].AffixName()}");

					List<int> oldItemSet = ItemReference.GetItemSet(oldItem);
					List<int> newItemSet = ItemReference.GetItemSet(newItem);
					int[] chestItemTypes = (from item in chest.item select item.type).ToArray();

					if (oldItemSet.Count != newItemSet.Count) //offset method to properly space out chest items
					{
						int offset = newItemSet.Count - oldItemSet.Count;
						for (int i = 0; i < 40; i++)
						{
                            if (i + offset < 0 || i + offset > 39)
                            {
                                continue;
                            }

                            if (chest.item[i].type == ItemID.None && chestItemTypes[i + offset] == 0)
							{
								break;
							}
							
							chest.item[i + offset].SetDefaults(chestItemTypes[i], false);
						}
					}

					for(int i = 0; i < newItemSet.Count; i++) {
						chest.item[i].SetDefaults(newItemSet[i], false);
						chest.item[i].stack = ItemReference.GetQuant(newItemSet[i]);
					}

					
					
				} else {
					Console.WriteLine($"INCOMPATIBLE CHEST {chestKey}: {chest.item[0].AffixName()}");
				}
			}
			Console.WriteLine(mySet.npcSet.Keys.ToList()[0]);

        }
        public override void SaveWorldData(TagCompound tag)
        {
            tag[nameof(mySet)] = mySet;
			mySet = new LootSet();
        }
        public override void LoadWorldData(TagCompound tag)
        {
            mySet = tag.Get<LootSet>(nameof(mySet));
        }
    }
}
	public class EnemyLoot : GlobalNPC {

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        LootSet mySet = ChestSpawn.mySet;
		int npcTypeFormatted = ItemReference.IDNPC(npc.type);
		if (mySet.npcSet.Keys.Contains(npcTypeFormatted))
		{
			npcLoot.RemoveWhere(rule => rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && mySet.totalPool.Contains(normalDropRule.itemId));
		}
    }
    public override void OnKill(NPC npc)
    {
        LootSet mySet = ChestSpawn.mySet;
        int npcTypeFormatted = ItemReference.IDNPC(npc.type);
        if (mySet.npcSet.Keys.Contains(npcTypeFormatted))
		{
			ChestPool itemPool = mySet.npcSet[npcTypeFormatted];
            int newItem = itemPool.GetNext();
            Item.NewItem(npc.GetSource_Death(), npc.Center, newItem, 1, true);
		}
    }

}