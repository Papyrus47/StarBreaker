namespace StarBreaker.Projs.Type
{
    public abstract class BaseThistle : ModProjectile
    {
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center - (Projectile.rotation.ToRotationVector2() * Projectile.width / 2f * Projectile.scale),
                Projectile.Center + (Projectile.rotation.ToRotationVector2() * Projectile.width / 2f * Projectile.scale),
                Projectile.height, ref r);
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
        public override bool PreDraw(ref Color lightColor)//抄的源码,不想再写
        {
            Texture2D value33 = TextureAssets.Projectile[Type].Value;
            Rectangle value34 = value33.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin11 = new Vector2(16f, value34.Height / 2);
            Color alpha4 = Projectile.GetAlpha(lightColor);
            Vector2 scale8 = new Vector2(Projectile.scale);
            float lerpValue4 = Utils.GetLerpValue(30f, 25f, Projectile.ai[0], clamped: true);
            scale8.Y *= lerpValue4;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            Main.EntitySpriteDraw(value33, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), value34, alpha4, Projectile.rotation, origin11, scale8, spriteEffects, 0);
            return false;
        }
    }
}
