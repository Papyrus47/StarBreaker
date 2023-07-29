using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Items.StarOrigin.StarOriginStaff.Skills
{
    public class SOS_NotUse : StarOriginStaff_BasicSkill
    {
        public SOS_NotUse(ModProjectile projectile) : base(projectile)
        {
        }
        public override void AI()
        {
            Projectile.velocity *= 0;
            Projectile.spriteDirection = 1;
            Projectile.Center = Player.Center + new Vector2(Player.width * -Player.direction,0);
            Projectile.rotation = -MathHelper.PiOver4 - Player.direction * MathHelper.PiOver4 * 0.5f;
        }
        public override void OnSkillDeactivate()
        {
            Projectile.rotation = 0;
        }
        public override bool SwitchCondition() => true;
        public override bool ActivationCondition() => false;
    }
}
