using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperSickleProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜镰刀");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 10;
            Projectile.Size = new Vector2(52, 42);
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.localNPCHitCooldown = 1;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += 0.3f;
            if (Projectile.rotation > 6.28f) Projectile.rotation = 0;
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            if (!player.channel)
            {
                Projectile.velocity *= 1.1f;
            }
            else
            {
                if (!player.CCed)
                {
                    Projectile.timeLeft = 60;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.velocity = (Projectile.velocity * 20 + (Main.MouseWorld - Projectile.Center) * 0.1f) / 21;
                    }
                    player.itemRotation = (player.position - Projectile.position).ToRotation();
                    player.ChangeDir(Projectile.direction);
                    player.itemTime = player.itemAnimation = 2;
                    player.heldProj = Projectile.whoAmI;
                }
            }
        }
    }
}
