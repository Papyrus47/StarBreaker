using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.Bullets
{
    public class SightBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("视域子弹");
        }
        public override void NewSetDef()
        {
            projColor = Color.GreenYellow;
            Projectile.tileCollide = false;
        }
        public override void StateAI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy((double)(300 - Projectile.timeLeft) / 300 * 0.2);
            if (Projectile.timeLeft % 100 == 0 && Projectile.timeLeft < 250)
            {
                Projectile.damage *= 2;
            }
        }

    }
}

