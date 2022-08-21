namespace StarBreaker.Items.Weapon.IceGunAndFireKnife
{
    public class FireKnife_Blade : ModProjectile
    {
        private Projectile proj = null;
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "炎刀");
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.Size = new(14, 90);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent entity && entity.Entity is Projectile p)
            {
                proj = p;
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.HeldItem.type != ModContent.ItemType<IceGunAndFireKnife>())
            {
                Projectile.Kill();
                return;
            }
            else if (proj != null && proj.ModProjectile is FireKnife)
            {
                if (proj.localAI[0] == 1)
                {
                    proj.localAI[0] = 2;
                    int pro = Projectile.NewProjectile(proj.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<FireKnife_Blade>(),
                        Projectile.damage, Projectile.knockBack, player.whoAmI);
                    Main.projectile[pro].localAI[0] = 1;
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.timeLeft = 2;
                int dir = (Projectile.localAI[0] == 0).ToDirectionInt();
                Projectile.Center = proj.Center + proj.velocity.RealSafeNormalize().RotatedBy(0.1 * dir) * (Projectile.height / 2 * Projectile.scale * proj.scale);//控制位置
                Projectile.velocity = proj.velocity.RotatedBy(proj.localAI[1] * dir);

            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                proj.Center, Projectile.Center + Projectile.velocity.RealSafeNormalize() * (Projectile.height / 2 * Projectile.scale),
                Projectile.width * Projectile.scale, ref r);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle rectangle = new Rectangle(0, 0, 14, texture.Height);
            if (Projectile.localAI[0] != 0)
            {
                rectangle.X = 24;
            }
            if (proj.localAI[1] != 0)
            {
                Main.spriteBatch.Draw(texture, proj.Center - Main.screenPosition, rectangle, lightColor,
                    Projectile.rotation, new Vector2(rectangle.Width / 2, rectangle.Height), Projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, rectangle, lightColor,
                    Projectile.rotation, rectangle.Size() * 0.5f / Projectile.scale, Projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            }
            return false;
        }
    }
}
