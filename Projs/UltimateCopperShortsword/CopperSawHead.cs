namespace StarBreaker.Projs.UltimateCopperShortsword
{
    public class CopperSawHead : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜链锯头");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 16;
            Projectile.height = 34;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft % 5 == 0 && Projectile.velocity.Length() > 0.2f)
            {
                Projectile.velocity *= 0.8f;//减速
            }
            else if (Projectile.velocity.Length() < 0.2f)
            {
                Projectile.velocity *= 2f;
            }//避免速度太慢
            int posX = (int)(Projectile.Center.X) / 16;
            int posY = (int)(Projectile.Center.Y) / 16;
            Tile tile = Main.tile[posX, posY];
            if (tile.HasTile && Main.tileAxe[tile.TileType])
            {
                new Player().PickTile((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), 110);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.9f;
            return false;
        }
    }
}
