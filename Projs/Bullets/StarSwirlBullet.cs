using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.Bullets
{
    internal class StarSwirlBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("四柱子弹");
        }
        public override void StateAI()
        {
            if (Projectile.timeLeft % 50 == 0 && Projectile.timeLeft < 300)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(90)) * 1.05f;
                Projectile.damage += Projectile.damage / 5;
            }
        }
        public override void NewSetDef()
        {
            projColor = Color.LightGreen;
        }
    }
}
