using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Projs.Waste
{
    public class StarBroadSwordProj : ModProjectile
    {
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰阔剑");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 50;
            Projectile.noDropItem = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.width = 52;
            Projectile.height = 52;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center - Projectile.velocity.SafeNormalize(default) * 52;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            Timer++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.position - Projectile.velocity, Projectile.position + Projectile.velocity, 20, ref r);
        }
    }
}
