using StarBreaker.Content.TheSkillProj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Items.StarOrigin.StarOriginStaff.Skills
{
    public abstract class StarOriginStaff_BasicSkill : ProjSkill_Instantiation
    {
        protected StarOriginStaff StarOriginStaff => modProjectile as StarOriginStaff;
        protected Player Player => StarOriginStaff.player;
        public bool SkyAttack;
        public bool IsWait;
        public StarOriginStaff_BasicSkill(ModProjectile projectile) : base(projectile)
        {

        }
        public override bool ActivationCondition()
        {
            bool inAir = Player.StarBreaker().InAir;
            if (SkyAttack)
            {
                return inAir;
            }
            return !inAir;
        }
        public void ResetHit()
        {
            Projectile.numHits = 0;
            for(int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
        }
    }
}
