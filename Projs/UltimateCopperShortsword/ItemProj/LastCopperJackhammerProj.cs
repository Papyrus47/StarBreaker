using Terraria.ID;

namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperJackhammerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜手提钻");
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = 20;
            Projectile.width = 22;
            Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void PostAI()
        {
            Projectile.ai[0]++;
            Projectile.frameCounter++;
            if(Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4) Projectile.frame = 0;
            }
            if (Projectile.ai[0] > 120)
            {
                Projectile.ai[0] = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(null, Projectile.Center, Projectile.velocity.SafeNormalize(default) * 15, ModContent.ProjectileType<CopperJackhammerHead>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }
}
