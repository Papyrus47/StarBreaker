using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.Bullets
{
    public class MightBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("力量子弹");
        }
        public override void NewSetDef()
        {
            projColor = new(100, 100, 255);
        }
        public override void StateAI()
        {

        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            target.AddBuff(ModContent.BuffType<Buffs.EnergySmash>(), 210);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);
            target.AddBuff(ModContent.BuffType<Buffs.EnergySmash>(), 210);
        }
    }
}
