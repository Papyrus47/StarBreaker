using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class BurnFistProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("炎拳");
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 500;
            Projectile.scale = 2;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4;
            Projectile.spriteDirection = Projectile.direction;
            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation -= MathHelper.PiOver2;
            }
            if (player.active)
            {
                Projectile.timeLeft = 2;
            }
            if (player.HeldItem.type == ModContent.ItemType<Items.Weapon.FrostFistW>())
            {
                if (player.HeldItem.ModItem is Items.Weapon.FrostFistW frostFist)
                {
                    if (frostFist.frostFistProjWhoAmI > -1 && Projectile.ai[0] != Main.projectile[frostFist.frostFistProjWhoAmI].ai[0])
                    {
                        Projectile.ai[0] = Main.projectile[frostFist.frostFistProjWhoAmI].ai[0];
                        Projectile.ai[1] = 0;
                    }
                }
                switch (Projectile.ai[0])
                {
                    case -1://呆在玩家后背
                    case 0://发射火焰
                    case 1://高速火
                    default:
                        {
                            Vector2 pos = player.Center + new Vector2(-30 * player.direction, -30);
                            Projectile.velocity = (pos - Projectile.position) * 0.2f;
                            switch (Projectile.ai[0])
                            {
                                case 0:
                                    {
                                        Projectile.ai[1]++;
                                        if (Projectile.ai[1] > 20)
                                        {
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                            {
                                                for (int i = -1; i <= 1; i++)
                                                {
                                                    Vector2 vel = (Main.MouseWorld - Projectile.position).SafeNormalize(default).RotatedBy(i * 0.1f) * 10;
                                                    Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel,
                                                        ModContent.ProjectileType<FireFist>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                                    projectile.friendly = true;
                                                    projectile.hostile = false;
                                                }
                                            }
                                            Projectile.ai[1] = 0;
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        Projectile.ai[1]++;
                                        if (Projectile.ai[1] > 5)
                                        {
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                            {
                                                Vector2 vel = (Main.MouseWorld - Projectile.position).SafeNormalize(default) * 10;
                                                Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel,
                                                    ModContent.ProjectileType<FireFist>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                                projectile.friendly = true;
                                                projectile.hostile = false;

                                            }
                                            Projectile.ai[1] = 0;
                                        }

                                        break;
                                    }
                            }
                            break;
                        }
                    case 5://抓取
                        {
                            if (player.whoAmI == Main.myPlayer)
                            {
                                Projectile.velocity = (Projectile.velocity * 10 + ((Main.MouseWorld - Projectile.position).SafeNormalize(default) * 10)) / 11;
                            }
                            if (Projectile.ai[1] <= 0)
                            {
                                for (float r = 0; r <= MathHelper.TwoPi; r += MathHelper.Pi / 20)
                                {
                                    Vector2 center = Projectile.position + (r.ToRotationVector2() * 50);
                                    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.FireworkFountain_Red);
                                    dust.velocity *= 0f;
                                    dust.noGravity = true;
                                }
                            }
                            if (!player.HasMinionAttackTargetNPC)
                            {
                                float max = 50;
                                foreach (NPC npc in Main.npc)
                                {
                                    float dis = Vector2.Distance(Projectile.position, npc.position);
                                    if (npc.active && npc.CanBeChasedBy() && !npc.friendly && !npc.immortal && dis < max)
                                    {
                                        player.MinionAttackTargetNPC = npc.whoAmI;
                                        max = dis;
                                    }
                                }
                            }
                            else
                            {
                                if (!Projectile.OwnerMinionAttackTargetNPC.active)
                                {
                                    player.MinionAttackTargetNPC = -1;
                                    break;
                                }
                                Projectile.ai[1]++;
                                if (Projectile.ai[1] > 120)
                                {
                                    Projectile.ai[1] = 0;
                                    Projectile.ai[0] = -1;
                                }
                                Projectile.OwnerMinionAttackTargetNPC.position = Projectile.Center;
                                Projectile.OwnerMinionAttackTargetNPC.immune[player.whoAmI] = 0;
                                for (int i = 0; i < 2; i++)
                                {
                                    Projectile projectile = Projectile.NewProjectileDirect(null, Projectile.Center + Projectile.velocity,
                                        (Projectile.OwnerMinionAttackTargetNPC.position - Projectile.position).SafeNormalize(default).RotateRandom(0.3) * 5, ModContent.ProjectileType<OuLa>(),
                                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                                    projectile.friendly = true;
                                    projectile.hostile = false;
                                    projectile.alpha = 255;
                                }

                            }
                            break;
                        }
                }
            }
            else if (player.HeldItem.type == ModContent.ItemType<Items.Weapon.BurnFistW>())
            {
                switch (Projectile.ai[0])
                {
                    case -1://呆在玩家后背
                        {
                            Vector2 pos = player.Center + new Vector2(-80 * player.direction, -30);
                            Projectile.velocity = (pos - Projectile.position) * 0.2f;
                            break;
                        }
                    case 0://消耗一定魔力,发射火焰
                        {
                            Vector2 pos = player.Center + new Vector2(0, -200);
                            Projectile.velocity = (pos - Projectile.position) * 0.2f;
                            if (Projectile.spriteDirection == 1)
                            {
                                Projectile.rotation -= MathHelper.PiOver2;
                            }
                            if (player.whoAmI == Main.myPlayer)
                            {
                                Projectile.ai[1]++;
                                if (Projectile.ai[1] % 20 == 0)
                                {
                                    if (Projectile.ai[1] > 120)//回归状态
                                    {
                                        Projectile.ai[1] = 0;
                                        Projectile.ai[0] = -1;
                                    }
                                    player.statMana -= 20;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        for (int i = -1; i <= 1; i++)
                                        {
                                            Vector2 vel = (Main.MouseWorld - Projectile.position).SafeNormalize(default).RotatedBy(i * 0.1f) * 10;
                                            Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel,
                                                ModContent.ProjectileType<FireFist>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                            projectile.friendly = true;
                                            projectile.hostile = false;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case 1://幻影拳
                        {
                            Projectile.ai[1]++;
                            if (player.whoAmI == Main.myPlayer)
                            {
                                Projectile.velocity = (Main.MouseWorld - Projectile.position) * 0.05f;
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Projectile projectile = Projectile.NewProjectileDirect(null, Projectile.Center + Projectile.velocity, Projectile.velocity.RotateRandom(0.3), ModContent.ProjectileType<OuLa>(),
                                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                                    projectile.friendly = true;
                                    projectile.hostile = false;
                                    projectile.alpha = 255;
                                }
                            }
                            if (Projectile.ai[1] > 120)
                            {
                                Projectile.ai[0] = -1;
                                Projectile.ai[1] = 0;
                            }
                            break;
                        }
                    case 2://抓取
                        {
                            if (player.whoAmI == Main.myPlayer)
                            {
                                Projectile.velocity = (Projectile.velocity * 10 + ((Main.MouseWorld - Projectile.position).SafeNormalize(default) * 10)) / 11;
                            }
                            if (Projectile.ai[1] <= 0)
                            {
                                for (float r = 0; r <= MathHelper.TwoPi; r += MathHelper.Pi / 20)
                                {
                                    Vector2 center = Projectile.position + (r.ToRotationVector2() * 50);
                                    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.FireworkFountain_Red);
                                    dust.velocity *= 0f;
                                    dust.noGravity = true;
                                }
                            }
                            if (!player.HasMinionAttackTargetNPC)
                            {
                                float max = 50;
                                foreach (NPC npc in Main.npc)
                                {
                                    float dis = Vector2.Distance(Projectile.position, npc.position);
                                    if (npc.active && npc.CanBeChasedBy() && !npc.friendly && !npc.immortal && dis < max)
                                    {
                                        player.MinionAttackTargetNPC = npc.whoAmI;
                                        max = dis;
                                    }
                                }
                            }
                            else
                            {
                                if (!Projectile.OwnerMinionAttackTargetNPC.active)
                                {
                                    player.MinionAttackTargetNPC = -1;
                                    break;
                                }
                                Projectile.ai[1]++;
                                if (Projectile.ai[1] > 120)
                                {
                                    Projectile.ai[1] = 0;
                                    Projectile.ai[0] = -1;
                                }
                                Projectile.OwnerMinionAttackTargetNPC.position = Projectile.Center;
                                for (int i = 0; i < 2; i++)
                                {
                                    Projectile projectile = Projectile.NewProjectileDirect(null, Projectile.Center + Projectile.velocity,
                                        (Projectile.OwnerMinionAttackTargetNPC.position - Projectile.position).SafeNormalize(default).RotateRandom(0.3) * 5, ModContent.ProjectileType<OuLa>(),
                                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                                    projectile.friendly = true;
                                    projectile.hostile = false;
                                    projectile.alpha = 255;
                                }

                            }
                            break;
                        }
                    default:
                        {
                            Projectile.ai[0] = -1;
                            break;
                        }
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
    }
}
