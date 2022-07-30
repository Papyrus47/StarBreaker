namespace StarBreaker.Items.BossSummon
{
    public class OnyxSummon : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.OnyxBlaster;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Blaster");
            DisplayName.AddTranslation(7, "玛瑙爆破枪");
            Tooltip.SetDefault("Summon Onyx Blaster");
            Tooltip.AddTranslation(7, "召唤玛瑙爆破枪");
        }
        public override void SetDefaults()
        {
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.width = 30;
            Item.height = 23;
        }
        public override bool? UseItem(Player player)
        {
            if (Main.CurrentFrameFlags.AnyActiveBossNPC)
            {
                return false;
            }

            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.NorBoss.OnyxBlaster.OnyxBlaster>());
            return base.UseItem(player);
        }
    }
}
