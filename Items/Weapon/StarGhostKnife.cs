using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon
{
    public class StarGhostKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰鬼刀");
            Tooltip.SetDefault("绝对的力量\n" +
                "曾经有人用了这把刀和那把枪走遍了世界\n" +
                "现在，它是你的了\n" +
                "切换状态:使用被绑定的按钮切换\n" +
                " \" 痛苦，绝望，都凝聚之上，只是被注入了星辰的力量才压制了那份真实的实力 \" ");
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = 1;
            Item.damage = 210;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 38;
            Item.rare = ItemRarityID.Red;
            Item.value = 13100232;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.mod == "Terraria" && line.Name == "ItemName")
                {
                    line.overrideColor = new Color?(new Color((int)Math.Sqrt(Main.time), 1, (int)Math.Sqrt(Main.time)));
                }
            }
        }
    }
}
