namespace StarBreaker.Projs.UltimateCopperShortsword
{
    public class CopperDrillBit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜飞钻");
        }
        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 18;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            oldVelocity.Normalize();
            int posX = (int)(Projectile.Center.X + oldVelocity.X * 16f) / 16;
            int posY = (int)(Projectile.Center.Y + oldVelocity.Y * 16f) / 16;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Tile tile = Main.tile[posX + i, posY + j];
                    if (tile == null)
                    {
                        Projectile.Kill();
                        return true;
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
            return true;
        }
    }
}