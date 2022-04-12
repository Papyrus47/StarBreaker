using Microsoft.Xna.Framework;
using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.TheGhost
{
    class Rhasa : Ghost
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("瘟疫之罗刹");
        }
        public override void SetDef()
        {
            LineColor = Color.Purple;
            TheColorTex = "StarBreaker/Projs/TheGhost/Raksha";

        }
        public override void Alive()
        {

        }
    }
}
