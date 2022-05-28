using Terraria.GameContent.Creative;

namespace StarBreaker.Items.ItemTiles
{
    public class StarHardRock : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Hard Rock");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "星辰硬石");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.StarHardRock>();
            Item.width = 12;
            Item.height = 12;
            Item.value = 3000;
        }
    }
}
