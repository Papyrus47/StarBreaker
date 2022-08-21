namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class CopperHammerGas : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("震锤气波");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.timeLeft = 100;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.extraUpdates = 10;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 1)
            {
                Tile tile = Main.tile[(int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 - 1];
                if (tile == default(Tile) || !tile.HasTile)
                {
                    Projectile.ai[0] = 0;
                }
                Projectile.velocity.Y = 0;
                Projectile.velocity.X = Projectile.ai[1];
                for (int i = -1; i <= 1; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }

                    Tile tileX = Main.tile[(int)Projectile.Center.X / 16 + i, (int)Projectile.Center.Y / 16];
                    if (tileX != null && tileX.HasTile)
                    {
                        Projectile.position.Y -= 16;
                        Projectile.position.X += Projectile.velocity.X;
                    }
                }
                for (int i = 0; i <= 20; i++)
                {
                    Tile tileY = Main.tile[(int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 - i];
                    if (tileY == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (tileY.WallType > 0)
                        {
                            WorldGen.KillWall((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 - i, false);
                        }
                    }
                }
                if (Main.rand.Next(10) == 0)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, 2, 2, DustID.Stone)];
                    dust.scale = Main.rand.NextFloat(0.5f, 1.5f);
                    dust.velocity = new Vector2(0, -5).RotatedByRandom(0.3f);
                    dust.velocity.X += Main.rand.NextFloat(-0.5f, 0.5f);
                }
            }
            else
            {
                Projectile.velocity.Y++;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 1;
            return false;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Height = 110;
            hitbox.Width = 10;
            hitbox.X -= 5;
            hitbox.Y -= 110;
        }
    }
}
