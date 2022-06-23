using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.EnergyDamage_Proj.Summon
{
    public class ToDarkFoxSword : EnergyProj
    {
        public override string Texture => (GetType().Namespace + "." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "至暗遗狐");
        }
        public override void NewSetDef()
        {
            DrawBulletBody = null;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = Projectile.height = 62;
            Projectile.tileCollide = false;
        }

        public override void StateAI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.timeLeft = 2;
                if (!player.HasMinionAttackTargetNPC)
                {
                    float maxDis = 800;
                    foreach (NPC npc in Main.npc)
                    {
                        float dis = (npc.position - player.position).Length();
                        if (npc.active && npc.CanBeChasedBy() && !npc.friendly && dis < maxDis)
                        {
                            maxDis = dis;
                            player.MinionAttackTargetNPC = npc.whoAmI;
                        }
                    }
                    Projectile.rotation = MathHelper.PiOver2 - 0.1f;
                    Projectile.spriteDirection = -player.direction;
                    Vector2 pos = player.Center + new Vector2((60 + 20 * Projectile.minionPos) * Projectile.spriteDirection, 10);
                    if (Vector2.Distance(pos, Projectile.Center) < 30)
                    {
                        Projectile.velocity *= 0.4f;
                    }
                    else
                    {
                        Projectile.velocity = (Projectile.velocity * 6 + (pos - Projectile.Center).SafeNormalize(default) * 10f) / 7;
                    }
                }
                else
                {
                    Vector2 pos = Projectile.OwnerMinionAttackTargetNPC.Center;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                    Projectile.spriteDirection = Projectile.direction;
                    if (Projectile.spriteDirection == -1)
                    {
                        Projectile.rotation += MathHelper.PiOver2;
                    }

                    Projectile.velocity = (pos - Projectile.Center).SafeNormalize(default) * 20;
                }
            }
        }
    }
}
