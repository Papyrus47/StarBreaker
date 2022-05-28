namespace StarBreaker.Projs
{
    internal class StarLine : ModProjectile
    {
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰激光");
        }
        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 DrawOrigin = Vector2.Zero;
            Vector2 center = Projectile.position + (new Vector2(Projectile.width, Projectile.height) / 2) - Main.screenPosition;
            center -= new Vector2(tex.Width, tex.Height) * Projectile.scale / 2f;
            center += DrawOrigin * Projectile.scale + new Vector2(0f, 4f + Projectile.gfxOffY);
            Main.spriteBatch.Draw(tex,
                center,
                null,
                Color.Blue * 0.8f,
                Projectile.rotation,
                DrawOrigin,
                new Vector2(100, 1),
                Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f);
            return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + (Projectile.rotation.ToRotationVector2()),
                Projectile.Center + (Projectile.rotation.ToRotationVector2() * 1000),
                2, ref r);
        }
    }
}
