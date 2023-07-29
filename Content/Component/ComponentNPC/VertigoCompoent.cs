using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.Component.ComponentNPC
{
    public class VertigoCompoent : BasicComponentNPC
    {
        public int VertigoTime;
        public float Strength;

        public VertigoCompoent(int vertigoTime, float strength = 0)
        {
            VertigoTime = vertigoTime;
            Strength = strength;
            if (Strength == 0) Strength = VertigoTime;
        }

        public bool Init;
        public override bool PreAI(NPC npc)
        {
            NPCs.StarBreakerGlobalNPC starBreakerGlobalNPC = npc.StarBreaker();
            if (!Init)
            {
                Init = true;
                starBreakerGlobalNPC.Vertigo -= Strength;
                if (starBreakerGlobalNPC.Vertigo > 0)
                {
                    return true;
                }
            }
            if(starBreakerGlobalNPC.Vertigo <= 0)
            {
                starBreakerGlobalNPC.Vertigo = starBreakerGlobalNPC.DefVertigo;
            }
            if(VertigoTime-- < 0)
            {
                ShouldRemove = true;
            }
            return false;
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int frame = 0;
            switch(VertigoTime % 9)
            {
                case < 3:frame = 0;break;
                case < 6:frame = 1;break;
                case < 9:frame = 2;break;
            }
            spriteBatch.Draw(StarBreakerAssetHelper.Vertigo.Value, npc.Top + new Vector2(0, 20) - screenPos, new Rectangle(0, frame * 20, 20, 20), Color.White, 0, StarBreakerAssetHelper.Vertigo.Size() * 0.5f,
                2f, SpriteEffects.None, 0f);
        }
    }
}
