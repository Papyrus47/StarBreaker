using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.TheGhost
{
    public class Saya : Ghost
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("冰霜之萨亚");
        }
        public override void SetDef()
        {
            LineColor = Color.Cyan;
            TheColorTex = "StarBreaker/Projs/TheGhost/Saya";
        }
        public override void Alive()
        {
            foreach (Player player in Main.player)
            {
                if (Main.rand.Next(100) == 0)
                {
                    if (Vector2.Distance(player.Center, Projectile.Center) < 1000)
                    {
                        player.AddBuff(BuffID.Frozen, 10);
                    }
                }
            }
        }
    }
}
