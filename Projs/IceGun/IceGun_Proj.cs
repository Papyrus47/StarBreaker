namespace StarBreaker.Projs.IceGun
{
    public class IceGun_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("FrostStar Boom Gun");
            DisplayName.AddTranslation(7, "霜星轰击者");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 5;
            Projectile.friendly = true;
            Projectile.Size = new(104, 36);
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if(player.channel)
            {
                Projectile.timeLeft = 2;
            }
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
            player.itemTime = player.itemAnimation = 2;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            if (Projectile.spriteDirection == -1) Projectile.rotation += MathHelper.Pi;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemRotation = StarBreakerWay.Vector2ToFloat_Atan2(Projectile.velocity,Projectile.direction);
            if(Main.myPlayer == player.whoAmI)
            {
                Projectile.velocity = (Main.MouseWorld - player.Center).RealSafeNormalize() * 20f;
            }
            if (Projectile.ai[1] < 360) Projectile.ai[1]++;
            if(Projectile.ai[1] % 2 == 0)
            {
                StarBreakerWay.NewDustByYouself(Projectile.Center, DustID.Ice, () => true,Vector2.UnitX, 50, (int)Projectile.ai[1] / 10,
                    (dust) =>
                    {
                        dust.noGravity = true;
                        dust.noLight = false;
                        dust.velocity *= 0;
                    });
            }
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.active)
            {
                player.PickAmmo(player.HeldItem, out int shootID, out _, out _, out float kn, out _);
                if (Projectile.ai[1] >= 360)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<IcePick>(), Projectile.damage, kn, Projectile.owner);
                }
                else
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 2, shootID, (int)(Projectile.damage * (Projectile.ai[1] / 120f)), kn, Projectile.owner);
                    proj.friendly = true;
                    proj.hostile = false;
                }
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 origin = new Vector2(32, 15);
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                origin.X += 40;
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition,
                new Rectangle(0,0,texture.Width,texture.Height / 2 - 1), lightColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
