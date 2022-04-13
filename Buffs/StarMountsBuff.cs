using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Buffs
{
    public class StarMountsBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰护盾");
            Description.SetDefault("欸你真用护盾飞了?!");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<Mounts.StarMountsMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
