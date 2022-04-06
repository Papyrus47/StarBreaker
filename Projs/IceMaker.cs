using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class IceMaker : ModProjectile
    {
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("冰柱");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.damage = 0;
            Projectile.width = Projectile.height = 10;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            StarPlayer StarPlayer = player.GetModPlayer<StarPlayer>();
            Projectile.velocity *= 0.9f;
            if (!StarPlayer.InIdeaDriven)
            {
                StarPlayer.InIdeaDriven = true;
            }
            if (player.HeldItem.type != ModContent.ItemType<Items.Weapon.FrostFistW>())
            {
                Projectile.Kill();
            }
        }
        public override bool? CanDamage()
        {
            return false;
        }
    }
}
