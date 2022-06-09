using StarBreaker.Items.Weapon.HradMode;
using Terraria.GameContent.ItemDropRules;

namespace StarBreaker.NPCs.NorBoss.OnyxBlaster
{
    [AutoloadBossHead]//自动加载boss头像
    public class OnyxBlaster : FSMNPC //作为开源mod的一个屑教程boss
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.OnyxBlaster;
        public override string BossHeadTexture => Texture;//调用同Texture一样的算法
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Blaster");//设置名字
            DisplayName.AddTranslation(7, "玛瑙爆破枪");//设置中文下的名字
        }
        public override void SetDefaults()
        {
            NPC.boss = true;//设置这个npc为boss
            NPC.lifeMax = 15000;//设置这个npc的血量上限
            NPC.damage = 23;//设置这个npc的伤害
            NPC.knockBackResist = 0f;//npc的击退抗性
            NPC.noGravity = true;//不受重力影响
            NPC.noTileCollide = true;//不会碰方块
            NPC.width = 60;//宽
            NPC.height = 26;//高
            NPC.defense = 9;//防御
            NPC.aiStyle = -1;//避免受到0的下坠影响
            NPC.HitSound = SoundID.NPCHit4;//受伤音效
            NPC.scale = 1.8f;//缩放变大
            NPC.npcSlots = 20;//避免刷出其他npc

            if (!Main.dedServ)//服务器又不听音乐
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/UnderfootEnterenceBoss");//这个是曲子
                SceneEffectPriority = SceneEffectPriority.BossMedium;//曲子优先度
            }
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;//boss头像旋转
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)//boss头像翻转
        {
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.None;
            }
            else
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)//这一个是图鉴信息
        {
            bestiaryEntry.Info.AddRange( //添加信息
                new IBestiaryInfoElement[]//创建一个数组
                {
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptDesert,//npc地形
                    new FlavorTextBestiaryInfoElement("一把黑暗之枪")//信息
                }
                );
        }
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active)
            {
                NPC.TargetClosest();
            }//获取玩家

            NPC.rotation = NPC.velocity.ToRotation();//旋转

            if (NPC.velocity.X < 0)
            {
                NPC.spriteDirection = 1;
            }
            else
            {
                NPC.spriteDirection = -1;
            }
            NPC.direction = NPC.spriteDirection;//改变朝向

            if (NPC.direction == 1)
            {
                NPC.rotation += MathHelper.Pi;
            }//修正朝向带来的问题
            if (Target.dead)//玩家死亡ai
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y > 30)
                {
                    NPC.active = false;
                }

                return;
            }
            switch (State)
            {
                case 1://瞄准玩家,发射散弹
                    {
                        if (Timer1 < 0)
                        {
                            Timer1 = 36;//倒计时
                            if (Timer2 > 3)//发射次数大于3次
                            {
                                Timer1 = Timer2 = 0;
                                State++;
                                break;
                            }
                            if (Timer2 > 0)
                            {
                                for (int i = 0; i < 8; i++)//这里是发射散弹
                                {
                                    ShootOnyx(NPC.Center, NPC.velocity.RotatedByRandom(0.3).RealSafeNormalize() * Main.rand.NextFloat(8, 10));
                                    SoundEngine.PlaySound(SoundID.Item36, NPC.Center);
                                }
                            }
                            Timer2++;//计数
                        }
                        else
                        {
                            Timer1--;
                            NPC.velocity = (NPC.velocity * 5 + (Target.Center - NPC.Center).RealSafeNormalize() * 5) / 6;
                        }
                        break;
                    }
                case 2://从天而降玛瑙
                    {
                        NPC.velocity = (Target.Center - NPC.Center).RealSafeNormalize();
                        Timer1++;
                        if (Timer1 > 40)
                        {
                            Timer1 = 0;
                            State++;
                        }
                        else
                        {
                            Vector2 center = Target.Center + new Vector2(Main.rand.NextFloatDirection() * 300, -480);
                            ShootOnyx(center, (Target.Center - center).RealSafeNormalize() * 9);
                        }
                        break;
                    }
                case 3://冲刺,路径留下对玩家的弹幕
                    {
                        Timer1++;
                        if (Timer1 < 60)//对准玩家
                        {
                            NPC.velocity = (NPC.velocity * 5 + (Target.Center - NPC.Center).RealSafeNormalize()) / 6;//渐变速度,好看一些
                        }
                        else if (Timer1 == 60)//开始冲刺
                        {
                            NPC.velocity = (Target.Center - NPC.Center).RealSafeNormalize() * 25;
                        }
                        else if (Timer1 > 90)//减速
                        {
                            NPC.velocity *= 0.98f;//减速
                            if (Timer1 > 120)
                            {
                                Timer1 = 0;
                                State++;
                            }
                            else if (Timer1 % 15 == 0)//每有15帧
                            {
                                ShootOnyx(NPC.Center, (Target.Center - NPC.Center).RealSafeNormalize() * 8);
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
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OnyxBlasterGun>()));
        }
        public override void OnKill()
        {
            StarBreakerSystem.downedOnyxBlaster = true;
        }
        private void ShootOnyx(Vector2 center, Vector2 vel)//私自定义的方法
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, vel, 661, Damage, 1.4f, Main.myPlayer);
                Main.projectile[proj].friendly = false;//修改友善与敌对
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].scale = 1.7f;
            }
        }
    }
}
