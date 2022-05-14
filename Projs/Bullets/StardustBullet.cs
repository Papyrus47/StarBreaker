using Microsoft.Xna.Framework;
using StarBreaker.Projs.Type;
using Terraria;

namespace StarBreaker.Projs.Bullets
{
    class StardustBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("四柱子弹");
        }
        public override void StateAI()
        {
            Projectile.velocity *= 1.01f;
            Projectile.velocity.Y += 0.05f;
            if (Projectile.timeLeft % 5 == 0)
            {
                Projectile.damage++;
            }
        }
        public override void NewSetDef()
        {
            projColor = Color.Cyan;
        }

    }
}
