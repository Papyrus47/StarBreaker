using StarBreaker.Projs.Bullets;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Bullet
{
    public class StardustBulletItem : EnergyBulletItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星尘子弹");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.crit = 10;
            Item.damage = 11;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<StardustBullet>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(50).AddIngredient(ItemID.FragmentStardust).AddTile(412).Register();
        }

        public override void ProjAI(Projectile Projectile)
        {
            Projectile.velocity.Y += 0.05f;
        }
        public override void Kill(Projectile Projectile)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.ai[0] == 0 && !Projectile.minion)
            {
                for (int i = -1; i <= 1; i++)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(i * 0.1f) * -1,
                        Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                    Main.projectile[proj].friendly = true;
                    Main.projectile[proj].hostile = false;
                }
            }
        }
    }
}
