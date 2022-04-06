using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    internal class FireFist : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("火焰");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 500;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
        }
        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedByRandom(0.01f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire3, 30);
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage /= (Main.masterMode || Main.expertMode) ? 2 : 1;
        }
    }
}
