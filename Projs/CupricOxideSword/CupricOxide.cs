namespace StarBreaker.Projs.CupricOxideSword
{
    public class CupricOxide : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("氧化铜碎片");
            Main.projFrames[Type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.frame = Main.rand.Next(6);
                Projectile.ai[0]++;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
