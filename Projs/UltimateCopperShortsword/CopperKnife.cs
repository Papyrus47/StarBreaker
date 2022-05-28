namespace StarBreaker.Projs.UltimateCopperShortsword
{
    public class CopperKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜飞刀");
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.width = 10;
            Projectile.height = 24;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            switch (Projectile.localAI[1])
            {
                case 0://正常下落
                    {
                        Projectile.velocity.Y += 0.01f;
                        break;
                    }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 12,
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * -12, 5, ref r);
        }
    }
}
