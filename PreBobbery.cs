using System.Configuration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using ItemSwapper;

namespace PreBobbery {
    public class BobberOverride : GlobalProjectile {
        public override bool PreKill(Projectile proj, int timeLeft) {
            if (Main.myPlayer == proj.owner && proj.bobber) {
                if (Main.myPlayer == proj.owner && proj.bobber) { //copy pasted from source code
                PopupText.ClearSonarText();
                if (proj.localAI[2] >= 1) {
                    int customSonarText = (int)(proj.localAI[2] - 1);
                    if (Main.popupText[customSonarText].sonar) {
                        Main.popupText[customSonarText].active = false;
                    }
                }
                if (proj.ai[1] > 0f) {
                    int itemType = (int)proj.ai[1];
                    int[] itemArray = ItemReference.GetItemSet(itemType);
                    foreach(int item in itemArray) {
                        GiveFish(Main.player[proj.owner], item, proj); }
                }
                Console.WriteLine(proj.ai[1]);
                proj.ai[1] = 0f;
                return false;
            }
            }
            return true;
        }
        private void GiveFish(Player thePlayer, int itemType, Projectile proj) {
            Item item = new Item();
            item.SetDefaults(itemType);
            if (itemType == 3196) { //just for bomb fishies and frost daggers, aka more source code stuff
                int finalFishingLevel = thePlayer.GetFishingConditions().FinalFishingLevel;
                int minValue = (finalFishingLevel / 20 + 3) / 2;
                int num = (finalFishingLevel / 10 + 6) / 2;
                if (Main.rand.Next(50) < finalFishingLevel)
                    num++;

                if (Main.rand.Next(100) < finalFishingLevel)
                    num++;

                if (Main.rand.Next(150) < finalFishingLevel)
                    num++;

                if (Main.rand.Next(200) < finalFishingLevel)
                    num++;

                int stack = Main.rand.Next(minValue, num + 1);
                item.stack = stack;
            }

            if (itemType == 3197) {
                int finalFishingLevel2 = thePlayer.GetFishingConditions().FinalFishingLevel;
                int minValue2 = (finalFishingLevel2 / 4 + 15) / 2;
                int num2 = (finalFishingLevel2 / 2 + 40) / 2;
                if (Main.rand.Next(50) < finalFishingLevel2)
                    num2 += 6;

                if (Main.rand.Next(100) < finalFishingLevel2)
                    num2 += 6;

                if (Main.rand.Next(150) < finalFishingLevel2)
                    num2 += 6;

                if (Main.rand.Next(200) < finalFishingLevel2)
                    num2 += 6;

                int stack2 = Main.rand.Next(minValue2, num2 + 1);
                item.stack = stack2;
            }
            PlayerLoader.ModifyCaughtFish(thePlayer, item);
            ItemLoader.CaughtFishStack(item); //THIS IS HOW I KNOW IM DEEP IN SOME SHIT

            item.newAndShiny = true;
            Item item2 = thePlayer.GetItem(proj.owner, item, default(GetItemSettings));
            if (item2.stack > 0) {
                int number = Item.NewItem(new EntitySource_OverfullInventory(thePlayer), (int)proj.position.X, (int)proj.position.Y, proj.width, proj.height, itemType, item2.stack, noBroadcast: false, 0, noGrabDelay: true);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
            }
            else {
                item.position.X = proj.Center.X - (float)(item.width / 2);
                item.position.Y = proj.Center.Y - (float)(item.height / 2);
                item.active = true;
                PopupText.NewText(PopupTextContext.RegularItemPickup, item, 0);
            }
        }
    }
}