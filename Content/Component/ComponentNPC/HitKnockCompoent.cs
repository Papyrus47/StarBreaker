using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.Component.ComponentNPC
{
    public class HitKnockCompoent : BasicComponentNPC
    {
        public int Time;
        public Vector2 Dir;
        public float Strength;
        public float DefStrength;
        public bool Init;
        public bool CollideY;
        public HitKnockCompoent(float strength, Vector2 dir, int time) : base()
        {
            Strength = strength;
            Dir = dir.RealSafeNormalize();
            DefStrength = Strength;
            Time = time;
        }
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

            if(starBreakerGlobalNPC.HitKnockResistance <= 0)
            {
                starBreakerGlobalNPC.HitKnockResistance = starBreakerGlobalNPC.DefHitKnockResistance;
            }
            Strength *= 0.9f;
            npc.velocity *= 0.85f;
            npc.velocity += Dir * Strength * 0.2f;
            if (npc.collideY && !CollideY && Math.Abs(Dir.ToRotation() - Vector2.UnitY.ToRotation()) < 1f)
            {
                CollideY = true;
                Dust dust = Dust.NewDustDirect(npc.Bottom, npc.width, 5, DustID.Stone);
                dust.noGravity = true;
                dust.velocity = (-Vector2.UnitY).RotateRandom(MathHelper.PiOver2) * 3f;
                dust.scale = 3f;
                npc.AddCompoent(new FellGroundCompoent(8,15));
                ShouldRemove = true;
            }
            if(Time-- <= 0)
            {
                ShouldRemove = true;
            }
            return false;
        }
    }
}
