using Microsoft.Xna.Framework;
using Terraria;

namespace StarBreaker.Projs.StarDoomStaff
{
    public class StarControlCrystal : StarCrystal
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("控制水晶");
        }
        public override void AI()
        {
            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[0] = -1;
                Projectile.ai[1]++;
            }
            if (Main.player[Projectile.owner].HasMinionAttackTargetNPC || Projectile.ai[0] > -1)
            {
                if (Projectile.ai[0] == -1 && Projectile.OwnerMinionAttackTargetNPC != null)
                {
                    NPC npc = Projectile.OwnerMinionAttackTargetNPC;
                    if (npc.friendly) return;
                    if (Vector2.Distance(Projectile.position, npc.position) < 300)
                    {
                        Projectile.timeLeft = 600;
                        Projectile.ai[0] = npc.whoAmI;
                    }
                }
                else if (Projectile.ai[0] >= 0)
                {
                    NPC npc = Main.npc[(int)Projectile.ai[0]];
                    if (!npc.active || !npc.CanBeChasedBy())
                    {
                        Projectile.ai[0] = -1;
                        return;
                    }
                    if (Vector2.Distance(npc.Center, Projectile.Center) > 250)
                    {
                        npc.Center = Projectile.Center + (npc.Center - Projectile.Center).SafeNormalize(default) * 250;
                    }
                    Projectile.velocity *= 0.9f;
                    if (Projectile.rotation > -MathHelper.PiOver4)
                    {
                        Projectile.rotation -= 0.02f;
                    }
                    else if (Projectile.rotation < -MathHelper.PiOver4)
                    {
                        Projectile.rotation += 0.02f;
                    }
                }
                else
                {
                    Projectile.ai[0] = -1;
                    base.AI();
                }
            }
            else
            {
                Projectile.ai[0] = -1;
                base.AI();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] >= 0 && Projectile.ai[0] <= 200)
            {
                Utils.DrawLine(Main.spriteBatch, Main.npc[(int)Projectile.ai[0]].Center, Projectile.Center, Color.Yellow * 0.5f, Color.Transparent, 5);
            }
            return base.PreDraw(ref lightColor);
        }
    }
}
