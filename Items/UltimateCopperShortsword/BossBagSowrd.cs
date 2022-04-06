using StarBreaker.NPCs.UltimateCopperShortsword.Bosses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class BossBagSowrd : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("宝藏袋");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
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
        public override bool CanRightClick()
        {
            return true;
        }
        public override int BossBagNPC => ModContent.NPCType<ShortSword3>();
        public override void OpenBossBag(Player player)
        {
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
                        ModContent.ItemType<LastCopperWhip>()
            };
            for (int i = 0; i < ID.Length; i++)
            {
                player.QuickSpawnItem(player.GetItemSource_OpenItem(Type), ID[i]);
            }
        }
    }
}
