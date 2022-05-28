using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.Bullets
{
    public class IceEnergyBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("冰霜能量弹");
        }
        public override void NewSetDef()
        {
            projColor = new Color(100, 20, 255);
        }
        public override void StateAI()
        {
            if (Projectile.velocity.Length() < 30f)
            {
                Projectile.velocity += Projectile.velocity.RealSafeNormalize();
            }
        }
    }
}
