using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperSpearProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜矛");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 200;
            Projectile.height = Projectile.width = 54;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 pointOnPlayerHead = player.RotatedRelativePoint(player.MountedCenter, true);
            player.heldProj = Projectile.whoAmI;
            Projectile.position = pointOnPlayerHead - new Vector2(Projectile.width / 2, Projectile.height / 2);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.PiOver2;
            player.itemRotation = Projectile.rotation;
            Projectile.velocity = Projectile.velocity.SafeNormalize(default) * (Projectile.ai[1] + 1);
            switch (Projectile.ai[0])
            {
                case 0://前进状态
                    {
                        Projectile.ai[1]++;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            for (int i = -1; i <= 1; i++)
                            {
                                Vector2 vel = Projectile.velocity.RotatedBy(i * MathHelper.Pi / 3);
                                Projectile.NewProjectile(null, Projectile.Center, vel, ModContent.ProjectileType<FlySpearProj>(),
                                    (int)System.Math.Sqrt(Projectile.damage), Projectile.knockBack, Main.myPlayer);
                            }
                        }
                        if (Projectile.ai[1] > 20)
                        {
                            Projectile.ai[0]++;
                        }
                        break;
                    }
                case 1://回归状态
                    {
                        Projectile.ai[1]--;
                        if (Projectile.ai[1] < 0)
                        {
                            Projectile.Kill();
                        }
                        break;
                    }
            }
            Projectile.position += Projectile.velocity;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 1;
            return false;
        }
    }
}
