using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Arms
{
    [AutoloadEquip(EquipType.Head)]
    public class FrigidIceHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("极寒之冰");
            Tooltip.SetDefault("用魔法凝聚的冰之头盔\n" +
                "提升你的跑步速度");
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
            player.maxRunSpeed += 2;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs) => head.type == Type && body.type == ModContent.ItemType<FrigidIceBody>() && legs.type == ModContent.ItemType<FrigidIceLegs>();
        public override void UpdateArmorSet(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.3f;
            player.setBonus = "提升30%的近战伤害\n" +
                "攻击概率使敌人减速\n" +
                "如果使用\"宣雨\"对应武器\"寒冰刺枪\",向目标施加一层\"霜冻\"n" +
                "\"霜冻\"达到六层之后,冻结敌人,每5秒减低1层\n" +
                "当装备全套的时候,你总感觉这一个套装冻结了周围的事物";
        }
    }
}
