using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.Bullets
{
    public class BrightBullet : EnergyProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("光明子弹");
        }
        public override void NewSetDef()
        {
            projColor = Color.LightPink;
        }
        public override void StateAI()
        {
            Projectile.velocity *= 1.01f;
            if (Projectile.timeLeft < 250)
            {
                NPC npc = null;
                float max = 800;
                foreach (NPC n in Main.npc)
                {
                    float dis = n.Distance(Projectile.position);
                    if (dis < max && n.active && n.CanBeChasedBy() && Collision.CanHit(n.position, 1, 1, Projectile.position, 1, 1))
                    {
                        max = dis;
                        npc = n;
                    }
                }
                if (npc != null)
                {
                    Projectile.velocity = (Projectile.velocity * 20 + (npc.position - Projectile.position).SafeNormalize(default) * 10) / 21;
                }
            }
        }
    }
}
