using Microsoft.Xna.Framework;
using StarBreaker.Projs.Type;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace StarBreaker.Projs.Bullets
{
    public class SunBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("四柱子弹");
        }
        public override void StateAI()
        {
            Projectile.velocity *= 1.01f;
        }
        public override void NewSetDef()
        {
            projColor = Color.Orange;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            if (target.immortal) return;
            target.life -= 10;
            CombatText.NewText(target.Hitbox, Color.IndianRed, -10);
            target.checkDead();
            target.AddBuff(BuffID.Daybreak, 100);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);
            target.statLife -= 10;
            CombatText.NewText(target.Hitbox, Color.IndianRed, -10);
            if (target.statLife <= 0) target.KillMe(PlayerDeathReason.ByProjectile(target.whoAmI, Projectile.whoAmI), 10, 10);
            target.AddBuff(BuffID.Daybreak, 100);
        }
    }
}
