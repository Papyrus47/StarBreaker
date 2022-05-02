using Microsoft.Xna.Framework;
using Terraria;

namespace StarBreaker.Projs.StarDoomStaff
{
    public class StarBoomCrystal : StarCrystal
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("爆炸水晶");
        }
        public override void AI()
        {
            if (Main.player[Projectile.owner].HasMinionAttackTargetNPC && Main.player[Projectile.owner].slotsMinions < 0.05f)
            {
                Projectile.tileCollide = true;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.timeLeft = 30;
                    Projectile.ai[1]++;
                    Projectile.velocity = (Projectile.OwnerMinionAttackTargetNPC.position - Projectile.position) / 10;
                }
                else
                {
                    Projectile.extraUpdates = 2;
                }
            }
            else
            {
                base.AI();
            }
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] < 2 && Main.player[Projectile.owner].HasMinionAttackTargetNPC)
            {
                Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                    Main.player[Projectile.owner].Center, Projectile.velocity.SafeNormalize(default) * 20, Type,
                    Projectile.damage, Projectile.knockBack, Projectile.owner, ++Projectile.ai[0], --Projectile.ai[1])].originalDamage = Projectile.damage;
            }
        }
    }
}
