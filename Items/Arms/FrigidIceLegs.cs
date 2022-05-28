namespace StarBreaker.Items.Arms
{
    [AutoloadEquip(EquipType.Legs)]
    public class FrigidIceLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("极寒之冰");
            Tooltip.SetDefault("用魔法凝聚的冰腿\n" +
                "为你的翅膀额外提供2秒的飞行时间");
        }
        public override void SetDefaults()
        {
            Item.defense = 12;
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void UpdateEquip(Player player)
        {
            player.wingTimeMax += 120;
        }
    }
}
