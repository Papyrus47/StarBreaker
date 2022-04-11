using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.ItemTiles
{
    public class InfiniteWood : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Wood}";//这里是一个表达式,获取原版贴图
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infinite Wood");
            DisplayName.AddTranslation(7, "无限木头");
            Tooltip.SetDefault("Yes! This is unlimited wood and you can use it whenever you want!\n" +
                "It's very fast.");
            Tooltip.AddTranslation(7, "是的!这块木头是无限使用的,只要你想用!\n" +
                "这真的很快");
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 3;
            Item.useTime = 3;
            Item.autoReuse = true;
            Item.createTile = TileID.WoodBlock;
            Item.width = 12;
            Item.height = 12;
            Item.value = 0;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.Wood, 999).AddTile(TileID.WorkBenches).Register();
    }
}
