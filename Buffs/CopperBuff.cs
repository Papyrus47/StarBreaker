namespace StarBreaker.Buffs
{
    public class CopperBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜化");
            Description.SetDefault("变成铜！\n" +
                "获得最后的铜短剑力量");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            StarPlayer shortSword = player.GetModPlayer<StarPlayer>();
            shortSword.EGO = true;
            if (shortSword.EGO)
            {
                player.buffTime[buffIndex] = 999;
            }
            if (player.dead)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
