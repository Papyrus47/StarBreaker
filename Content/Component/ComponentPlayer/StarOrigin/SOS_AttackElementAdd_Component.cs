using StarBreaker.Content.ControlPlayerSystem;
using StarBreaker.Content.ElementClass;
using StarBreaker.UIs.PlayerControlSystemUI.StarOrigin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.Component.ComponentPlayer.StarOrigin
{
    public class SOS_AttackElementAdd_Component : BasicPlayerComponent
    {
        StarOriginControlSystem starOrigin;

        public SOS_AttackElementAdd_Component(Player player) : base(player)
        {
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            starOrigin ??= player.StarBreaker().playerSystem as StarOriginControlSystem;
            BasicElementsClass elementsClass = starOrigin.ElementID switch
            {
                1 => new FireElement(),
                2 => new WindElement(),
                3 => new IceElement(),
                4 => new LightningElement(),
                _ => new NoneElement(),
            };
            starOrigin.ModifyHitNPC(target, new() { elementsClass }, ref modifiers);
            target.AddElementClass(elementsClass);
        }
    }
}
