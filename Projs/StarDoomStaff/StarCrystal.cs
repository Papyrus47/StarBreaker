using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Projs.StarDoomStaff
{
    public abstract class StarCrystal : ModProjectile
    {
        public override string Texture => (GetType().Namespace + ".StarCrystal").Replace('.', '/');
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.friendly = true;
            Projectile.width = 26;
            Projectile.height = 24;
            Projectile.minion = true;
            Projectile.minionSlots = 0.01f;
            Projectile.localNPCHitCooldown = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Main.player[Projectile.owner].active)
            {
                Projectile.timeLeft = 2;
            }
            if (!Main.player[Projectile.owner].HasMinionAttackTargetNPC)
            {
                Player player = Main.player[Projectile.owner];
                Projectile.position = player.position + new Vector2(0, -100);
                Projectile.velocity = Vector2.Zero;
                Projectile.rotation = -MathHelper.PiOver4;
                Projectile.extraUpdates = 0;
            }
            else
            {
                Projectile.extraUpdates = 2;
                Projectile.velocity = (Projectile.velocity * 10 + (Projectile.OwnerMinionAttackTargetNPC.position - Projectile.position) * 0.1f) / 11;
            }
        }
    }
    public class StarBoomCrystal : StarCrystal
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("爆炸水晶");
        }
        public override void AI()
        {
            if (Main.player[Projectile.owner].HasMinionAttackTargetNPC && Main.player[Projectile.owner].slotsMinions < 0.05f)
            {
                Projectile.tileCollide = true;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.timeLeft = 30;
                    Projectile.ai[1]++;
                    Projectile.velocity = (Projectile.OwnerMinionAttackTargetNPC.position - Projectile.position) / 10;
                }
                else
                {
                    Projectile.extraUpdates = 2;
                }
            }
            else
            {
                base.AI();
            }
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] < 2 && Main.player[Projectile.owner].HasMinionAttackTargetNPC)
            {
                Main.projectile[Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(),
                    Main.player[Projectile.owner].Center, Projectile.velocity.SafeNormalize(default) * 20, Type,
                    Projectile.damage, Projectile.knockBack, Projectile.owner, ++Projectile.ai[0], --Projectile.ai[1])].originalDamage = Projectile.damage;
            }
        }
    }
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
