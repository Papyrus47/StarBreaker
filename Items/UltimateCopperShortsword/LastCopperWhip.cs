using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperWhip : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜鞭");
            Tooltip.SetDefault("你的召唤物将集中攻击敌人\n" +
                 "每击中一次目标伤害提升10%\n" +
                 "以软铁击硬石");
        }
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<LastCopperWhipProj>(), 90, 1.3f, 15, 19);
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.value = 24300;
            Item.rare = ItemRarityID.Red;
        }
    }
}
