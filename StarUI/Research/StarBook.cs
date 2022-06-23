namespace StarBreaker.StarUI.Research
{
    public class StarBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星书");
        }
        public override void SetDefaults()
        {
            Item.Size = new(10);
            Item.useTime = 1;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override bool? UseItem(Player player)
        {
            return base.UseItem(player);
        }
    }
}
