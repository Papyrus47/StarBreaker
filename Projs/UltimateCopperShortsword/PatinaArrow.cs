namespace StarBreaker.Projs.UltimateCopperShortsword
{
    public class PatinaArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜箭");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.width = 14;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.arrow = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft < 100)
            {
                if (Projectile.velocity.Y * Projectile.velocity.Y < 400)
                {
                    Projectile.velocity.Y++;
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 16,
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * -16, 3, ref r);
        }
    }
}
