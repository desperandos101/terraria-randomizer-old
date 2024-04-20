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

namespace Angler {
    public class AnglerLoot : ModPlayer {
        public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
        {
            LootPool pool = ChestSpawn.mySet.questSet[0];
            int[] initialSet = ItemReference.AddExtrasToItemSet(pool.initialSet);
            int itemInSet = 0;
            foreach (Item item in rewardItems) {
                if (initialSet.Contains(item.type))
                    itemInSet = item.type;
                    break;
            }
            if (itemInSet > 0) {
                int[] oldItemSet = ItemReference.GetItemSet(itemInSet);
                rewardItems.RemoveAll(item => oldItemSet.Contains(item.type));
                int newItem = pool.GetNext();
                int[] newItemSet = ItemReference.GetItemSet(newItem);
                foreach (int item in newItemSet) {
                    Item extraItem = new Item();
                    extraItem.SetDefaults(item);
                    rewardItems.Add(extraItem);
                }            
            }
        }            
    }
    /*
    	public class AnglerLoot : GlobalNPC {
		private static int questCounter = 0;
        public override void OnChatButtonClicked(NPC npc, bool firstButton)
        {
            if (npc.type == NPCID.Angler) {
				if (firstButton && !Main.LocalPlayer.HasItem(Main.anglerQuestItemNetIDs[Main.anglerQuest]))
					AnglerQuestSwapNext();
			}
        }
		public void AnglerQuestSwapNext() //modified from regular swap
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;

			Main.anglerWhoFinishedToday.Clear();
			Main.anglerQuestFinished = false;
			bool flag = NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || Main.hardMode || NPC.downedSlimeKing || NPC.downedQueenBee;
			bool flag2 = true;
			while (flag2) {
				flag2 = false;
				// Patch note: ↑flag2 and num↓ are used below.
				
				questCounter = (questCounter + 1) % Main.anglerQuestItemNetIDs.Length;

				Main.anglerQuest = questCounter;
				int num = Main.anglerQuestItemNetIDs[Main.anglerQuest];
				
				bool hardMode = Main.hardMode;
				if (num == 2454 && (!hardMode || WorldGen.crimson))
					flag2 = true;

				if (num == 2457 && WorldGen.crimson)
					flag2 = true;

				if (num == 2462 && !hardMode)
					flag2 = true;

				if (num == 2463 && (!hardMode || !WorldGen.crimson))
					flag2 = true;

				if (num == 2465 && !hardMode)
					flag2 = true;

				if (num == 2468 && !hardMode)
					flag2 = true;

				if (num == 2471 && !hardMode)
					flag2 = true;

				if (num == 2473 && !hardMode)
					flag2 = true;

				if (num == 2477 && !WorldGen.crimson)
					flag2 = true;

				if (num == 2480 && !hardMode)
					flag2 = true;

				if (num == 2483 && !hardMode)
					flag2 = true;

				if (num == 2484 && !hardMode)
					flag2 = true;

				if (num == 2485 && WorldGen.crimson)
					flag2 = true;

				if ((num == 2476 || num == 2453 || num == 2473) && !flag)
					flag2 = true;

				ItemLoader.IsAnglerQuestAvailable(num, ref flag2);
			}

			NetMessage.SendAnglerQuest(-1);
		}
    }
    public class FishSet : ModSystem {

    }*/
}