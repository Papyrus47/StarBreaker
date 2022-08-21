using StarBreaker.NPCs.UltimateCopperShortsword.Bosses;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class BossBagSowrd : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("宝藏袋");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Purple;
            Item.expert = true;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            IEntitySource openItem = player.GetSource_OpenItem(Type);
            int[] ID =
                {
                     ModContent.ItemType<LastCopperAxe>(),
                      ModContent.ItemType<LastCopperBow>(),
                       ModContent.ItemType<LastCopperChainSaw>(),
                        ModContent.ItemType<LastCopperDiamond>(),
                         ModContent.ItemType<LastCopperHammer>(),
                          ModContent.ItemType<LastCopperPick>(),
                           ModContent.ItemType<LastCopperSickle>(),
                            ModContent.ItemType<LastCopperSpear>(),
                             ModContent.ItemType<LastShortSowrd>(),
                             ModContent.ItemType<LastCopperKnife>(),
                             ModContent.ItemType<LastCopperWhip>(),
                             ModContent.ItemType<LastCopperGun>(),
                             ModContent.ItemType<LastCopperJackhammer>(),
                             ModContent.ItemType<LastCopperSowrdSummonStaff>()
                 };
            for (int i = 0; i < ID.Length; i++)
            {
                player.QuickSpawnItem(openItem, ID[i]);
            }
        }
    }
}
