using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using StarBreaker.Items.Type;
using Terraria.Audio;

namespace StarBreaker.Items.Weapon.Drums
{
    public class NorDrum : Drum
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("鼓");
            Tooltip.SetDefault("标记伤害5点\n" +
                "敌人存在标记时,使敌人受到召唤物的攻击增加1/5(最高20)");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = Item.useAnimation = 30;
            Item.width = Item.height = 32;
            Item.damage = 10;//这是伤害
        }
        public override bool? UseItem(Player player)
        {
            NPC npc = player.FindTargetNPC();
            if (npc != null)
            {
                npc.GetGlobalNPC<NPCs.StarGlobalNPC>().DrumHitDamage = 5;
            }
            return base.UseItem(player);
        }
    }
}
