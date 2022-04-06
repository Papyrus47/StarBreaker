using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


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
                        for (int i = 0; i <= 5; i++)
                        {
                            Tile tile = Main.tile[(int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + i];
                            if (tile == null || !tile.HasTile)
                            {
                                continue;
                            }
                            Projectile.NewProjectile(null, Projectile.position + new Vector2(0, (i - 1) * 16), Vector2.Zero, ModContent.ProjectileType<CopperSawHead>(),
                                Projectile.damage, Projectile.knockBack, Projectile.owner, 0, (Main.MouseWorld - Projectile.Center).X);
                            break;
                        }
                    }
                }
            }
        }
    }
}
