using StarBreaker.Content.ProjAIHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Items.StarOrigin.StarOriginStaff.Skills
{
    public class SOS_CircleSwing : SOS_Swing
    {
        public SOS_CircleSwing(ModProjectile projectile,SwingHelper swingHelper) : base(projectile,new Vector2(1,0.4f), Vector2.UnitX, 0.5f, 1,swingHelper, 0.2f)
        {

        }
        public override void AI()
        {
            base.AI();
            if (SkyAttack)
            {
                Player.velocity.Y -= 1f;
            }
            if ((int)Projectile.ai[1] % 15 == 0)
            {
                ResetHit();
            }
            if (Projectile.ai[1] <= 270)
            {
                Player.velocity.X += 0.5f * Player.direction;
            }
        }
        public override float SwingTimerChange()
        {
            return TimerChange(Projectile.ai[1] / 270) * MathHelper.TwoPi * 2f * SwingRotDir;
        }
    }
}
