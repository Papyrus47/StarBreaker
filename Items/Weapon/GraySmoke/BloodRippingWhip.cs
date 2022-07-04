using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace StarBreaker.Items.Weapon.GraySmoke
{
    public class BloodRippingWhip : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "鲜血撕裂鞭");
            Tooltip.SetDefault("通过蓄力,撕裂敌人的血肉\n" +
                "使用者如果是灰烟缭绕,其力量可怕可以撕裂一栋摩天大楼\n" +
                "命中时,施加1570层流血,随着蓄力时间提升效果\n" +
                "当最大蓄力击中目标时,使目标一分钟内无法损失流血层数\n" +
                "当鞭子重复命中5次同一目标后,重复计算两次伤害和流血层数");
        }
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<BloodRippingWhipProj>(), 150, 10f,3f, 40);
            Item.DamageType = DamageClass.Melee;
            Item.value = 24300;
            Item.rare = ItemRarityID.Red;
            Item.channel = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach(var tool in tooltips)
            {
                if(tool.Name == "ItemName" && tool.Mod == "Terraria")
                {
                    tool.OverrideColor = Color.Gray;
                    break;
                }
            }
        }
    }
}
