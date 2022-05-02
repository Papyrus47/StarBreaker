using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs.Bosses.StarBreakerEX
{
    public class StarRocket_Hostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rocket");
            DisplayName.AddTranslation(7, "导弹");
        }
        public override void SetDefaults()
        {
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 500;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.scale == 2f)
            {
                Projectile.extraUpdates = 2;
                Player player = Main.player[(int)Projectile.ai[1]];
                Vector2 center = player.Center + new Vector2(0, -200);
                if(Vector2.Distance(center,Projectile.Center) < 50)
                {
                    Projectile.Kill();
                }
                else
                {
                    Projectile.velocity = (Projectile.velocity * 10 + (center - Projectile.Center).RealSafeNormalize() * 5) / 11;
                }
            }
            if(Projectile.ai[0] == 1f)
            {
                Projectile.extraUpdates = 2;
                Projectile.velocity.Y += 0.05f;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Vector2.Distance(Projectile.Center,targetHitbox.TopLeft()) < 30;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Projectile.Center, 3, 3, DustID.Smoke);
            }
            if (Projectile.scale == 2f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for(int i = -3;i<=3;i++)
                {
                    Vector2 vel = Vector2.UnitY * 5;
                    vel = vel.RotatedBy(MathHelper.Pi / 18 * i);
                    _ = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, Type, Projectile.damage, Projectile.knockBack, Projectile.owner,1);
                }
            }
        }
    }
}
