namespace StarBreaker.Projs
{
    public class BeamLight : ModProjectile
    {
        public override string Texture => "StarBreaker/Projs/Star";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("光柱");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            //Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.1f;
            Projectile.velocity.Y = Projectile.ai[0];
            Projectile.velocity.X *= 1.02f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.ai[1] == 1)
            {
                target.KillMe(PlayerDeathReason.ByCustomReason(target.name + "死于鬼之柱"), 10, 10);
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == ModContent.NPCType<NPCs.StarGhostKnife>())
            {
                return;
            }

            if (Projectile.ai[1] == 1)
            {
                target.life = 0;
                target.checkDead();
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + Projectile.rotation.ToRotationVector2() * 100,
                Projectile.Center + Projectile.rotation.ToRotationVector2() * -100,
                5, ref r);
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(tex,
               Projectile.Center - Main.screenPosition,
               null,
               new Color(0, 0.6f, 1f, 0f),
               Projectile.rotation,
               tex.Size() / 2,
               new Vector2(1.2f, 5),
               SpriteEffects.None,
               0f);
        }
    }
}
