namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperDiamondProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜钻");
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = 20;
            Projectile.width = 22;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void PostAI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 5)
            {
                Projectile.ai[0] = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(null, Projectile.Center, Projectile.velocity.SafeNormalize(default) * 10, ModContent.ProjectileType<CopperDrillBit>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }
}
