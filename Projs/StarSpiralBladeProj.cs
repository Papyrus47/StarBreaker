using StarBreaker.Items.Weapon;

namespace StarBreaker.Projs
{
    public class StarSpiralBladeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰旋刃");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.width = Projectile.height = 112;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.localAI[0];
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[0] += 0.1f;
                if (Projectile.localAI[0] > 30)
                {
                    Projectile.localAI[1]++;
                }
            }
            else
            {
                Projectile.localAI[0] -= 0.1f;
                if (Projectile.localAI[0] < -30)
                {
                    Projectile.localAI[1]--;
                }
            }
            Player player = Main.player[Projectile.owner];
            Projectile.damage = player.GetWeaponDamage(player.HeldItem) + (int)Math.Abs(Projectile.localAI[0] * 5);
            if (Main.mouseLeft && player.HeldItem.type == ModContent.ItemType<StarSpiralBlade>() && Projectile.timeLeft < 170)
            {
                Projectile.timeLeft = 5;
            }
            else if (player.ownedProjectileCounts[Type] > 1)
            {
                Projectile.Kill();
            }
            if (Projectile.timeLeft < 100)
            {
                Projectile.timeLeft = 5;
                Projectile.velocity = (player.Center - Projectile.Center).RealSafeNormalize() * 30;
                if (Vector2.Distance(player.Center, Projectile.Center) < 112)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (player.HasMinionAttackTargetNPC)
                {
                    if (!Projectile.OwnerMinionAttackTargetNPC.CanBeChasedBy())
                    {
                        player.MinionAttackTargetNPC = -1;
                        return;
                    }
                    if (Projectile.timeLeft < 150)
                    {
                        Projectile.timeLeft = 200;
                    }

                    Projectile.velocity = (Projectile.velocity * 9f + (Projectile.OwnerMinionAttackTargetNPC.Center - Projectile.Center) * 0.3f) / 10f;
                }
                else
                {
                    float max = 3000;
                    foreach (NPC npc in Main.npc)
                    {
                        float dis = Vector2.Distance(npc.Center, Projectile.Center);
                        if (npc.active && !npc.friendly && npc.CanBeChasedBy() && dis < max)
                        {
                            max = dis;
                            player.MinionAttackTargetNPC = npc.whoAmI;
                        }
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.immortal)
            {
                return;
            }

            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            target.GetGlobalNPC<NPCs.StarGlobalNPC>().StarSpiralBladeProj = Projectile.whoAmI;
            if (Projectile.localAI[1] == 0)
            {
                int dama = (int)((damage * 5f) + Math.Abs(Projectile.localAI[0] * 10) - target.defense);
                if (dama <= 0)
                {
                    dama = 1;
                }

                Main.player[Projectile.owner].addDPS((int)target.StrikeNPC(dama, knockback, 10));
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, target.whoAmI, dama);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int dama = (int)((damage * 2.5f) + Math.Abs(Projectile.localAI[0] * 10) - target.defense);
                    if (dama <= 0)
                    {
                        dama = 1;
                    }

                    Main.player[Projectile.owner].addDPS((int)target.StrikeNPC(dama, knockback, 10));
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, target.whoAmI, dama);
                    }
                }
                target.HitEffect(0, 10);
                target.checkDead();
            }
        }
    }
}
