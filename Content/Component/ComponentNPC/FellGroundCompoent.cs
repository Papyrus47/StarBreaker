using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.Component.ComponentNPC
{
    public class FellGroundCompoent :BasicComponentNPC
    {
        public int Time;
        public float Strength;
        public FellGroundCompoent(int time, float strength)
        {
            Time = time;
            Strength = strength;
        }
        public bool Init;
        public override bool PreAI(NPC npc)
        {
            NPCs.StarBreakerGlobalNPC starBreakerGlobalNPC = npc.StarBreaker();
            if (!Init)
            {
                Init = true;
                starBreakerGlobalNPC.HitKnockResistance -= Strength;
                if (starBreakerGlobalNPC.HitKnockResistance > 0)
                {
                    ShouldRemove = true;
                    return false;
                }
            }

            npc.velocity.X *= 0.8f;
            if (starBreakerGlobalNPC.HitKnockResistance <= 0)
            {
                starBreakerGlobalNPC.HitKnockResistance = starBreakerGlobalNPC.DefHitKnockResistance;
            }
            if (Time-- <= 0)
            {
                ShouldRemove = true;
            }
            return false;
        }
    }
}
