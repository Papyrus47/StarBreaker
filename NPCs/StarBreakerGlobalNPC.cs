using StarBreaker.Content;
using StarBreaker.Content.Appraise;
using StarBreaker.Content.Component;
using StarBreaker.Content.Component.ComponentNPC;
using StarBreaker.Content.Component.ComponentPlayer;
using StarBreaker.Content.ElementClass.NPCSetting;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace StarBreaker.NPCs
{
    public class StarBreakerGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool StarsPierceProj_Throughout;
        public NPC_ElementClass elementClass;
        public Dictionary<Type, BasicComponentNPC> Compontent;
        public float HitKnockResistance;
        public float DefHitKnockResistance;
        public float Vertigo;
        public float DefVertigo;
        public int StunLocked;
        public int DefStunlocked;
        public override GlobalNPC NewInstance(NPC target)
        {
            Compontent = new();
            elementClass = new();
            return base.NewInstance(target);
        }
        public override void Load()
        {
            Terraria.On_NPC.UpdateNPC_Inner += NPC_UpdateNPC_Inner;
            //On_NPC.StrikeNPC += On_NPC_StrikeNPC;
        }
        //private double On_NPC_StrikeNPC(On_NPC.orig_StrikeNPC orig, NPC self, int Damage, float knockBack, int hitDirection, bool crit, bool noEffect, bool fromNet)
        //{
        //    var apprasie = AppraiseSystem.Instance.appraise[0].AppraiseID;
        //    bool change = false;
        //    if (!self.immortal && apprasie <= AppraiseID.A || apprasie == AppraiseID.None)
        //    {
        //        change = true;
        //        self.immortal = true;
        //    }
        //    double index = orig.Invoke(self, Damage, knockBack, hitDirection, crit, noEffect, fromNet);
        //    if (change) self.immortal = false;
        //    return index;
        //}
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            pool.Clear();
        }
        public override void ResetEffects(NPC npc)
        {
            if (Compontent == null) Compontent = new();
            List<BasicComponentNPC> components = new(Compontent.Values);
            for (int i = 0; i < components.Count; i++)
            {
                (components[i] as IStarBreakerComponent).RemoveUpdate(Compontent);
            }
            components.ForEach(x => x.ResetEffects(npc));
        }
        public override void SetDefaults(NPC npc)
        {
            DefHitKnockResistance = HitKnockResistance;
            DefVertigo = Vertigo;
            DefStunlocked = StunLocked;
        }
        public override bool PreAI(NPC npc)
        {
            foreach (var item in Compontent)
            {
                if (!item.Value.PreAI(npc)) return false;
            }
            return base.PreAI(npc);
        }
        public override void AI(NPC npc)
        {
            Compontent.ForEachValue(x => x.AI(npc));
        }
        public override void PostAI(NPC npc)
        {
            Compontent.ForEachValue(x => x.PostAI(npc));
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            foreach (var item in Compontent)
            {
                if (!item.Value.PreDraw(npc,spriteBatch,screenPos,drawColor)) return false;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
            Compontent.ForEachValue(x => x.PostDraw(npc, spriteBatch, screenPos, drawColor));
        }
        private void NPC_UpdateNPC_Inner(Terraria.On_NPC.orig_UpdateNPC_Inner orig, NPC self, int i)
        {
            if (!NPC_CanUpdate())
            {
                return;
            }

            orig.Invoke(self, i);
        }
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            modifiers.HideCombatText();
            modifiers.CritDamage *= 0;
            if (Compontent == null) return;
            foreach (var item in Compontent)
            {
                item.Value.ModifyIncomingHit(npc, ref modifiers);
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            elementClass ??= new();
            elementClass.ModifyHit(ref modifiers);
            foreach (var item in Compontent)
            {
                item.Value.ModifyHitByProjectile(npc,projectile,ref modifiers);
            }
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            var apprasie = AppraiseSystem.Instance.appraise[0].AppraiseID;
            bool flag = apprasie <= AppraiseID.A || apprasie == AppraiseID.None;
            if (flag)
            {
                npc.life += damageDone;
            }
            Compontent.ForEachValue(x => x.OnHitByItem(npc, player, item, hit, damageDone));
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            elementClass.OnHit(npc);
            var apprasie = AppraiseSystem.Instance.appraise[0].AppraiseID;
            bool flag = apprasie <= AppraiseID.A || apprasie == AppraiseID.None;
            if (flag)
            {
                npc.life += damageDone;
            }
            Compontent.ForEachValue(x => x.OnHitByProjectile(npc, projectile, hit, damageDone));
        }
        //public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        //{
        //    var apprasie = AppraiseSystem.Instance.appraise[0].AppraiseID;
        //    if (apprasie <= AppraiseID.A || apprasie == AppraiseID.None)
        //    {
        //        damage = 0.0;
        //        crit = false;
        //        return false;
        //    }
        //    return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        //}
        public bool NPC_CanUpdate()
        {
            return !StarsPierceProj_Throughout;
        }
    }
}
