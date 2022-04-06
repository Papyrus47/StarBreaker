using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class StarBreakerProj : ModProjectile
    {
        private NPC target;
        private float State
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override string Texture => "StarBreaker/Items/Weapon/StarBreakerW";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰击碎者");
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = 40;
            Projectile.width = 142;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Ranged;

        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode == 2)
            {
                writer.Write(Projectile.localAI[0]);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != 1)
            {
                Projectile.localAI[0] = reader.ReadSingle();
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            StarPlayer starPlayer = player.GetModPlayer<StarPlayer>();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            Lighting.AddLight(Projectile.Center, new Vector3(1, 1, 1));
            if (starPlayer.SummonStarWenpon)
            {
                Projectile.timeLeft = 2;
            }
            if (player.active && !player.dead)
            {
                if (target == null)
                {
                    float max = 1000;
                    foreach (NPC npc in Main.npc)
                    {
                        float dis = Vector2.Distance(npc.Center, player.Center);
                        if (player.statLife > player.statLifeMax2 * 0.8f)
                        {
                            if (max > dis && npc.active && npc.CanBeChasedBy() && !npc.friendly && npc.realLife == -1)
                            {
                                target = npc;
                                break;
                            }
                            else if (max > dis && npc.active && npc.CanBeChasedBy() && !npc.friendly && npc.realLife != -1)
                            {
                                if (npc.type == NPCID.SolarCrawltipedeTail)
                                {
                                    target = npc;
                                    break;
                                }
                                NPC tar = Main.npc[npc.realLife];
                                if (tar.active && tar.CanBeChasedBy() && !tar.friendly)
                                {
                                    target = tar;
                                    break;
                                }
                                break;
                            }
                        }//血量高，随机选择一个在范围内的敌人打
                        else
                        {
                            if (dis < max && npc.active && !npc.friendly && npc.CanBeChasedBy() && Collision.CanHit(player.position, 1, 1, npc.position, 1, 1))
                            {
                                max = dis;
                                target = npc;
                            }
                        }//血量低，瞄准最近的敌人
                    }
                    if (target == null)
                    {
                        Projectile.Center = player.Center + new Vector2(0, -200);
                        Projectile.velocity = player.velocity;
                        if (Projectile.velocity == Vector2.Zero) Projectile.velocity.X = 1;
                    }
                }
                else
                {
                    if (!target.active || !target.CanBeChasedBy() || (!Collision.CanHit(player.position, 1, 1, target.position, 1, 1)) && player.statLife <= player.statLifeMax2 * 0.8f)
                    {
                        target = null;
                        return;
                    }
                    if (player.statLife < player.statLifeMax2 * 0.5f)//切换射击状态
                    {
                        State = 3;
                    }
                    else if (State == 3)
                    {
                        State = 0;
                    }
                    switch (State)
                    {
                        case 0://对着npc发射弹幕
                            {
                                Timer++;
                                float speed = (float)Math.Sqrt((Projectile.Center - target.Center).Length());
                                Projectile.velocity = Vector2.Normalize(target.Center - Projectile.Center) * speed * 0.8f;
                                if (Projectile.Distance(target.position) < 200)
                                {
                                    Projectile.position -= Projectile.velocity;
                                }
                                if (Timer % 15 == 0)
                                {
                                    ShootAmmo(player);
                                }
                                else if (Timer > 200)
                                {
                                    State++;
                                    Timer = 0;
                                }
                                break;
                            }
                        case 1://十字散弹
                            {
                                Timer++;
                                float speed = (float)Math.Sqrt((Projectile.Center - target.Center).Length());
                                Projectile.velocity = speed * Vector2.Normalize(target.Center - Projectile.Center) * 0.3f;
                                if (Timer % 3 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 ves;
                                    if (Main.rand.NextBool())
                                    {
                                        ves.X = Projectile.rotation.ToRotationVector2().X * (Main.rand.NextBool() ? -10 : 10);
                                        ves.Y = 0;
                                    }
                                    else
                                    {
                                        ves.X = 0;
                                        ves.Y = Projectile.rotation.ToRotationVector2().Y * (Main.rand.NextBool() ? -10 : 10);
                                    }
                                    if (Main.netMode != 1)
                                    {
                                        ShootAmmo(player);
                                    }
                                }
                                else if (Timer > 80)
                                {
                                    State++;
                                    Timer = 0;
                                }
                                break;
                            }
                        case 2://Dna弹幕？
                            {
                                Timer++;
                                if (Timer % 30 == 0 && Main.netMode != 1)
                                {
                                    Projectile.velocity = Vector2.Normalize(target.Center - Projectile.Center) * 8f;
                                    for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.PiOver2)
                                    {
                                        for (float j = -1; j <= 1; j++)
                                        {
                                            Vector2 center1 = (i.ToRotationVector2() * 50) + target.Center;
                                            Vector2 realCenter = new Vector2(center1.X - (j * 60), center1.Y);
                                            if (Main.netMode != 1)
                                            {
                                                ShootAmmo(player);
                                            }
                                        }
                                    }
                                    SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                                }
                                else if (Timer > 130)
                                {
                                    Timer = 0;
                                    State = 0;
                                }
                                break;
                            }
                        case 3://保护玩家的射击
                            {
                                Projectile.velocity = Vector2.Normalize(target.Center - Projectile.Center);
                                Projectile.Center = player.Center + (Projectile.Center - player.Center).SafeNormalize(default) * 200;
                                Projectile.position -= Projectile.velocity;
                                Timer++;
                                if (Timer > 8)
                                {
                                    int[] bullets = new int[4]
               {
                ModContent.ProjectileType<Bullets.SunBullet>(),
                ModContent.ProjectileType<Bullets.StarSwirlBullet>(),
                ModContent.ProjectileType<Bullets.StardustBullet>(),
                ModContent.ProjectileType<Bullets.NebulaBullet>(),
               };
                                    int shoot = bullets[Main.rand.Next(0, 4)];
                                    SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                                    for (float i = -10; i <= 10; i++)
                                    {
                                        Vector2 center = Projectile.Center + (Projectile.velocity.RotatedBy(i * 0.1f) * 50);
                                        int proj = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 10, shoot, 100 + Projectile.damage, Projectile.knockBack, player.whoAmI);
                                        Main.projectile[proj].friendly = true;
                                        Main.projectile[proj].hostile = false;
                                    }
                                    Timer = 0;
                                }
                                break;
                            }
                    }
                }
            }
            else
            {
                Projectile.velocity *= 0f;
                Projectile.rotation = Main.GlobalTimeWrappedHourly;
                Timer++;
                if (Timer > 5)
                {
                    int[] bullets = new int[4]
    {
                ModContent.ProjectileType<Bullets.SunBullet>(),
                ModContent.ProjectileType<Bullets.StarSwirlBullet>(),
                ModContent.ProjectileType<Bullets.StardustBullet>(),
                ModContent.ProjectileType<Bullets.NebulaBullet>(),
    };
                    int shoot = bullets[Main.rand.Next(0, 4)];
                    SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                    for (float i = -5; i <= 5; i++)
                    {
                        Vector2 vec = (i + Projectile.rotation).ToRotationVector2() * MathHelper.Pi / 5;
                        int proj = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, vec * 10, shoot, 100 + Projectile.damage, Projectile.knockBack, player.whoAmI);
                        Main.projectile[proj].friendly = true;
                        Main.projectile[proj].hostile = true;
                    }
                    Timer = 0;
                }
            }//死亡攻击
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation -= MathHelper.Pi;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            if (State == 3)
            {
                target.velocity -= target.velocity * 0.85f;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Vector2.Distance(Projectile.Center, targetHitbox.TopLeft()) < 20;
        }
        private void ShootAmmo(Player player)
        {
            int[] bullets = new int[4]
            {
                ModContent.ProjectileType<Bullets.SunBullet>(),
                ModContent.ProjectileType<Bullets.StarSwirlBullet>(),
                ModContent.ProjectileType<Bullets.StardustBullet>(),
                ModContent.ProjectileType<Bullets.NebulaBullet>(),
            };
            int shoot = bullets[Main.rand.Next(0, 4)];
            SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
            for (float i = -5; i <= 5; i++)
            {
                Vector2 vec = (i.ToRotationVector2() * MathHelper.Pi / 18) + Projectile.velocity.SafeNormalize(Vector2.Zero);
                int proj = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, vec * 10, shoot, 100 + Projectile.damage, Projectile.knockBack, player.whoAmI, 0, -114);
                Main.projectile[proj].friendly = true;
                Main.projectile[proj].hostile = false;
            }

        }
    }
}
