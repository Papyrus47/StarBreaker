using StarBreaker.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items
{
    internal class EmergencyUseOfBossSummoner : ModItem
    {
        public override string Texture => "StarBreaker/Projs/Star";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰武器诱捕器");
            Tooltip.SetDefault("吸引星辰武器\n" +
                "根据时期与地形决定召唤的boss");
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.width = 66;
            Item.height = 25;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 40;
            Item.noUseGraphic = true;
        }
        public override bool? UseItem(Player player)
        {
            if(!StarBreakerSystem.downedStarBreakerNom)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<StarBreakerN>());
            }
            else if (!StarBreakerSystem.downedStarBreakerEX)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<StarBreakerEX>());
            }
            return base.UseItem(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
