using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Projs.Bosses
{
    public class StarSpiralBladeProj_Hostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰旋刃分身");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 500;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 80;
        }
        public override void AI()
        {
            Projectile.rotation += Main.GlobalTimeWrappedHourly;
            if (Projectile.rotation > 31415)
            {
                Projectile.rotation = 0;
            }
            switch (Projectile.ai[0])
            {
                case 1://存活时间一半后回旋
                    {
                        if (Projectile.timeLeft == 250)
                        {
                            Projectile.velocity = -Projectile.velocity;
                        }
                        break;
                    }
                case 2://受重力影响
                    {
                        Projectile.velocity.Y += 0.3f;
                        break;
                    }

                case 3://血牙那种
                    {
                        Projectile.velocity = Projectile.velocity.RotatedBy(0.01 * Projectile.ai[1]);
                        break;
                    }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.Purple;
            return base.PreDraw(ref lightColor);
        }
    }
}
