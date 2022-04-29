using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon.NoHardMode
{
    public class StoneSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stone sword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "石剑");
            Tooltip.SetDefault("emm...This sword is \" HARD \"");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "唔...这剑是很\" 硬 \"");
        }
        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 40;
            Item.damage = 8;
            Item.knockBack = 6.3f;
            Item.autoReuse = false;
            Item.useTurn = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 230;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.StoneBlock, 30).AddTile(TileID.WorkBenches).Register();
    }
}
