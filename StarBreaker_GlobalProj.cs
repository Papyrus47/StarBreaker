using StarBreaker.Content.ElementClass;
using StarBreaker.Content.TheSkillProj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.WorldBuilding;

namespace StarBreaker
{
    public class StarBreaker_GlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            base.OnSpawn(projectile, source);
            if (projectile.ModProjectile is IBasicSkillProj basic)
            {
                basic.Init();
                basic.OldSkills = new();
            }
        }
        public override void AI(Projectile projectile)
        {
            if (projectile.ModProjectile is IBasicSkillProj basic)
            {
                basic.CurrentSkill.AI();
                basic.SwitchSkill();
                if (Main.player[projectile.owner].HeldItem.shoot != projectile.type)
                {
                    projectile.Kill();
                    return;
                }
                projectile.timeLeft = 5;
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.ModProjectile is IBasicSkillProj basic)
            {
                return basic.CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
            }
            return base.PreDraw(projectile, ref lightColor);
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.ModProjectile is IBasicSkillProj basic)
            {
                basic.CurrentSkill.OnHitNPC(target,hit,damageDone);
            }
        }
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (projectile.ModProjectile is IBasicSkillProj basic)
            {
                basic.CurrentSkill.ModifyHitNPC(target, ref modifiers);
            }
        }
        public override bool? CanDamage(Projectile projectile)
        {
            if (projectile.ModProjectile is IBasicSkillProj basic)
            {
                return basic.CurrentSkill.CanDamage();
            }
            return base.CanDamage(projectile);
        }
        public override bool? Colliding(Projectile projectile, Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projectile.ModProjectile is IBasicSkillProj basic)
            {
                return basic.CurrentSkill.Colliding(projHitbox,targetHitbox);
            }
            return base.Colliding(projectile, projHitbox, targetHitbox);
        }
    }
}
