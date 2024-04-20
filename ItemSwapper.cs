using System;
using System.Collections.Generic;
using System.Linq;
using static System.Random;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Audio;
using Terraria.GameContent.Achievements;

using LootClass;
using Terraria.GameContent.ItemDropRules;
using ItemSwapper;
using CustomDropRule;
using System.Linq.Expressions;
using ReLogic.Content;

using MyExtensions;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;
using Terraria.GameContent.Bestiary;

namespace ItemSwapper
{	
	public class ChestSpawn : ModSystem
	{
		public static LootSet mySet = new LootSet();
		public static void ResetSet() {
			/*Surface*/mySet.AddChestPool(new int[] {0, ItemID.WoodenCrate, ItemID.WoodenCrateHard}, new int[] {280, 281, 284, 285, 953, 946, 3068, 3069, 3084, 4341});
			/*Underground*/mySet.AddChestPool(new int[] {1, 8, 32, 50, 51, 56}, new int[] {49, 50, 53, 54, 975, 930, 997, 906});
			/*Ivy*/mySet.AddChestPool(new int[] {10}, new int[] {211, 212, 213, 964, 3017, 2292, 753});
			/*Ice*/mySet.AddChestPool(new int[] {11}, new int[] {670, 724, 950, 1319, 987, 1579, 669});
			/*Sky*/mySet.AddChestPool(new int[] {13}, new int[] {159, 65, 158, 2219});
			/*Web*/mySet.AddChestPool(new int[] {15}, new int[] {939});
			/*Water*/mySet.AddChestPool(new int[] {17}, new int[] {186, 187, 277, 4404, 863});
			/*Desert*/mySet.AddChestPool(new int[] {62, 69}, new int[] {934, 857, 4061, 4062, 4263, 4262, 4056, 4055, 4276});

			/*Bat Bat*/mySet.AddRulePool(new int[] {49, 634, 51, 60, 150, 93, 137, 151, 121, 152, 158}, new int[] {5097});
			/*Bezoar*/mySet.AddRulePool(new int[] {42, 176, 141}, new int[] {887});
			/*Blood Pool*/mySet.AddRulePool(new int[] {586, 587}, new int[] {4381, 4325, 4273});
			/*Bone Sword + Helmets*/mySet.AddRulePool(new int[] {21}, new int[] {1166, 954, 955});
			/*Cobalt Armor*/mySet.AddRulePool(new int[] {42, 43}, new int[] {960});
			/*Compass*/mySet.AddRulePool(new int[] {494, 496, 498, 58, 16, 185, 167, 197}, new int[] {393});
			/*Chain Knife*/mySet.AddRulePool(new int[] {49}, new int[] {1325});
			/*Demon Scythe*/mySet.AddRulePool(new int[] {62, 66}, new int[] {272});
			/*Depth Meter*/mySet.AddRulePool(new int[] {494, 496, 498, 49, 51, 150, 93, 634}, new int[] {18});
			/*Diving Helmet*/mySet.AddRulePool(new int[] {65}, new int[] {268});
			/*Exotic Scimitar*/mySet.AddRulePool(new int[] {207}, new int[] {3349});
			/*Gladius + Armor*/mySet.AddRulePool(new int[] {481}, new int[] {4463, 3187});
			/*Harpoon*/mySet.AddRulePool(new int[] {111, 26, 29, 27, 28}, new int[] {160});
			/*Mandible Blade*/mySet.AddRulePool(new int[] {580, 581}, new int[] {3772});
			/*Metal Detector*/mySet.AddRulePool(new int[] {195}, new int[] {3102});
			/*Mining Set*/mySet.AddRulePool(new int[] {44}, new int[] {410});
			/*Money Trough*/mySet.AddRulePool(new int[] {489, 490, 586, 587}, new int[] {3213});
			/*Night Vision Helmet*/mySet.AddRulePool(new int[] {482, 483}, new int[] {3109});
			/*Obsidian Rose*/mySet.AddRulePool(new int[] {24}, new int[] {1323});
			/*Paintball Gun*/mySet.AddRulePool(new int[] {227}, new int[] {3350});
			/*Rally*/mySet.AddRulePool(new int[] {494, 496, 498}, new int[] {3285});
			/*Shackle, Zombie Arm*/mySet.AddRulePool(new int[] {3}, new int[] {216, 1304});
			/*Shark Tooth Necklace*/mySet.AddRulePool(new int[] {489, 490}, new int[] {3212});
			/*Slime Staff*/mySet.AddRulePool(new int[] {-3, 1, -8, -7, -9, -6, 147, 537, -10, 184, 204, 16, -5, -4, 535, 302, 333, 334, 335, 336, 141, 121, 138, 658, 659, 660}, new int[] {1309});
			/*Stylish Scissors*/mySet.AddRulePool(new int[] {354}, new int[] {3352});
			/*Wizard Hat*/mySet.AddRulePool(new int[] {45}, new int[] {238});

			/*Shadow Armor*/mySet.AddRulePool(new int[] {6}, new int[] {956});
			/*Tentacle Spike Corr.*/mySet.AddRulePool(new int[] {956, 7}, new int[] {5094});

			/*Merchant*/mySet.AddShopPool(17, new int[] {1991, 88});
			/*Zoologist*/mySet.AddShopPool(633, new int[] {4759, 4672, 4716});
			/*Dryad*/mySet.AddShopPool(20, new int[] {144});
			/*Arms Dealer*/mySet.AddShopPool(19, new int[] {95, 98});
			/*Goblin Tinkerer*/mySet.AddShopPool(105, new int[] {128, 398, 486});
			/*Witch Doctor*/mySet.AddShopPool(228, new int[] {986});

			/*Any Accessories*/mySet.AddFishPool(new int[] {2423, 3225, 2420});
			/*Blood Moon*/mySet.AddFishPool(new int[] {4382});
			/*Rockfish*/mySet.AddFishPool(new int[] {2320});
			/*Demon Conch*/mySet.AddFishPool(new int[] {4819});
			/*Ocean*/mySet.AddFishPool(new int[] {2332, 2341, 2342});

			/*Wood Crate Base*/mySet.AddRulePool(new int[] {ItemID.WoodenCrate, ItemID.WoodenCrateHard, ItemID.IronCrate, ItemID.IronCrateHard}, new int[] {ItemID.SailfishBoots, ItemID.TsunamiInABottle});
			/*Hardmode Sundial*/mySet.AddRulePool(new int[] {ItemID.WoodenCrateHard, ItemID.IronCrateHard, ItemID.GoldenCrateHard}, new int[] {ItemID.Sundial});
			/*Iron Crate*/mySet.AddRulePool(new int[] {ItemID.IronCrate, ItemID.IronCrateHard}, new int[] {ItemID.GingerBeard, ItemID.TartarSauce, ItemID.FalconBlade});
			/*Golden Crate*/mySet.AddRulePool(new int[] {ItemID.GoldenCrate, ItemID.GoldenCrateHard}, new int[] {ItemID.HardySaddle, ItemID.EnchantedSword});

			/*Shadow Orb*/mySet.AddSmashPool(TileID.ShadowOrbs, new int[] {ItemID.Musket, ItemID.ShadowOrb, ItemID.Vilethorn, ItemID.BallOHurt, ItemID.BandofStarpower});

			/*Angler*/mySet.AddQuestPool(new int[] {ItemID.FuzzyCarrot, ItemID.AnglerHat, ItemID.HoneyAbsorbantSponge, ItemID.BottomlessHoneyBucket, ItemID.GoldenFishingRod, ItemID.BottomlessBucket, ItemID.SuperAbsorbantSponge, ItemID.GoldenBugNet, ItemID.FishHook, ItemID.FishMinecart, ItemID.SeashellHairpin, ItemID.FishCostumeMask, ItemID.HighTestFishingLine, ItemID.AnglerEarring, ItemID.TackleBox, ItemID.FishermansGuide, ItemID.WeatherRadio, ItemID.Sextant, ItemID.FishingBobber});

		}
        public override void OnModLoad() //Where all pools are initialized.
        {
            ResetSet();
        }
        public override void PostWorldGen() 
		{
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

				if (mySet.chestSet.Keys.Contains(chestKey) && mySet.chestSet[chestKey].IsEnabled) {
					int oldItem = chest.item[0].type;
					//int newItem = mySet.chestSet[chestKey].GetNext();
					int newItem = 960;
					Console.WriteLine($"COMPATIBLE CHEST {chestKey}: {chest.item[0].AffixName()}");

					int[] oldItemSet = ItemReference.GetItemSet(oldItem);
					int[] newItemSet = ItemReference.GetItemSet(newItem);
					chest.item = ItemReference.OffsetInventory(oldItemSet.Length, newItemSet.Length, chest.item);

					for(int i = 0; i < newItemSet.Length; i++) {
						chest.item[i].SetDefaults(newItemSet[i], false);
						chest.item[i].stack = ItemReference.GetQuant(newItemSet[i]);
					}

					
					
				} else {
					Console.WriteLine($"INCOMPATIBLE CHEST {chestKey}: {chest.item[0].AffixName()}");
				}
			}

        }
        public override void SaveWorldData(TagCompound tag)
        {
            tag[nameof(mySet)] = mySet;
			mySet = new LootSet();
			ResetSet();
        }
        public override void LoadWorldData(TagCompound tag)
        {
            mySet = tag.Get<LootSet>(nameof(mySet));
        }
    }

	public class EnemyLoot : GlobalNPC {

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        LootSet mySet = ChestSpawn.mySet;
		int npcTypeFormatted = ItemReference.IDNPC(npc.type);
		int[] options = mySet.GetInitialRuleOptions(npcTypeFormatted);
		if (options.Length != 0)
		{	
			npcLoot.RemoveWhere(rule => rule is CommonDrop normalDropRule && options.Contains(normalDropRule.itemId));
			npcLoot.Add(new LootsetDropRule(50));
			Console.WriteLine(npcTypeFormatted);
		}
    }
    public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
    {	
		LootSet mySet = ChestSpawn.mySet;
		if (mySet.shopSet.ContainsKey(npc.type) && mySet.shopSet[npc.type].IsEnabled) {
			LootPool pool = mySet.shopSet[npc.type];
			for (int i = 0; i < items.Length; i++) { //TODO: appeal to salanto
				Item item = items[i];
				if (item == null) {
					break;
				}
				if(pool.initialSet.Contains(item.type)) {

					int index = Array.IndexOf(pool.initialSet, item.type);
					int[] poolSet = pool.GetSet();
					int	newItem = poolSet[index];
					int[] newItemSet = ItemReference.GetItemSet(newItem, true);

					var itemSlices = items.SplitArray(i);
					var firstSlice = itemSlices.Item1;
					var secondSlice = ItemReference.OffsetInventory(1, newItemSet.Length, itemSlices.Item2);

					for(int j = 0; j < newItemSet.Length; j++) {
						secondSlice[j].SetDefaults(newItemSet[j], false);
					}
					items = firstSlice.Concat(secondSlice).ToArray();
				}
			}
		}
    }
}

	public class FishLoot : ModPlayer {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
			LootSet mySet = ChestSpawn.mySet;
			LootPool pool = mySet.GetFishPool(itemDrop);
			if (pool != null) {
				int newItem = pool.GetNext();

				itemDrop = newItem;
				Item itemRef = new Item();
				itemRef.SetDefaults(newItem);

				sonar.Text = itemRef.Name;
				sonar.Color = ItemRarity.GetColor(itemRef.rare);
				sonar.DurationInFrames = 360;
				sonar.Velocity = new Vector2(0, -7f);
			}
			
        }
	}
}