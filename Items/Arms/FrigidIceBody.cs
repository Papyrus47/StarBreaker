﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Arms
{
    [AutoloadEquip(EquipType.Body)]
    public class FrigidIceBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("极寒之冰");
            Tooltip.SetDefault("用魔法凝聚的冰之护甲\n" +
                "固定增加20的近战伤害");
        }
        public override void SetDefaults()
        {
            Item.defense = 21;
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void UpdateEquip(Player player)
        {
            player.meleeAddDamage = 20;
        }
    }
}
