using StarBreaker.Projs.StarDoomStaff.Boss;
using System.IO;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace StarBreaker.NPCs.StarDoomStaff
{
    [AutoloadBossHead]
    public class StarDoomStaff : FSMNPC
    {
        private int TargerPlayerDeadCounts;//代表目标玩家死亡次数
        private bool TargetInDead;//表示目标存不存在死亡状态中
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Doom Staff");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "终末之星杖");
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override string BossHeadTexture => Texture;
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.lifeMax = 500000;
            NPC.knockBackResist = 0f;
            NPC.defense = 18;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 80;
            NPC.height = 80;
            NPC.damage = 80;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/DarkPurgatoryIntrusion");
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange( //添加信息
                new IBestiaryInfoElement[]//创建一个数组
                {
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,//说明本npc为星空
                    new FlavorTextBestiaryInfoElement("能带来终末的法杖,与她对抗不会有什么好下场")//信息
                }
                );
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapon.StarDoomStaff>()));
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(TargerPlayerDeadCounts);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            TargerPlayerDeadCounts = reader.ReadInt32();
        }
        public override void AI()
        {
            if (NPC.target == 255 || NPC.target <= 0)
            {
                NPC.TargetClosest();
            }
            int damage = NPC.damage;
            if (Main.expertMode)
            {
                damage /= 2;
            }
            else if (Main.masterMode)
            {
                damage /= 3;
            }

            if (Target.dead || !Target.active)
            {
                if (!TargetInDead)
                {
                    TargetInDead = true;
                    Target.respawnTimer = 1;
                    string NPC_Say_Text = "";
                    switch (TargerPlayerDeadCounts)
                    {
                        case 0:
                            {
                                NPC_Say_Text = "还不够,你死的太少了,起来重新打!";
                                break;
                            }
                        case 1:
                            {
                                NPC_Say_Text = "你死的次数,完全不够我取乐";
                                break;
                            }
                        case 2:
                            {
                                NPC_Say_Text = "你以为死第3次就可以摆脱我了?";
                                break;
                            }
                        case 3:
                            {
                                NPC_Say_Text = "已经死到了一个不幸的数字了哦?你最终绝对会倒下的";
                                break;
                            }
                        case 4:
                            {
                                NPC_Say_Text = "你死的次数,还没有超过被我肢解的人!";
                                break;
                            }
                        case 5:
                            {
                                NPC_Say_Text = "最终,你也将被我虐待至死!";
                                break;
                            }
                        case 49:
                            {
                                NPC_Say_Text = "50次...你真的是不靠别的方法打败他们的吗";
                                break;
                            }
                    }
                    CombatText.NewText(NPC.Hitbox, Color.Purple, NPC_Say_Text, true);
                    TargerPlayerDeadCounts++;
                    NPC.life += (int)(NPC.lifeMax * 0.05f);
                    if (TargerPlayerDeadCounts == 50)
                    {
                        NPC.active = false;
                    }

                    State = 1;
                    Timer1 = Timer2 = Timer3 = 0;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                TargetInDead = false;
                switch (State)
                {
                    case 0://开幕
                        {
                            Timer1++;
                            NPC.rotation = -MathHelper.PiOver4;
                            NPC.dontTakeDamage = true;
                            if (Timer1 % 50 == 0)
                            {
                                string NPC_Say_Text = "";
                                Color weaponSayColor = Color.Purple;
                                string weaponSayText = "";
                                switch ((int)Timer1 / 50)
                                {
                                    case 0:
                                        {
                                            NPC_Say_Text = "...又一个人";
                                            break;
                                        }
                                    case 1:
                                        {
                                            NPC_Say_Text = "不对...你身上为什么又有星辰之力在流动";
                                            break;
                                        }
                                    case 2:
                                        {
                                            NPC_Say_Text = "...是他们啊";
                                            weaponSayText = "为什么你真的要来打星杖啊";
                                            break;
                                        }
                                    case 3:
                                        {
                                            NPC_Say_Text = "你是...想来让我变成你的武器?";
                                            weaponSayText = "我**,从没有过这么大的压力";
                                            weaponSayColor = Color.LightBlue;
                                            break;
                                        }
                                    case 4:
                                        {
                                            NPC_Say_Text = "真的是笑死我了";
                                            weaponSayText = "我...霜拳救我QAQ";
                                            break;
                                        }
                                    case 5:
                                        {
                                            NPC_Say_Text = "知道吗";
                                            weaponSayText = "知道吗,我现在想揍死你" + Target.name;
                                            weaponSayColor = Color.LightBlue;
                                            break;
                                        }
                                    case 6:
                                        {
                                            NPC_Say_Text = "有数不胜数的人,想得到我";
                                            break;
                                        }
                                    case 7:
                                        {
                                            NPC_Say_Text = "我把他们虐待的很惨,很惨";
                                            weaponSayText = "终末就要来了...";
                                            weaponSayColor = Color.LightBlue;
                                            break;
                                        }
                                    case 8:
                                        {
                                            NPC_Say_Text = "这样吧";
                                            break;
                                        }
                                    case 9:
                                        {
                                            NPC_Say_Text = "我给你50次机会";
                                            break;
                                        }
                                    case 10:
                                        {
                                            NPC_Say_Text = "我允许你死亡50次,同时我会给自己回血";
                                            weaponSayText = "50次???";
                                            break;
                                        }
                                    case 11:
                                        {
                                            if (Target.HasItem(ModContent.ItemType<Items.Weapon.StarSpiralBlade>()))
                                            {
                                                NPC_Say_Text = "星辰旋刃...你也在啊";
                                                weaponSayText = "...我的死对头,终末之星杖啊,今天我会和你做出个了断的";
                                                weaponSayColor = Color.MediumPurple;
                                                _ = PopupText.NewText(new AdvancedPopupRequest()
                                                {
                                                    Text = "不是,她不就是打了你一下吗,怎么就是死对头了?",
                                                    DurationInFrames = 120,
                                                    Velocity = new Vector2(0, -4),
                                                    Color = Color.LightBlue
                                                }, Target.Center);
                                            }
                                            else
                                            {
                                                NPC_Say_Text = "他不在?我只能说你是真的疯狂";
                                                weaponSayText = "星辰旋刃吗...";
                                            }
                                            break;
                                        }
                                    case 12:
                                        {
                                            NPC_Say_Text = "废话少说,开始吧";
                                            NPC.dontTakeDamage = false;
                                            State++;
                                            Timer1 = 0;
                                            break;
                                        }
                                }
                                CombatText.clearAll();
                                _ = CombatText.NewText(NPC.Hitbox, Color.Purple, NPC_Say_Text, true);
                                _ = PopupText.NewText(new AdvancedPopupRequest()
                                {
                                    Text = weaponSayText,
                                    DurationInFrames = 120,
                                    Velocity = new Vector2(0, -4),
                                    Color = weaponSayColor
                                }, Target.Center);
                            }
                            break;
                        }
                    case 1://旋转冲刺
                        {
                            NPC.rotation += 2.3f;
                            if (Timer1 % 50 == 0)
                            {
                                NPC.velocity = (Target.position - NPC.position).SafeNormalize(default) * 30;
                            }
                            else if (Timer1 % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (Target.position - NPC.position).SafeNormalize(default) * 20,
                                    ModContent.ProjectileType<StarBoomCrystal_Boss>(), damage, 1.2f, Main.myPlayer);
                            }
                            Timer1++;
                            if (Timer1 > 200)
                            {
                                Timer1 = 0;
                                State++;
                            }
                            break;
                        }
                    case 2://鬼火传送
                        {
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.velocity = (NPC.velocity * 5 + (Target.position - NPC.position).SafeNormalize(default)) / 6;
                            Timer1++;
                            if (Timer1 > 60)
                            {
                                Timer1 = 0;
                                Vector2 pos = Target.position + (Target.position - NPC.position).SafeNormalize(default).RotatedBy(0.2) * 200;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < 50; i++)
                                    {
                                        Vector2 projPos = pos + (pos - NPC.position) / 50 * i;
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projPos, (pos - projPos) * 0.5f,
                                            ModContent.ProjectileType<StarBoomCrystal_Boss>(), damage, 1.2f, Main.myPlayer);
                                    }
                                }
                                NPC.position = pos;
                                State++;
                            }
                            break;
                        }
                    case 3://狙击
                        {
                            if (NPC.rotation < -MathHelper.PiOver4)
                            {
                                NPC.rotation += 0.05f;
                            }
                            else
                            {
                                NPC.rotation -= 0.05f;
                            }
                            NPC.velocity *= 0.99f;
                            Timer1++;
                            if (Timer1 > 10)
                            {
                                Timer1 = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), Target.position + new Vector2(Target.direction * 500, Target.velocity.Y),
                                        Vector2.UnitX * Target.direction * -10, ModContent.ProjectileType<StarBoomCrystal_Boss>(), damage, 1.2f, Main.myPlayer);
                                }
                                if (Timer2 >= 5)
                                {
                                    Timer2 = 0;
                                    State++;
                                    break;
                                }
                                Timer2++;
                            }
                            break;
                        }
                    case 4://测试激光
                        {
                            NPC.velocity *= 0.5f;
                            try
                            {
                                if (!Main.projectile[(int)Timer2].active || Main.projectile[(int)Timer2].type != ModContent.ProjectileType<EliminateRays_Boss>())
                                {
                                    Timer2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<EliminateRays_Boss>(),
                                        damage, 1.2f, Main.myPlayer);
                                }
                            }
                            catch { }
                            break;
                        }
                    default:
                        {
                            State = 1;
                            break;
                        }
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
