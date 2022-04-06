using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon
{
    public class EnergyConverter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Converter");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "能量转换器");
            Tooltip.SetDefault("星辰击碎者所携带的能量转换器,通过魔法转换为它的子弹");
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 10;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 10;
            Item.knockBack = 1.2f;
            Item.mana = 5;
            Item.rare = ItemRarityID.Orange;
            Item.crit = 31;
            Item.width = 30;
            Item.height = 46;
            Item.channel = true;
            Item.value = 10000;
            Item.shoot = ModContent.ProjectileType<Projs.EnergyConverterProj>();
            Item.shootSpeed = 10;
            Item.noUseGraphic = true;
        }
    }
}
