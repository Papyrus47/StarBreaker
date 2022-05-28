namespace StarBreaker.Projs.IceGun
{
    public class BigIcePick : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("冰锥");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 120;
            Projectile.width = 40;
            Projectile.height = 126;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.position -= Projectile.velocity;//忘记写那个了
            if (Projectile.Opacity < 1)
            {
                Projectile.Opacity += 0.05f;
            }

            if (Projectile.scale < Projectile.ai[0])
            {
                Projectile.scale += 0.1f;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float s = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + Projectile.velocity.RealSafeNormalize() * 63 * Projectile.scale,
                Projectile.Center + Projectile.velocity.RealSafeNormalize() * -63 * Projectile.scale,
                Projectile.scale * 35, ref s);
        }
    }
}
