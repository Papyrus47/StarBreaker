using StarBreaker.Projs.XuanYu;
using Terraria.Utilities;

namespace StarBreaker.Items.Weapon.XuanYu
{
    public class FrostSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Spear");
            DisplayName.AddTranslation(7, "寒霜刺枪");
            Tooltip.SetDefault("Spinning attack on hit: Limit the speed of the target for 5 seconds\n" +
                "Piercing attack on hit: Apply five layers of \"Fresh Flame\" to the target, and apply five layers of bleeding at the same time\n" +
                "Fresh Flame: One layer is reduced every 1/12 second. If the layer does not become zero every second, it will cause \"Max HP * Layer * 0.1%\" damage");
            Tooltip.AddTranslation(7, "旋转攻击命中敌人:限制敌人的速度,持续五秒\n" +
                "穿刺攻击命中敌人:向目标施加五层\"极寒之炎\",同时施加五层流血\n" +
                "极寒之炎:每1/12秒减少一层,如果每隔一秒层数没有变为零,则造成\"血量上限 * 层数 * 0.1%\"的伤害\n" +
                "每用完一次穿刺攻击为一遍攻击循环");
            SacrificeTotal =1;
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 32;
            Item.knockBack = 1.2f;
            Item.rare = ItemRarityID.Red;
            Item.crit = 21;
            Item.width = Item.height = 33;
            Item.shoot = ModContent.ProjectileType<FrostSpearProj>();
            Item.shootSpeed = 15f;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item104;
            Item.noUseGraphic = true;
            Item.useTurn = true;
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
