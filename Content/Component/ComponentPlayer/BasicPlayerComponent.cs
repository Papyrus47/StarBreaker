using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;

namespace StarBreaker.Content.Component.ComponentPlayer
{
    public abstract class BasicPlayerComponent : IStarBreakerComponent
    {
        public StarBreakerPlayer starBreakerPlayer;
        public Player player => starBreakerPlayer.Player;

        public bool ShouldRemove { get; set; }
        public BasicPlayerComponent(Player player)
        {
            starBreakerPlayer = player.StarBreaker();
            Apply();
        }
        public BasicPlayerComponent(StarBreakerPlayer starBreakerPlayer)
        {
            this.starBreakerPlayer = starBreakerPlayer;
            Apply();
        }

        public virtual void Apply() { }
        public virtual void Remove() { }
        public virtual void ResetEffects() { }
        public virtual bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter) => true;
        public virtual void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit) { }
        public virtual void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit) { }
        public virtual bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) => true;
        public virtual void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) { }
        public virtual void PreUpdateMovement() { }

        public virtual void PreUpdate()
        {
        }
        public virtual void ProcessTriggers(TriggersSet triggersSet)
        {
        }

        public virtual void SetControls()
        {
        }
        public virtual void PreUpdateBuffs()
        {
        }
        public virtual void PostUpdateBuffs()
        {
        }
        public virtual void UpdateEquips()
        {
        }
        public virtual void PostUpdateEquips()
        {
        }
        public virtual void PostUpdate()
        {
        }
        public virtual bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }
        public virtual void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }
        public virtual void MeleeEffects(Item item, Rectangle hitbox)
        {
        }
        public virtual void OnHitAnything(float x, float y, Entity victim)
        {
        }
        public virtual void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
        }
        public virtual void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
        }
        public virtual bool? CanHitNPCWithProj(Projectile proj, NPC target)
        {
            return null;
        }
        public virtual void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
        }
        public virtual void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDon)
        {
        }
        public virtual bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            return true;
        }
        public virtual void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
        }
        public virtual void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
        }
        public virtual bool CanBeHitByProjectile(Projectile proj)
        {
            return true;
        }
        public virtual void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
        }
        public virtual void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
        }
        public virtual void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
        }
        public virtual void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
        }
        public virtual bool CanUseItem(Item item)
        {
            return true;
        }
    }
}
