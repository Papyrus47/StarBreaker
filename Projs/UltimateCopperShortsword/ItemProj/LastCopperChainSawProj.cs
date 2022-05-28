namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperChainSawProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜链锯");
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = 20;
            Projectile.width = 16;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void PostAI()
        {
            Projectile.ai[0]++;
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] > 5)
                {
                    Projectile.ai[0] = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(null, Projectile.Center, Projectile.velocity.RealSafeNormalize() * 10, ModContent.ProjectileType<CopperSawHead>(),
                            Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }
    }
}
