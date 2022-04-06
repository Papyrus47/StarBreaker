using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs.UltimateCopperShortsword
{
    public class FlySpearProj : ModProjectile
    {
        NPC target = null;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜矛");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 200;
            Projectile.height = Projectile.width = 10;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.usesIDStaticNPCImmunity = true;
        }
        public override void AI()
        {
            if (Projectile.friendly)
            {
                if (Projectile.ai[1] == 0)
                {
                    Projectile.velocity.Y += 0.1f;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.PiOver2;
                }
                else
                {
                    if (target != null)
                    {
                        Vector2 proj_vel = target.Center - Projectile.Center;
                        if (proj_vel != Vector2.Zero)
                        {
                            proj_vel.Normalize();
                            proj_vel *= 14f;
                        }
                        Projectile.velocity = (Projectile.velocity * 4 + proj_vel) / 5;//速度渐变
                        if (target.active && target.CanBeChasedBy() && !target.friendly)
                        {
                            Projectile.gfxOffY = target.gfxOffY;
                            Projectile.Center = target.Center - Projectile.velocity * 2;
                            target.HitEffect(0, 1.0);
                        }
                        else
                        {
                            Projectile.ai[1] = 0;
                        }
                    }
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.PiOver2;
                if (Projectile.ai[0] == 0 && Projectile.ai[1] == 0)
                {
                    Projectile.velocity.Y += 0.05f;
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            target.buffImmune[BuffID.Poisoned] = false;
            int r = 0;
            foreach (int i in target.buffType)
            {
                if (i == BuffID.Poisoned)
                {
                    Projectile.damage += target.buffTime[r];
                }
                r++;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type != NPCID.TargetDummy)
            {
                Projectile.ai[1] = 1;
                this.target = target;
                target.AddBuff(BuffID.Poisoned, 60, true);
                if (Projectile.timeLeft > 30) Projectile.timeLeft = 30;
            }
            Projectile.damage = 30;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.immune = false;
            target.immuneTime = 0;
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.immune = false;
            target.immuneTime = 0;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[1] = 1;
            Projectile.velocity = Vector2.Zero;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            Vector2 rot = (Projectile.rotation - (MathHelper.PiOver4 + MathHelper.PiOver2)).ToRotationVector2();
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + rot * 27,
                Projectile.Center + rot * -27,
                5, ref r);
        }
    }
}
