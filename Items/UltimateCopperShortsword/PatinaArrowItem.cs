using StarBreaker.Projs.UltimateCopperShortsword;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class PatinaArrowItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜箭");
            Tooltip.SetDefault("小心保管这种质地软的箭");
        }
        public override void SetDefaults()
        {
            Item.ammo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<PatinaArrow>();
            Item.shootSpeed = 2f;
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 0.3f;
            Item.maxStack = 999;
            Item.value = 90;
            Item.consumable = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(50).AddIngredient(ItemID.CopperBar).AddTile(TileID.Furnaces).Register();
        }
    }
}
