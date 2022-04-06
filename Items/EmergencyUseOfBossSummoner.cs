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
            DisplayName.SetDefault("tml更新后紧急本mod测试boss召唤器");
            Tooltip.SetDefault("开发者所用，用于Hero无法使用时，手动写入召唤的boss\n" +
                "可空手合成");
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.width = 66;
            Item.height = 25;
            Item.useStyle = 3;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 40;
            Item.noUseGraphic = true;
        }
        public override bool? UseItem(Player player)
        {
            foreach (NPC n in Main.npc)
            {
                n.active = false;
            }
            int npc = NPC.NewNPC(player.GetNPCSource_TileInteraction((int)player.position.X, (int)player.position.Y + 100), (int)player.position.X, (int)player.position.Y + 100, ModContent.NPCType<NPCs.StarBreakerN>());
            //StarBreakerSystem.downedStarBreakerEX = false;
            //NPC.NewNPC((int)player.position.X, (int)player.position.Y - 100,NPCID.MoonLordCore);

            player.immune = true;
            player.immuneTime = 300;
            foreach (Item item in Main.item)
            {
                item.active = false;
            }
            //Item.NewItem(player.Hitbox, ModContent.ItemType<FrostFistW>());
            return base.UseItem(player);
        }
        //public override void AddRecipes()
        //{
        //    CreateRecipe().Register();
        //}
    }
}
