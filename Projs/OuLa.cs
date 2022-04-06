using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    internal class OuLa : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("幻影拳");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 3;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = 1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.alpha > 0) Projectile.alpha = 0;
            if (Projectile.velocity.Length() < 20)
            {
                Projectile.velocity *= Main.rand.NextFloat(1, 2);
                for (float i = 0; i < 10; i++)
                {
                    Vector2 center = Projectile.Center + Projectile.velocity + i.ToRotationVector2() * 20;
                    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.Firework_Yellow);
                    dust.noGravity = true;
                    dust.velocity = (Projectile.Center - center) / 13;
                    dust.velocity.X /= 5;
                    dust.velocity = dust.velocity.RotatedBy((Projectile.Center - center).ToRotation());
                }
            }
        }
    }
}
