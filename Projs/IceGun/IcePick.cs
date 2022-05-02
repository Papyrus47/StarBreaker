using Microsoft.Xna.Framework;
using StarBreaker.Projs.Bullets;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace StarBreaker.Projs.IceGun
{
    public class IcePick : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("冰锥");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 500;
            Projectile.width = 82;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 30; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Unit();
                    Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + (vel * 100), vel * 1.5f, ModContent.ProjectileType<BigIcePick>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner,Main.rand.NextFloat(1f,2f));
                    projectile.Opacity = 0;
                    projectile.scale = 0;
                }
            }
        }
    }
}
