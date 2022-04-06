using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastSowrd : ModProjectile
    {
        private float State
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float Timer1
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private float Timer2
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        public override string Texture => "StarBreaker/Items/UltimateCopperShortsword/LastShortSowrd";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜短剑");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode == 2)
            {
                writer.Write(Timer2);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != 1)
            {
                Timer2 = reader.ReadSingle();
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            StarPlayer shortSword = player.GetModPlayer<StarPlayer>();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (shortSword.SwordSum)
            {
                Projectile.timeLeft = 2;
            }
            NPC npc = null;
            float max = 2000;
            foreach (NPC n in Main.npc)
            {
                float ToN = Vector2.Distance(n.Center, player.Center);
                if (ToN < max && !n.friendly && n.CanBeChasedBy())
                {
                    max = ToN;
                    npc = n;
                }
            }//追寻目标

            if (!shortSword.SwordTurret)//跟随模式
            {
                if (npc != null)
                {
                    float ToNPC = Vector2.Distance(npc.Center, Projectile.Center);
                    switch (State)
                    {
                        case 0://冲刺
                            {
                                if (Timer1 <= 0)
                                {
                                    Timer1 = 20;
                                    Timer2++;
                                    if (Timer2 > 5)
                                    {
                                        Timer2 = 0;
                                        Timer1 = 0;
                                        State = 1;
                                    }
                                    Projectile.velocity = (npc.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 15;
                                }
                                else Timer1--;
                                break;
                            }
                        case 1://发射剑气
                            {
                                if (ToNPC > 30)
                                {
                                    Projectile.velocity = (npc.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 5;
                                }
                                else
                                {
                                    Projectile.Center = player.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 100;
                                }
                                if (Timer1 <= 0)
                                {
                                    Timer1 = 30;
                                    if (Main.netMode != 1)
                                    {
                                        var proj = Projectile.NewProjectileDirect(null, Projectile.Center, (npc.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 21 + npc.velocity * 0.6f,
                                                ModContent.ProjectileType<LostSword2>(), Projectile.damage, 2.3f, player.whoAmI);
                                        proj.friendly = true;
                                        proj.hostile = false;
                                        proj.ai[0] = 3;
                                    }
                                    Timer2++;
                                }
                                else Timer1--;
                                if (Timer2 > 5)
                                {
                                    Timer2 = 0;
                                    Timer1 = 0;
                                    Projectile.velocity = Vector2.One;
                                    State++;
                                }
                                break;
                            }
                        case 2://旋转,一圈散弹
                            {
                                Projectile.velocity = Projectile.velocity.RotatedBy(0.1f);
                                if (Timer1 % 2 == 0)
                                {
                                    if (Main.netMode != 1)
                                    {
                                        var proj = Projectile.NewProjectileDirect(null, Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5,
                                                ModContent.ProjectileType<LostSword2>(), Projectile.damage, 3.5f, player.whoAmI, 3);
                                        proj.friendly = true;
                                        proj.hostile = false;
                                    }
                                }
                                Timer1++;
                                if (Timer1 > 200)
                                {
                                    State++;
                                    Timer1 = 0;
                                }
                                break;
                            }
                        default:
                            State = 0;
                            break;
                    }


                }
                else
                {
                    Projectile.velocity = (Projectile.velocity * 99 +
                        (player.Center - Projectile.Center).SafeNormalize(Vector2.One) * 10) / 100;
                }
            }
            else//炮塔模式
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.position = player.position + (Projectile.position - player.position).SafeNormalize(default) * 80;
                Projectile.rotation = (player.position - Projectile.position).ToRotation() + MathHelper.PiOver4;
                if (npc != null)
                {
                    Projectile.rotation = (npc.position - Projectile.position).ToRotation() + MathHelper.PiOver4;
                    Timer1++;
                    Timer2 = 0;
                    State = 0;
                    if (Timer1 > 10)
                    {
                        Timer1 = 0;
                        if (Main.netMode != 1)
                        {
                            for (int rand = Main.rand.Next(1, 5); rand > 0; rand--)
                            {
                                var proj = Projectile.NewProjectileDirect(null, Projectile.Center, (npc.Center - Projectile.Center).SafeNormalize(Vector2.UnitX).RotatedByRandom(0.1f) * 18 + npc.velocity * 0.6f,
                                        ModContent.ProjectileType<LostSword2>(), Projectile.damage, 2.3f, player.whoAmI);
                                proj.friendly = true;
                                proj.hostile = false;
                                proj.ai[0] = 3;
                            }
                        }
                    }
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float s = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20,
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * -20,
                6, ref s);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[Projectile.owner];
            StarPlayer shortSword = player.GetModPlayer<StarPlayer>();
            damage += shortSword.PlayerEmotion;
        }
    }
}
