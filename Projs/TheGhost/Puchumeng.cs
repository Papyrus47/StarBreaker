using Microsoft.Xna.Framework;
using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.TheGhost
{
    class Puchumeng : Ghost
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("侵蚀之普戾蒙");
        }
        public override void SetDef()
        {
            LineColor = Color.Green;
            TheColorTex = "StarBreaker/Projs/TheGhost/Puchumeng";

        }
        public override void Alive()
        {
        }
    }
}
