using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Buffs
{
    public class IncantationFireInBody : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("咒火入体");
            Description.SetDefault("咒火在体内燃烧");
            Main.debuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
            if (!npc.immortal)
            {
                npc.life -= 10;
                CombatText.NewText(npc.Hitbox, CombatText.DamagedHostile, 10);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.GreenFairy);
                    dust.velocity *= 2;
                    dust.scale *= 2f;
                    dust.noGravity = true;
                }
                npc.checkDead();
            }
        }
    }
}
