using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.NPCs
{
    [AutoloadBossHead]
    public class StarBreakerN : ModNPC
    {
        private float Timer1
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        private float Timer2
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        private int State
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        private Player Target => Main.player[NPC.target];
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰击碎者");
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.lifeMax = 30000;
            NPC.knockBackResist = 0f;
            NPC.defense = 18;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 50;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/AttackOfTheKillerQueen");
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange( //添加信息
                new IBestiaryInfoElement[]//创建一个数组
                {
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,//说明本npc为星空
                    new FlavorTextBestiaryInfoElement("星辰击碎者,因为受到他的主人的影响而被迫远离他的主人,他原来的主人的强度不可忽视")//信息
                }
                );
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapon.StarBreakerW>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapon.EnergyConverter>()));
        }
        public override void OnKill()
        {
            StarBreakerSystem.downedStarBreakerNom = true;
            StarBreakerSystem.sommonBossTime = -1;
        }
        public override void BossHeadRotation(ref float rotation) => rotation = NPC.rotation;

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects) => spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        public override void AI()
        {
            #region 状态机前执行的内容
            if (Target.dead || !Target.active || NPC.target == 255 || NPC.target <= 0)
            {
                NPC.TargetClosest();
            }
            StarGlobalNPC.StarBreaker = NPC.whoAmI;//改变GlobalNPC的一个静态字段

            if (!SkyManager.Instance["StarBreaker:StarSky"].IsActive())//开启天空
            {
                SkyManager.Instance.Activate("StarBreaker:StarSky");
            }

            Vector2 toTarget = Target.Center - NPC.Center;
            NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? 1 : -1;
            NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);//NPC旋转部分

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
            int damage = 50 / (Main.expertMode || Main.masterMode ? 2 : 1);
            #endregion
            switch (State)
            {
                case 0://开幕ai
                    {
                        NPC.dontTakeDamage = true;
                        Timer1++;
                        NPC.rotation = toTarget.ToRotation();
                        Color color = Color.Purple;
                        switch (Timer1)
                        {
                            case 50:
                                {
                                    Main.NewText("如果你真的要去面对月球领主", color);
                                    break;
                                }
                            case 100:
                                {
                                    Main.NewText("我想看看你的实力", color);
                                    break;
                                }
                            case 150:
                                {
                                    Main.NewText("乖乖站好,让我射死你", color);
                                    Timer1 = 0;
                                    State = 1;
                                    NPC.dontTakeDamage = false;
                                    break;
                                }
                        }
                        break;
                    }
                case 1://散射10发
                    {
                        NPC.velocity = toTarget * 0.015f;
                        Timer1++;
                        if(Timer1 > 20 - (toTarget.Length() * 0.01f))//根据距离微调发射时间
                        {
                            if(Timer2 < 10)
                            {
                                Timer1 = 0;
                                if(Timer2 == 0)//这是说话
                                {
                                    PopupText.NewText(new()
                                    {
                                        Color = Color.Purple,
                                        Text = "不要乱动,我在瞄准你",
                                        DurationInFrames = 120,
                                        Velocity = Vector2.UnitY * -5
                                    }, NPC.Center);
                                }
                                if(Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = RandBullets();
                                    for (float i = -5;i<=5;i++)
                                    {
                                        Vector2 vel = (i.ToRotationVector2() * MathHelper.Pi / 18) + NPC.velocity.RealSafeNormalize();
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vel * 10,type, damage, 1.2f, Main.myPlayer);
                                    }
                                    SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                                }
                            }
                            else
                            {
                                Timer1 = Timer2 = 0;
                                State++;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(),NPC.Center,Vector2.Zero,ModContent.ProjectileType<Projs.StarShield>(),0,0,Main.myPlayer,NPC.whoAmI);//生成护盾
                                }
                                break;
                            }
                            Timer2++;
                        }
                        break;
                    }
                case 2://超级 散射
                    {
                        Timer1++;
                        NPC.velocity *= 0.9f;
                        if(Timer1 == 30)//说一句话
                        {
                            PopupText.NewText(new()
                            {
                                Color = Color.Purple,
                                DurationInFrames = 120,
                                Text = "小心你的四周!",
                                Velocity = Vector2.UnitY * -5
                            }, NPC.Center);
                        }
                        else if(Timer1 >= 60)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                                int type = RandBullets();
                                for (int i = 0; i < 4; i++)
                                {
                                    for (float j = -5; j <= 5; j++)
                                    {
                                        Vector2 center = Target.Center + new Vector2(80 * j, -400 - Math.Abs(j * 30));
                                        Vector2 RealCenter = Target.Center + (center - Target.Center).RotatedBy(MathHelper.PiOver2 * i);
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), RealCenter, (Target.Center - RealCenter).RealSafeNormalize() * 2f, type, damage, 1.2f, Main.myPlayer);
                                    }
                                }
                            }
                            Timer1 = 0;
                            State++;
                        }
                        break;
                    }
                case 3://冲刺
                case 5://高速冲刺,靠近后散弹
                    {
                        Timer1++;
                        if(Timer1 < 60)
                        {
                            NPC.velocity *= 0.9f;
                            if(Timer1 == 5)
                            {
                                PopupText.NewText(new()
                                {
                                    Color = Color.Purple,
                                    DurationInFrames = 120,
                                    Text = "我要上了!",
                                    Velocity = Vector2.UnitY * -5
                                }, NPC.Center);
                            }
                        }
                        else if(Timer1 == 60)
                        {
                            NPC.velocity = toTarget.RealSafeNormalize() * 30;
                        }
                        else if(Timer1 > 90 && State == 3)
                        {
                            if (Timer1 > 120 || NPC.velocity.Length() < 2f)
                            {
                                State++;
                                Timer1 = 0;
                            }
                            else if(Timer1 % 3 == 0)NPC.velocity *= 0.9f;
                        }
                        else if(State == 5 && (Timer1 > 400 || Vector2.Distance(Target.Center,NPC.Center) < 300 || Vector2.Distance(Target.Center, NPC.Center) > 1200))
                        {
                            State++;
                            Timer1 = 0;
                            int type = RandBullets();
                            for (float i = -8; i <= 8; i++)
                            {
                                Vector2 vel = (i.ToRotationVector2() * MathHelper.Pi / 10) + NPC.velocity.RealSafeNormalize();
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vel * 10, type, damage, 1.2f, Main.myPlayer);
                            }
                            SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                        }
                        break;
                    }
                case 4://老大粗子弹!
                    {
                        NPC.velocity = toTarget * 0.015f;
                        Timer1++;
                        if(Timer1 == 30)
                        {
                            PopupText.NewText(new()
                            {
                                Color = Color.Purple,
                                DurationInFrames = 120,
                                Text = "这可是宝贝!",
                                Velocity = Vector2.UnitY * -5
                            }, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Timer2 = RandBullets();
                                for (int i = 0; i < 10; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center,NPC.velocity.RealSafeNormalize() * 5f,(int)Timer2, damage, 1.2f, Main.myPlayer);
                                }
                            }
                        }
                        else if(Timer1 == 90)
                        {
                            int count = 0;
                            foreach(Projectile projectile in Main.projectile)//遍历一次弹幕,让他们散射
                            {
                                if(projectile.active && projectile.type == (int)Timer2 && !projectile.friendly && count < 10)
                                {
                                    projectile.velocity = projectile.velocity.RotatedBy(MathHelper.Pi / 5 * count) * 3;
                                    count++;
                                }
                            }
                        }
                        else if(Timer1 == 120)
                        {
                            Timer1 = 0;
                            State++;
                        }
                        break;
                    }
                default:
                    {
                        Timer1 = Timer2 = 0;
                        State = 1;
                        break;
                    }
            }
        }
        #region 绘制部分
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            #region 旧位置储存
            for (int j = NPC.oldRot.Length - 1; j > 0; j--)
            {
                NPC.oldRot[j] = NPC.oldRot[j - 1];
            }
            NPC.oldRot[0] = NPC.rotation;
            #endregion
            #region 散弹
            if (State == 1)
            {
                float dis = (NPC.Center - Target.Center).Length();
                float color = (dis - 200) / dis;
                spriteBatch.Draw(ModContent.Request<Texture2D>("StarBreaker/NPCs/SanDan").Value,
                    NPC.Center - Main.screenPosition,
                    null,
                    new Color(color, 1f, 0f, 1f),
                    NPC.rotation + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi),
                    new Vector2(0, 48),
                    new Vector2(2, 1f),
                    SpriteEffects.None,
                    0f);
            }
            #endregion
            #region 拖尾
            for (int i = 0; i < NPC.oldPos.Length; i += 2)
            {
                Texture2D NPCTexture = TextureAssets.Npc[NPC.type].Value;
                int frameCount = Main.npcFrameCount[NPC.type];
                Vector2 DrawOrigin = new(NPCTexture.Width / 2, NPCTexture.Height / frameCount / 2);
                Vector2 DrawPosition = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
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
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
        #endregion
        private static int RandBullets()
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
