using StarBreaker.NPCs.UltimateCopperShortsword.Bosses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class SumShortSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜短剑");
            Tooltip.SetDefault("怒火照耀全身");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CopperBar, 10).AddTile(TileID.MythrilAnvil).Register();
        }

        public override bool CanUseItem(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<ShortSword>() && npc.active ||
                    npc.type == ModContent.NPCType<ShortSword2>() && npc.active ||
                    npc.type == ModContent.NPCType<ShortSword3>() && npc.active)
                {
                    return false;
                }
            }
            return true;
        }
        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ShortSword>());
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }
    }
}
