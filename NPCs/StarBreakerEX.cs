using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.NPCs
{
    public class StarBreakerEX : FSMNPC
    {
        private readonly string[] _sayText = {
        "很好",
        "我们最终还是击败了某个boss",
        "现在，我需要新的 主人",
        "我以前的那个主人兼制造者...疯了",
        "他一疯，拳套一走，我们就失去了控制",
        "现在,我要重新击败他",
        "什么?我要主人做什么?",
        "xswl，我也不知道",
        "终于有个像样的对手了,我可不会放水了",
        "顺带一提,我忘记给你看看我的特殊能力了",
        "我的魔法可以操控两把枪",
        "温馨提示:不要撞墙"
        };
        private Vector2 _enCenter;
        public override string Texture => "StarBreaker/NPCs/StarBreakerN";
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰击碎者");
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server)
            {
                writer.WriteVector2(_enCenter);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                _enCenter = reader.ReadPackedVector2();
            }
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.lifeMax = (Main.masterMode) ? 120000 : (Main.expertMode) ? 60000 : 30000;
            NPC.knockBackResist = 0f;
            NPC.defense = 18;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 32;
            NPC.height = 32;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/AttackOfTheKillerQueen");
            }
        }
        public override void AI()
        {
            if (Target.dead || !Target.active || NPC.target == 255 || NPC.target <= 0)
            {
                NPC.TargetClosest();
            }//获取敌对目标

            StarGlobalNPC.StarBreaker = NPC.whoAmI;//改变GlobalNPC的一个静态字段
            NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);//NPC旋转部分

            if (!SkyManager.Instance["StarBreaker:StarSky"].IsActive())//开启天空
            {
                SkyManager.Instance.Activate("StarBreaker:StarSky");
            }
            if (Target.dead || !Target.active)
            {
                NPC.velocity.Y += 0.1f;
                if (SkyManager.Instance["StarBreaker:StarSky"].IsActive())//关闭天空
                {
                    SkyManager.Instance.Deactivate("StarBreaker:StarSky");
                }
                if (NPC.velocity.Y > 20) NPC.active = false;//到达一定的下坠速度就自杀
                return;
            }
            Vector2 toTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);//到玩家的单位向量
            NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? 1 : -1;//npc朝向
            Enchantment();
            switch (State)
            {
                case 0://开幕
                    {
                        NPC.dontTakeDamage = true;
                        Timer1++;
                        NPC.rotation = toTarget.ToRotation();
                        Color color = Color.Purple;
                        if (Timer1 % 50 == 0)
                        {
                            if (Timer1 / 50 >= 12)
                            {
                                Timer1 = 0;
                                State++;
                                NPC.dontTakeDamage = false;
                                _enCenter = Target.Center;
                                break;
                            }
                            else if (Timer1 / 50 >= 10 && Timer1 / 50 < 11)
                            {
                                int npc1 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.position.X - 100, (int)NPC.position.Y - 500, ModContent.NPCType<StarBreakerEXGunNPC.EXSDMG>());
                                Main.npc[npc1].realLife = NPC.whoAmI;
                                int npc2 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.position.X + 100, (int)NPC.position.Y - 500, ModContent.NPCType<StarBreakerEXGunNPC.EXVortexBeater>());
                                Main.npc[npc2].realLife = NPC.whoAmI;
                            }
                            Main.NewText(_sayText[(int)(Timer1 / 50)], color);
                        }
                        break;
                    }
                case 1://发射平行散弹
                    {
                        Timer1++;
                        float speed = (float)Math.Sqrt((NPC.Center - Target.Center).Length());
                        NPC.velocity = speed * toTarget * 0.2f;
                        if (Timer1 - speed > 40)
                        {
                            int bullet = RandBullets();
                            if (Main.netMode != 1)
                            {
                                for (int i = -5; i <= 5; i++)
                                {
                                    Vector2 ves = NPC.velocity.SafeNormalize(default) * 5;
                                    Vector2 center = NPC.Center + ((ves.ToRotation() - MathHelper.PiOver2).ToRotationVector2() * i * 10);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), center, ves, bullet,
                                        50, 2.3f, Main.myPlayer);
                                }
                                SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                            }
                            Timer2++;
                            Timer1 = 0;
                            if (Timer2 > 10)
                            {
                                Timer2 = 0;
                                State++;
                            }
                        }
                        break;
                    }
                case 2://发射星星
                    {
                        Timer1++;
                        float speed = (float)Math.Sqrt((NPC.Center - Target.Center).Length());
                        NPC.velocity = speed * toTarget * 0.2f;
                        if (Timer1 > 30 && Main.netMode != 1)
                        {
                            int damage = Main.rand.Next(100, 120) / (Main.expertMode ? 2 : 1);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, toTarget * 5,
                                 RandBullets(), damage, 1.5f, Main.myPlayer);
                            Timer1 = 0;
                            Timer2++;
                            SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                        }
                        else if (Timer2 > 5)
                        {
                            Timer1 = 0;
                            Timer2 = 0;
                            State++;
                        }
                        break;
                    }
                case 3://拼刺刀!
                    {
                        if (Timer1 <= 0)
                        {
                            Timer1 = 50;
                            NPC.velocity = toTarget * 20;
                            Timer2++;
                            if (Timer2 > 5)
                            {
                                Timer1 = 0;
                                Timer2 = 0;
                                State++;
                            }
                        }
                        else Timer1--;
                        if (Vector2.Distance(Target.position, NPC.position) < 36)
                        {
                            Target.statLife -= 20;
                            CombatText.NewText(Target.Hitbox, Color.Red, 20);
                        }
                        break;
                    }
                case 4://发射激光
                    {
                        switch (Timer3)
                        {
                            case 0://使速度变低
                                {
                                    NPC.velocity *= 0.9f;
                                    if (NPC.velocity.Length() < 0.5f) Timer3++;
                                    break;
                                }
                            case 1://发射一圈激光
                                {
                                    NPC.position = NPC.oldPosition;
                                    if (NPC.velocity.Length() < 0.5f)
                                    {
                                        NPC.velocity = toTarget * 5;
                                    }
                                    else
                                    {
                                        NPC.velocity = NPC.velocity.RotatedBy(0.02f);
                                        Timer1++;
                                        if (Main.netMode != 1)
                                        {
                                            int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, NPC.velocity.RotatedBy(-0.02f),
                                                ModContent.ProjectileType<Projs.StarLine>(), 100, 1.2f, Main.myPlayer);
                                            Main.projectile[proj].friendly = false;
                                            Main.projectile[proj].hostile = true;
                                        }
                                        if (Timer1 > 360)
                                        {
                                            Timer1 = 0;
                                            Timer3 = 0;
                                            State++;
                                        }
                                    }

                                    break;
                                }
                        }
                        break;
                    }
                case 5://星辰 弹幕
                    {
                        if (!NPC.dontTakeDamage)
                        {
                            NPC.dontTakeDamage = true;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Timer2 = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), _enCenter, Vector2.Zero, ModContent.ProjectileType<Projs.ProjDrawStar>(), 1, 1, Main.myPlayer, _enCenter.X, _enCenter.Y);
                            }

                        }
                        else
                        {
                            NPC.velocity *= 0.85f;
                            if (!Main.projectile[(int)Timer2].active || Main.projectile[(int)Timer2].type != ModContent.ProjectileType<Projs.ProjDrawStar>())
                            {
                                NPC.dontTakeDamage = false;
                                Timer2 = 0;
                                State++;
                                NPC.velocity = Vector2.One;
                            }
                        }
                        break;
                    }
                default:
                    {
                        State = 1;
                        break;
                    }
            }

        }
        public override void OnKill()
        {
            StarBreakerSystem.downedStarBreakerEX = true;        
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            #region 旧位置储存
            for (int j = NPC.oldRot.Length - 1; j > 0; j--)
            {
                NPC.oldRot[j] = NPC.oldRot[j - 1];
            }
            NPC.oldRot[0] = NPC.rotation;
            #endregion
            #region 结界绘制
            if (State != 0)
            {
                Texture2D textureEn = ModContent.Request<Texture2D>("StarBreaker/Projs/Type/EnergyProj").Value;
                for (int i = 0; i < 4; i++)
                {
                    float rot = 0 + i * MathHelper.PiOver2 + MathHelper.PiOver2;
                    Vector2 center = _enCenter + ((i * MathHelper.PiOver2).ToRotationVector2() * 1000) - screenPos;
                    Main.spriteBatch.Draw(textureEn, center, null, Color.Purple,
                        rot, textureEn.Size() * 0.5f, new Vector2(800, 1), SpriteEffects.None, 0f);
                }
            }
            #endregion
            #region 残影
            for (int i = 0; i < NPC.oldPos.Length; i += 2)
            {
                Texture2D NPCTexture = TextureAssets.Npc[NPC.type].Value;
                int frameCount = Main.npcFrameCount[NPC.type];
                Vector2 DrawOrigin = new Vector2(NPCTexture.Width / 2, NPCTexture.Height / frameCount / 2);
                Vector2 DrawPosition = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                DrawPosition -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * NPC.scale / 2f;
                DrawPosition += DrawOrigin * NPC.scale + new Vector2(0f, 4f + NPC.gfxOffY);
                Main.spriteBatch.Draw(NPCTexture,
                    DrawPosition,
                    new Rectangle?(NPC.frame),
                    new Color(0.5f, 0.5f, i / 6, (NPC.oldPos.Length - i) / NPC.oldPos.Length),
                    NPC.oldRot[i],
                    DrawOrigin,
                    1f,
                    NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0f);
            }
            #endregion
            return true;
        }
        private void Enchantment()//结界
        {
            if (State == 0)
            {
                return;
            }
            if (Target.Center.Y > (_enCenter + new Vector2(0, 1000)).Y)
            {
                Target.velocity.Y = 0;
                Target.gravDir = 0;
                Target.gravity = 0;
            }
            else if (Target.Center.Y < (_enCenter + new Vector2(0, -1000)).Y)
            {
                Target.velocity.Y = 0;
            }
            if (Target.Center.X < (_enCenter + new Vector2(-1000, 0)).X || Target.Center.X > (_enCenter + new Vector2(1000, 0)).X)
            {
                Target.velocity.X = 0;
            }
            if (Target.Center.X - (_enCenter + new Vector2(-1000, 0)).X < 5 || Target.Center.X - (_enCenter + new Vector2(1000, 0)).X > 5
                || Target.Center.Y - (_enCenter + new Vector2(0, -1000)).Y < 5 || Target.Center.Y - (_enCenter + new Vector2(0, 1000)).Y > 5)
            {
                Target.position = _enCenter;
            }
        }
        private static int RandBullets()//随机子弹
        {
            int[] bullets = new int[4]
            {
                ModContent.ProjectileType<Projs.Bullets.SunBullet>(),
                ModContent.ProjectileType<Projs.Bullets.StarSwirlBullet>(),
                ModContent.ProjectileType<Projs.Bullets.StardustBullet>(),
                ModContent.ProjectileType<Projs.Bullets.NebulaBullet>(),
            };
            return bullets[Main.rand.Next(4)];
        }
    }
}
