using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.Bullets
{
    public class FrightBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("恐惧子弹");
        }
        public override void NewSetDef()
        {
            projColor = new(255, 100, 100);
        }
        public override void StateAI()
        {
            Projectile.extraUpdates = 3;
            if (Projectile.timeLeft % 10 == 0)
            {
                Projectile.damage++;
            }
        }
    }
}
