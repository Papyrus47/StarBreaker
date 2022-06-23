namespace StarBreaker.Buffs
{
    public class CopperSummonBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("小铜短剑");
            Description.SetDefault("小铜短剑难道不可爱吗?");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if(player.active)
            {
                player.buffTime[buffIndex] = 60;
            }
            if (player.dead)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
