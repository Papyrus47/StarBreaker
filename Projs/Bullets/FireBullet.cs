using Microsoft.Xna.Framework;
using StarBreaker.Projs.Type;
using Terraria;
using Terraria.ID;

namespace StarBreaker.Projs.Bullets
{
    public class FireBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("火焰子弹");
        }
        public override void NewSetDef()
        {
            projColor = Color.Red;
        }
        public override void StateAI()
        {
            Projectile.velocity *= 1.1f;
            if (Projectile.timeLeft % 10 == 0 && Projectile.timeLeft >= 280 && Projectile.ai[0] == 0)
            {
                int proj = Projectile.NewProjectile(null, Projectile.position, Vector2.Zero, Type, Projectile.damage, Projectile.knockBack, Main.myPlayer, 1, Projectile.ai[1]);
                Main.projectile[proj].width = Main.projectile[proj].height = 5;
                Main.projectile[proj].friendly = true;
                Main.projectile[proj].hostile = false;
                Main.projectile[proj].timeLeft = 30;
            }
            if (Projectile.ai[0] == 1)
            {
                Dust.NewDust(Projectile.position, 5, 5, DustID.Torch);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 1)
            {
                return false;
            }
            return base.PreDraw(ref lightColor);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] == 1)
            {
                return projHitbox.Center().Distance(targetHitbox.Center()) < targetHitbox.Width;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
    }
}
