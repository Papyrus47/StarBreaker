namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class FlyPick : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("飞稿");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 2000;
            Projectile.width = Projectile.height = 32;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 5;
            Projectile.extraUpdates = 5;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Projectile.rotation += 0.85f;
            Projectile.velocity.Y += 0.3f;
            if (Projectile.rotation > 6.28f)
            {
                Projectile.rotation = 0;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X = -oldVelocity.X;
            Projectile.velocity.Y -= 0.5f;
            Projectile.timeLeft -= 30;
            oldVelocity.Normalize();
            for (int i = -3; i <= 3; i++)
            {
                for (int j = -3; j <= 3; j++)
                {
                    int posX = (int)(Projectile.Center.X + oldVelocity.X * 16f) / 16;
                    int posY = (int)(Projectile.Center.Y + oldVelocity.Y * 16f) / 16;
                    if (posX + i > 0 && posX + i < Main.maxTilesX)
                    {
                        if (posY + j > 0 && posY + j < Main.maxTilesY)
                        {
                            Tile tile = Main.tile[posX + i, posY + j];
                            if (tile == null || Main.tileSolidTop[tile.TileType])
                            {
                                continue;
                            }
                            else
                            {
                                if (tile.HasTile)
                                {
                                    if (Main.myPlayer == Projectile.owner)
                                    {
                                        new Player().PickTile(posX + i, posY + j, 110);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
