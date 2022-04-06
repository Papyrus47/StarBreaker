using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Buffs
{
    public class EnergySmash : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("能量击碎");
            Description.SetDefault("强大的能量击碎了护甲");//介绍
            Main.debuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCs.StarGlobalNPC>().EnergySmash = true;//让npc的对应实例字段的值变成true
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense -= 20;//这是一个每帧重置的防御
        }
    }
}
