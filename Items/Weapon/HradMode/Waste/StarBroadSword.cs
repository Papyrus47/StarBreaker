using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon.HradMode.Waste
{
    public class StarBroadSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Broad Sword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "星辰阔剑");
            Tooltip.SetDefault("横向挥动\n" +
                "命中敌人时,施加50层流血");
            Item.staff[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 210;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 3.2f;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.crit = 61;
            Item.mana = 2;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.value = 5130142;
            Item.UseSound = SoundID.Item101;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<Projs.Waste.StarBroadSwordProj>();
            Item.shootSpeed = 10;
            Item.noUseGraphic = true;
        }
    }
}
