using System.Configuration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;

namespace PreBobbery {
    public class BobberOverride : GlobalProjectile {
        public override void OnKill(Projectile proj, int timeLeft)
        {
            if(proj.aiStyle == 61) {
                Console.WriteLine($"VALUE 0:{proj.ai[0]}");
                Console.WriteLine($"VALUE 1:{proj.ai[1]}");
                Console.WriteLine($"VALUE 2:{proj.ai[2]}");
            }
        }
    }
}