namespace StarBreaker.Projs
{
    public class GhostSlash : ModProjectile
    {
        public Vector2 StartingPoint;
        public Vector2 EndPoint;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("鬼影斩");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            //Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void AI()
        {
            float X = (StartingPoint.X + EndPoint.X) / 2;
            float Y = (StartingPoint.Y + EndPoint.Y) / 2;
            Projectile.Center = new Vector2(X, Y);
            Projectile.velocity *= 0;
            Projectile.ai[0]++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] < 75)
            {
                return false;
            }

            float r = 0;
            bool b1 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                new Vector2(StartingPoint.X, StartingPoint.Y + 50), new Vector2(EndPoint.X, EndPoint.Y - 50), 10, ref r);
            bool b2 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                new Vector2(StartingPoint.X, StartingPoint.Y - 50), new Vector2(EndPoint.X, EndPoint.Y + 50), 10, ref r);
            return b1 || b2;
        }
        //public override void PostDraw(Color lightColor)
        //{
        //    int b = Math.Min((int)Projectile.ai[0] * 2, 150);
        //    int r = Math.Min(((int)Projectile.ai[0] - 20) * 2, 110);
        //    Texture2D texture2D = ModContent.Request<Texture2D>("StarBreaker/Projs/GhostSlash").Value;
        //    Main.spriteBatch.Draw(
        //        texture2D,
        //        Projectile.Center + new Vector2(50, 1.5f) - Main.screenPosition,
        //                    null,
        //                    new Color(r, 0, b, 0),
        //                    Projectile.rotation,
        //                    texture2D.Size() / 2f,
        //                    new Vector2(10, 1.5f),
        //                    SpriteEffects.None,
        //                    0f
        //                    );
        //}
    }
}
