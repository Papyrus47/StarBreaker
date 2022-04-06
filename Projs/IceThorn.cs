using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace StarBreaker.Projs
{
    internal class IceThorn : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("冰柱");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 500;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.scale = 2;
        }
        public override void AI()
        {
            Projectile.velocity *= 1.00000001f;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.ai[0] = Projectile.alpha;
            if (Projectile.alpha > 0) Projectile.alpha -= 2;
            else Projectile.alpha = 0;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] > 0)
            {
                return false;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage /= (Main.masterMode || Main.expertMode) ? 2 : 1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.ai[0] > 0)
            {
                overPlayers.Add(index);
                return;
            }
            overWiresUI.Add(index);
        }
    }
}
