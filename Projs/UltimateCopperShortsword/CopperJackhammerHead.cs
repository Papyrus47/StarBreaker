namespace StarBreaker.Projs.UltimateCopperShortsword
{
    public class CopperJackhammerHead : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜手提钻");
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 22;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity.Y += 0.1f;
        }
        public override void Kill(int timeLeft)
        {
            const int Const = 15;
            for (int i = -Const; i <= Const; i++)
            {
                for (int j = -Const; j <= Const; j++)
                {
                    int posX = (int)(Projectile.Center.X) / 16;
                    int posY = (int)(Projectile.Center.Y) / 16;
                    Dust.NewDust(new Vector2(posX + i, posY + j) * 16, 1, 1, DustID.Torch);
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
    }
}
