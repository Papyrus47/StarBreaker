using StarBreaker.Items.Bullet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class EnergyConverterProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("能量转换器");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1;
            ProjectileID.Sets.YoyosTopSpeed[Type] = 10;
            ProjectileID.Sets.YoyosMaximumRange[Type] = 10000;
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 3000;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 16;
        }
        public override void PostAI()
        {
            Item.NewItem(Projectile.GetItemSource_FromThis(), Main.player[Projectile.owner].Hitbox, Main.rand.Next(new int[] {ModContent.ItemType<NebulaBulletItem>(),
                ModContent.ItemType<SolarBulletItem>(),
                ModContent.ItemType<StardustBulletItem>(),
                ModContent.ItemType<VortexBulletItem>()}));
            Main.player[Projectile.owner].statMana -= 2;
            if (Main.player[Projectile.owner].statMana < 2)
            {
                Projectile.Kill();
            }
        }
    }
}
