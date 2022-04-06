using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

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
            Projectile.timeLeft = 10;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            Projectile.scale -= 0.05f;
            Projectile.Opacity = Projectile.scale;
            if (Projectile.scale <= 0)
            {
                Projectile.Kill();
            }
            Tile tile = Main.tile[(int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16];
            if (tile != null)
            {
                if (Main.tileAxe[tile.TileType])
                {
                    Projectile.ai[0] += 0.1f;
                    new Player().PickTile((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), 110);
                }
                else
                {
                    tile = Main.tile[(int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 - 1];
                    if (Main.tileSolidTop[tile.TileType] || Main.tileSolid[tile.TileType])
                    {
                        Projectile.velocity.Y++;
                    }
                    else
                    {
                        Projectile.velocity.Y = 0;
                    }
                }
            }
            else
            {
                Projectile.velocity.Y++;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override void Kill(int timeLeft)
        {
            int dis = Projectile.ai[1] > 0 ? 1 : -1;
            if (Projectile.ai[0] < 15)
            {
                Projectile.ai[0]++;
                Projectile.NewProjectile(null, Projectile.Center + new Vector2(Projectile.width * dis, 0), Vector2.Zero,
                    ModContent.ProjectileType<CopperSawHead>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], Projectile.ai[1]);
            }
        }
    }
}
