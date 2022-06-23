namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperHammerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜锤");
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(42);
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = ProjAIStyleID.Boomerang;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.timeLeft = 430;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = -1;
            AIType = 301;
        }
        public override void AI()
        {
            for (int i = -3; i <= 3; i++)
            {
                for (int j = -3; j <= 3; j++)
                {
                    int posX = (int)(Projectile.Center.X) / 16;
                    int posY = (int)(Projectile.Center.Y)/ 16;
                    if (posX + i > 0 && posX + i < Main.maxTilesX)
                    {
                        if (posY + j > 0 && posY + j < Main.maxTilesY)
                        {
                            Tile tile = Main.tile[posX + i, posY + j];
                            if (tile == default(Tile))
                            {
                                continue;
                            }
                            else if (tile.WallType > 0 && !tile.HasTile && !tile.HasUnactuatedTile)
                            {
                                WorldGen.KillWall(posX + i, posY + j);
                            }
                        }
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            StarBreakerWay.DrawTailTexInPos(TextureAssets.Projectile[Type].Value,Projectile.oldPos,
                lightColor,lightColor* 0.3f,Projectile.rotation,Projectile.spriteDirection,Vector2.Zero,
                null,null,true);
            return false;
        }
    }
}
