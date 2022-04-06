using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.NPCs.StarBreakerEXGunNPC
{
    internal abstract class EXGunNPC : ModNPC
    {
        private Player Target => Main.player[NPC.target];
        private NPC StarBreakerEX_NPC => Main.npc[NPC.realLife];
        private float Timer1 => StarBreakerEX_NPC.ai[0];
        private float Timer2 => StarBreakerEX_NPC.ai[1];
        private float Timer3 => StarBreakerEX_NPC.ai[2];
        private int State => (int)StarBreakerEX_NPC.ai[3];
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 30000;
            NPC.knockBackResist = 0f;
            NPC.defense = 18;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 32;
            NPC.height = 32;
            NPC.dontTakeDamage = true;
        }
        public override void AI()
        {
            if (Target.dead || !Target.active || NPC.target == 255 || NPC.target <= 0)
            {
                NPC.TargetClosest();
            }//获取敌对目标
            if (Target.dead || !Target.active || !StarBreakerEX_NPC.active)
            {
                NPC.velocity.Y -= 0.1f;
                if (NPC.velocity.Y < -20) NPC.active = false;//到达一定的下坠速度就自杀
                return;
            }
            Vector2 toTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);//到玩家的单位向量
            NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? 1 : -1;//npc朝向
            NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);//NPC旋转部分
            switch (State)
            {
                case 0://下落
                    {
                        NPC.velocity.Y = 5;
                        break;
                    }
                case 1://发射小串普通子弹
                    {
                        if (NPC.type == ModContent.NPCType<EXSDMG>())
                        {
                            Vector2 pos = StarBreakerEX_NPC.Center + (StarBreakerEX_NPC.velocity.SafeNormalize(default).RotatedBy(MathHelper.PiOver2) * 50);
                            NPC.velocity = (pos - NPC.Center) / 10;
                        }
                        else
                        {
                            Vector2 pos = StarBreakerEX_NPC.Center + (StarBreakerEX_NPC.velocity.SafeNormalize(default).RotatedBy(MathHelper.PiOver2) * -50);
                            NPC.velocity = (pos - NPC.Center) / 10;
                        }
                        if (Timer1 % 30 == 0)
                        {
                            if (Main.netMode != 1)
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    Vector2 ves = NPC.velocity.SafeNormalize(default) * 5;
                                    Vector2 center = NPC.Center + ((ves.ToRotation() - MathHelper.PiOver2).ToRotationVector2() * i * 10);
                                    int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), center, ves, ProjectileID.Bullet,
                                        50, 2.3f, Main.myPlayer);
                                    Main.projectile[proj].friendly = false;
                                    Main.projectile[proj].hostile = true;
                                }
                            }
                        }
                        break;
                    }
                case 2://远程协助星辰击碎者
                    {
                        if (NPC.type == ModContent.NPCType<EXSDMG>())
                        {
                            Vector2 pos = StarBreakerEX_NPC.Center + (StarBreakerEX_NPC.velocity.SafeNormalize(default).RotatedBy(MathHelper.PiOver2) * 200);
                            NPC.velocity = (pos - NPC.Center) / 10;
                        }
                        else
                        {
                            Vector2 pos = StarBreakerEX_NPC.Center + (StarBreakerEX_NPC.velocity.SafeNormalize(default).RotatedBy(MathHelper.PiOver2) * -200);
                            NPC.velocity = (pos - NPC.Center) / 10;
                        }
                        NPC.rotation = toTarget.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);
                        if (Timer1 % 10 == 0)
                        {
                            if (Main.netMode != 1)
                            {
                                int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, toTarget * 5, ProjectileID.Bullet,
                                    50, 2.3f, Main.myPlayer);
                                Main.projectile[proj].friendly = false;
                                Main.projectile[proj].hostile = true;
                            }
                        }
                        break;
                    }
                case 3://到玩家身边缓慢发射子弹
                    {
                        if (NPC.type == ModContent.NPCType<EXSDMG>())
                        {
                            Vector2 pos = Target.Center + new Vector2(500, 0);
                            NPC.velocity = (pos - NPC.Center) / 10;
                        }
                        else
                        {
                            Vector2 pos = Target.Center + new Vector2(-500, 0);
                            NPC.velocity = (pos - NPC.Center) / 10;
                        }
                        NPC.rotation = toTarget.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);
                        if (Timer1 % 30 == 0)
                        {
                            if (Main.netMode != 1)
                            {
                                int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, toTarget * 5, ProjectileID.Bullet,
                                    50, 2.3f, Main.myPlayer);
                                Main.projectile[proj].friendly = false;
                                Main.projectile[proj].hostile = true;
                            }
                        }
                        break;
                    }
                case 4://跟随星辰击碎者旋转
                    {
                        if (NPC.type == ModContent.NPCType<EXSDMG>())
                        {
                            Vector2 pos = StarBreakerEX_NPC.Center + (StarBreakerEX_NPC.velocity.SafeNormalize(default).RotatedBy(MathHelper.PiOver2) * 100);
                            NPC.velocity = (pos - NPC.Center) / 10;
                        }
                        else
                        {
                            Vector2 pos = StarBreakerEX_NPC.Center + (StarBreakerEX_NPC.velocity.SafeNormalize(default).RotatedBy(MathHelper.PiOver2) * -100);
                            NPC.velocity = (pos - NPC.Center) / 10;
                        }
                        NPC.rotation = toTarget.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);
                        if (Timer3 == 1)
                        {
                            if (Timer1 % 30 == 0)
                            {
                                if (Main.netMode != 1)
                                {
                                    int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (toTarget * 5).RotatedByRandom(0.01f), ProjectileID.Bullet,
                                        50, 2.3f, Main.myPlayer);
                                    Main.projectile[proj].friendly = false;
                                    Main.projectile[proj].hostile = true;
                                }
                            }
                        }
                        break;
                    }
                case 5://摸鱼
                    {
                        if (NPC.type == ModContent.NPCType<EXSDMG>())
                        {
                            Vector2 pos = Target.position + new Vector2(0, -100);
                            NPC.velocity = (pos - NPC.Center) / 10;
                        }
                        else
                        {
                            Vector2 pos = Target.position + new Vector2(0, 100);
                            NPC.velocity = (pos - NPC.Center) / 10;
                        }
                        break;
                    }
                default:
                    NPC.velocity *= 0;
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            #region 控制的线
            Texture2D textureEn = ModContent.Request<Texture2D>("StarBreaker/Projs/Type/EnergyProj").Value;
            Main.spriteBatch.Draw(textureEn, NPC.Center - screenPos, null, Color.Purple * 0.8f,
                -MathHelper.PiOver2, Vector2.Zero, new Vector2(800, 0.8f), SpriteEffects.None, 0f);
            #endregion
            return true;
        }
    }
}
