namespace StarBreaker.Projs.UltimateCopperShortsword
{
    internal class LostSwordLaser2 : ModProjectile
    {
        public override string Texture => "StarBreaker/Projs/UltimateCopperShortsword/LostSword2";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Copper Laser");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铜激光");
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            if (Projectile.ai[0] == 0)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                        Projectile.Center,
                        Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 2000,
                        5, ref r);
            }
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            if (Projectile.ai[0] == 1)
            {
                Texture2D texture = StarBreakerAssetTexture.MyExtras[8].Value;
                for (int i = 0; i < 2000; i += 512)
                {
                    Main.spriteBatch.Draw(texture,
                        (Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * i) - Main.screenPosition,
                        null,
                        new Color(0, 255, 0, 100),
                        Projectile.rotation - MathHelper.PiOver4,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0);
                }
            }
            else if (Projectile.ai[0] == 0)
            {
                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
                for (int i = 0; i < 2000; i += 32)
                {
                    Main.spriteBatch.Draw(texture,
                        (Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * i) - Main.screenPosition,
                        null,
                        Color.White,
                        Projectile.rotation,
                        texture.Size() * 0.5f,
                        1,
                        SpriteEffects.None,
                        0);
                }
            }
        }
    }
}
