using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.Bullets
{
    public class DarkBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("黑暗子弹");
        }
        public override void NewSetDef()
        {
            projColor = Color.MediumPurple;
        }
        public override void StateAI()
        {
            Projectile.velocity *= 1.01f;
            Projectile.damage++;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            target.AddBuff(BuffID.Ichor, 180);
            target.AddBuff(BuffID.CursedInferno, 180);
        }
    }
}
