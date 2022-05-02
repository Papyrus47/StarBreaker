using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    internal class SniperGloves : ModProjectile
    {
        private Player target => Main.player[(int)Projectile.ai[0]];
        private int Bullet => (int)Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("狙击");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 200;
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            if (target.dead || target == null) Projectile.Kill();
            Projectile.velocity = (Projectile.velocity * 10 + (target.Center - Projectile.Center).SafeNormalize(Vector2.One) * 5) / 11;
            Projectile.rotation = 0;
        }
        public override void Kill(int timeLeft)
        {
            if (target == null) return;
            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2)
            {
                Vector2 center = Projectile.position + (i.ToRotationVector2() * 500);
                if (Main.netMode != 1)
                {
                    if (Main.netMode != 1)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_Death(), center, (Projectile.position - center).SafeNormalize(Vector2.One) * 20,
                            Bullet, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition,
                null, new Color(255, 100, 100, 0), Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2,
                1f, 0, 0f);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage = 0;
            target.immune = true;
            Projectile.Kill();
        }
    }
}
