namespace StarBreaker.Projs.StarDoomStaff.Boss
{
    public class StarBoomCrystal_Boss : ModProjectile
    {
        public override string Texture => (GetType().Namespace + ".StarCrystal").Replace('.', '/');
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("爆炸水晶");
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.hostile = true;
            Projectile.width = 26;
            Projectile.height = 24;
            Projectile.localNPCHitCooldown = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            StarBreakerWay.ProjDrawTail(Projectile, lightColor * 0.5f, lightColor * 0.1f);
            return base.PreDraw(ref lightColor);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
    }
}
