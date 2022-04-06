using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.NPCs
{
    public abstract class FSMNPC : ModNPC
    {
        protected float Timer1
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        protected float Timer2
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        protected float Timer3
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        protected float State
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        /// <summary>
        /// NPC的目标
        /// </summary>
        protected Player Target => Main.player[NPC.target];
    }
}
