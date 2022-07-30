using StarBreaker.Projs.XuanYu;
using Terraria.Utilities;

namespace StarBreaker.Items.Weapon.XuanYu
{
    public class FrostThornSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Thorn Sword");
            DisplayName.AddTranslation(7, "寒霜刺剑");
            Tooltip.SetDefault("Stab forward three times\n" +
                "On hit: Limit the speed of the target for 3 seconds");
            Tooltip.AddTranslation(7, "向前方刺出三次攻击\n" +
                "命中敌人时:限制敌人的速度,持续三秒");
            Item.SacrificeCountNeededByItemId(1);
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 32;
            Item.knockBack = 1.2f;
            Item.rare = ItemRarityID.Red;
            Item.crit = 21;
            Item.width = Item.height = 40;
            Item.shoot = ModContent.ProjectileType<FrostThornSwordProj>();
            Item.shootSpeed = 15f;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }
        public override bool? PrefixChance(int pre, UnifiedRandom rand)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            Item.prefix = -1;
        }
    }
}
