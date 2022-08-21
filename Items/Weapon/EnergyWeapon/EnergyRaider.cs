using StarBreaker.Items.Type;

namespace StarBreaker.Items.Weapon.EnergyWeapon
{
    public class EnergyRaider : BaseEnergyRanged
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("能源突击者");
            Tooltip.SetDefault("一般的爱好使用能量枪系列的人使用的武器");
        }
        public override void NewSetDef()
        {
            base.NewSetDef();
            Item.width = 72;
            Item.height = 34;
            Item.damage = 8;
            Item.knockBack = 1.1f;
            Item.useTime = Item.useAnimation = 3;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.crit = 11;
            Item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.IronBar, 10).AddIngredient(ItemID.StarCannon).AddTile(TileID.MythrilAnvil).Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-30, 5);
        }
    }
}
