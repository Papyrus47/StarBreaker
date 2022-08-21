using StarBreaker.Items;
using System.IO;

namespace StarBreaker.Projs
{
    internal class StarBreakerHeadProjReal : ModProjectile
    {
        private float State
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private float Timer2
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public override string Texture => "StarBreaker/Items/Weapon/StarBreakerW";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰击碎者");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = 40;
            Projectile.width = 142;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            
        }
    }
}
