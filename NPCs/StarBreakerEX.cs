using StarBreaker.Projs;
using System.IO;
using Terraria.Graphics.Effects;

namespace StarBreaker.NPCs
{
    public class StarBreakerEX : FSMNPC
    {
        private string[] _sayText = {
        "很好",
        "我们最终还是击败了月球领主",
        "现在,我需要知道我们的关系",
        "我以前的那个主人兼制造者...疯了",
        "他一疯，拳套一走，我们就失去了控制",
        "现在,我要重新击败他",
        "所以我现在要对你进行试炼",
        "不然你怎么能打败他?",
        "我不会放水了",
        "顺带一提,我还藏着4把武器没给你展示过",
        "那么来吧,进行这一场决斗",
        "我会很高兴你能够击败我的"
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
            NPC.damage = 50;
            NPC.defense = 18;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 32;
            NPC.height = 32;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/AttackOfTheKillerQueen");
                SceneEffectPriority = SceneEffectPriority.BossMedium;//曲子优先度
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
                if (NPC.velocity.Y > 20)
                {
                    NPC.active = false;//到达一定的下坠速度就自杀
                }

                return;
            }
            Vector2 toTarget = Target.Center - NPC.Center;//到玩家的单位向量
            NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? 1 : -1;//npc朝向
            int damage = NPC.damage;
            if (Main.expertMode || Main.masterMode)
            {
                damage /= 2;
            }

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
                                _sayText = null;//释放内存,避免再吃更多内存
                                _enCenter = Target.Center;
                                break;
                            }
                            else if (Timer1 / 50 >= 10 && Timer1 / 50 < 11)
                            {
                                int npc1 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X - 100, (int)NPC.position.Y - 500, ModContent.NPCType<StarBreakerEXGunNPC.AntiTankGun>());
                                Main.npc[npc1].realLife = NPC.whoAmI;
                                npc1 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + 100, (int)NPC.position.Y - 500, ModContent.NPCType<StarBreakerEXGunNPC.FocusFlamethrower>());
                                Main.npc[npc1].realLife = NPC.whoAmI;
                                npc1 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X - 100, (int)NPC.position.Y + 500, ModContent.NPCType<StarBreakerEXGunNPC.GatlingGun>());
                                Main.npc[npc1].realLife = NPC.whoAmI;
                                npc1 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + 100, (int)NPC.position.Y + 500, ModContent.NPCType<StarBreakerEXGunNPC.SpikedCannon>());
                                Main.npc[npc1].realLife = NPC.whoAmI;
                            }
                            Main.NewText(_sayText[(int)(Timer1 / 50)], color);
                        }
                        break;
                    }
                case 1://星击随机使用一把武器,对玩家攻击
                    {
                        if (Timer3 == 0)
                        {
                            Timer3 = Main.rand.Next(1, 5);
                            switch (Timer3)
                            {
                                case 1: FightSayText("M-137格林机枪,用你的速度撕裂他!"); break;
                                case 2: FightSayText("反坦克炮,炸烂他的身体!"); break;
                                case 3: FightSayText("你能躲过我的聚焦喷火器?"); break;
                                case 4: FightSayText("FM-92刺弹炮,刺入爆炸!"); break;
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                _ = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<StarShield>(),
                                    0, 0, Main.myPlayer, NPC.whoAmI);
                            }
                        }
                        else
                        {
                            NPC.velocity = (Target.Center - NPC.Center).RealSafeNormalize() * 10;
                            NPC.Center -= NPC.velocity;
                            Timer1++;
                            if (Timer1 > 300)
                            {
                                Timer1 = 0;
                                Timer3 = 0;
                                State++;
                            }
                        }
                        break;
                    }
                case 2://星击自己发射子弹
                    {
                        NPC.velocity = toTarget * 0.015f;
                        Timer1++;
                        if (Timer1 > 20 - (toTarget.Length() * 0.001f))//根据距离微调发射时间
                        {
                            if (Timer2 < 10)
                            {
                                Timer1 = 0;
                                if (Timer2 == 0)//这是说话
                                {
                                    FightSayText("停火!我自己开火");
                                }
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = RandBullets();
                                    for (float i = -5; i <= 5; i++)
                                    {
                                        Vector2 vel = (i.ToRotationVector2() * MathHelper.Pi / 18) + NPC.velocity.RealSafeNormalize();
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel * 10, type, damage, 1.2f, Main.myPlayer);
                                    }
                                    SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                                }
                            }
                            else
                            {
                                Timer1 = Timer2 = 0;
                                State++;
                                break;
                            }
                            Timer2++;
                        }
                        break;
                    }
                case 3://同步行动,开始大火力输出
                    {
                        Timer1++;
                        NPC.velocity = toTarget * 0.015f;
                        if (Timer1 == 10)
                        {
                            FightSayText("同步行动,瞄准他!");
                        }
                        else if (Timer1 == 20)
                        {
                            FightSayText("开火!");
                        }
                        else if (Timer1 > 20 && Timer1 % 30 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = RandBullets();
                                for (float i = -5; i <= 5; i++)
                                {
                                    Vector2 vel = (i.ToRotationVector2() * MathHelper.Pi / 18) + NPC.velocity.RealSafeNormalize();
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel * 10, type, damage, 1.2f, Main.myPlayer);
                                }
                                SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                            }
                            Timer2++;
                            if (Timer2 > 10)
                            {
                                Timer1 = Timer2 = Timer3 = 0;
                                State++;
                            }
                        }
                        break;
                    }
                case 4://包围射击,在玩家运动路程过长后
                    {
                        Timer2 += Target.velocity.Length();
                        NPC.velocity *= 0.9f;
                        Timer1 += 0.5f;
                        if (Timer2 < 800)
                        {
                            if (Timer3 == 0)
                            {
                                FightSayText("我们休息一下,只要你不动,我就无敌在这里,等到时间够长为止,或者你走的够多为止");
                                NPC.dontTakeDamage = true;
                                Timer3++;
                            }
                        }
                        else
                        {
                            Timer1 += 0.5f;
                            if (Timer3 == 1)
                            {
                                FightSayText("休息够了?来让我看看!");
                                Timer3++;
                            }
                        }
                        if (Timer1 > 300)
                        {
                            Timer1 = Timer2 = Timer3 = 0;
                            State++;
                            NPC.dontTakeDamage = false;
                        }
                        break;
                    }
                case 5://难得的冲刺
                    {
                        Timer1--;
                        if (Timer1 <= 0)
                        {
                            if (Timer2 == 0)
                            {
                                FightSayText("我来和你贴贴了~");
                            }
                            else
                            {
                                NPC.velocity = toTarget.RealSafeNormalize() * 30;
                            }

                            Timer2++;
                            Timer1 = 40;
                            if (Timer2 > 5)
                            {
                                Timer1 = Timer2 = 0;
                                State++;
                            }
                        }
                        else if (Timer1 < 20)
                        {
                            NPC.velocity *= 0.85f;
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
        private void FightSayText(string Text)
        {
            PopupText.NewText(new()
            {
                Color = Color.Purple,
                DurationInFrames = 120,
                Text = Text,
                Velocity = Vector2.UnitY * -5
            }, NPC.Center);
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
