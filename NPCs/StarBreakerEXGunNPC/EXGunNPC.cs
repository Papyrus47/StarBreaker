using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent;

namespace StarBreaker.NPCs.StarBreakerEXGunNPC
{
    public abstract class EXGunNPC : FSMNPC
    {
        private NPC StarBreakerEX_NPC => Main.npc[NPC.realLife];
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 30000;
            NPC.knockBackResist = 0f;
            NPC.defense = 18;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 32;
            NPC.height = 32;
            NPC.dontTakeDamage = true;
        }
        public sealed override void AI()
        {
            if (Target.dead || !Target.active || NPC.target == 255 || NPC.target <= 0)
            {
                NPC.TargetClosest();
            }//获取敌对目标
            if (Target.dead || !Target.active || !StarBreakerEX_NPC.active || StarBreakerEX_NPC.life < StarBreakerEX_NPC.lifeMax * 0.5f)
            {
                NPC.velocity.Y -= 0.1f;
                if (NPC.velocity.Y < -20) NPC.active = false;//到达一定的下坠速度就自杀
                return;
            }
            GunAI();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Utils.DrawLine(spriteBatch, StarBreakerEX_NPC.Center, NPC.Center, Color.Purple * 0.2f, Color.Purple, 0.5f);//星击绘制控制的线
            return true;
        }
        public abstract void GunAI();
    }
}
