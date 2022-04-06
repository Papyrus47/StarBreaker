using Terraria;
using Terraria.ID;

namespace StarBreaker.NPCs.StarBreakerEXGunNPC
{
    internal class EXVortexBeater : EXGunNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("EX-星旋机枪");
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 3)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= 7 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
    }
}
