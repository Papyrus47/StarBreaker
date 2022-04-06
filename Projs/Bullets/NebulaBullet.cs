using StarBreaker.Projs.Type;
using Terraria;

namespace StarBreaker.Projs.Bullets
{
    class NebulaBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("四柱子弹");
        }
        public override void StateAI()
        {
            Projectile.velocity *= 1.01f;
        }
        public override void NewSetDef()
        {
            projColor = new(1, 0.3f, 1f);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            target.velocity += -target.velocity * 0.05f;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);
            target.velocity = -target.velocity;
        }
    }
}
