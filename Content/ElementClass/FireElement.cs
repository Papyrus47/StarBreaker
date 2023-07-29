using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.ElementClass
{
    public class FireElement : BasicElementsClass
    {
        public override void OnHurt(Entity target)
        {
            Dust dust = Dust.NewDustDirect(target.Center,5,5, DustID.Torch);
            dust.scale = 1.3f;
            dust.velocity = Main.rand.NextVector2Unit() * 2f;
            dust.noGravity = true;
        }
    }
}
