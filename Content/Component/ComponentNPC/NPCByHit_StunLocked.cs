using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.Component.ComponentNPC
{
    public class NPCByHit_StunLocked : BasicComponentNPC
    {
        public int Time;
        public NPCByHit_StunLocked(int Time)
        {
            this.Time = Time;
        }
        public override bool PreAI(NPC npc)
        {
            NPCs.StarBreakerGlobalNPC starBreakerGlobalNPC = npc.StarBreaker();
            starBreakerGlobalNPC.StunLocked -= Time;
            if(starBreakerGlobalNPC.StunLocked <= 0)
            {
                starBreakerGlobalNPC.StunLocked = starBreakerGlobalNPC.DefStunlocked;
            }
            else
            {
                ShouldRemove = true;
                return true;
            }
            if(Time-- <= 0)
            {
                ShouldRemove = true;
                return false;
            }
            return false;
        }
    }
}
