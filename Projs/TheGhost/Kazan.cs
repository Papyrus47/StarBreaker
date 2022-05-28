using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.TheGhost
{
    internal class Kazan : Ghost
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("苏蔑卡赞");
        }
        public override void SetDef()
        {
            LineColor = Color.Red;
            TheColorTex = "StarBreaker/Projs/TheGhost/Kazan";
        }
        public override void Alive()
        {
        }
    }
}
