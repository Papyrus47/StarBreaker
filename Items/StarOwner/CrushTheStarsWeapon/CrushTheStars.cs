namespace StarBreaker.Items.StarOwner.CrushTheStarsWeapon
{
    public class CrushTheStars : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crush The Stars");
            //DisplayName.AddTranslation(7, "破星连斩");
            // Tooltip.SetDefault("测试挥舞");
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.width = 66;
            Item.height = 25;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<CrushTheStarsProj>();
            Item.shootSpeed = 5f;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
    }
}
