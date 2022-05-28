using StarBreaker.Projs.Bullets;
using Terraria.GameContent.Creative;

namespace StarBreaker.Items.Bullet
{
    public class SolarBulletItem : EnergyBulletItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("日耀子弹");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.crit = 10;
            Item.damage = 11;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<SunBullet>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(50).AddIngredient(ItemID.FragmentSolar).AddTile(412).Register();
        }

        public override void ProjAI(Projectile Projectile)
        {
            Projectile.extraUpdates = 3;
            Projectile.damage++;
        }
    }
}
