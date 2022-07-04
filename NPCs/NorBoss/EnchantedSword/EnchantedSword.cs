namespace StarBreaker.NPCs.NorBoss.EnchantedSword
{
    [AutoloadBossHead]
    //附魔剑,一个走地机boss
    //我会详细的介绍怎么写一个走地机boss
    //等到我有另外npc的帧图再搞一个带帧图的走地机boss
    public class EnchantedSword : FSMNPC
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.EnchantedSword;
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "附魔剑");
            NPCID.Sets.TrailCacheLength[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.Size = new(34);//体积
            NPC.noTileCollide = false;//这是设置碰墙
            NPC.noGravity = false;//受重力影响
            NPC.aiStyle = -1;
            NPC.damage = 47;
            NPC.knockBackResist = 0f;
            NPC.friendly = false;
            NPC.boss = true;
            NPC.behindTiles = true;//这一个是绘制用的,Draw在物块后面
            NPC.value = 30424;
            NPC.lifeMax = 12000;
            NPC.defense = 5;
            NPC.scale = 1.8f;

            if (!Main.dedServ)//服务器又不听音乐
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/UnderfootEnterenceBoss");//这个是曲子
                SceneEffectPriority = SceneEffectPriority.BossMedium;//曲子优先度
            }
        }
        public override void AI()
        {
            if(NPC.target < 0 || NPC.target == 255 || !Target.active || Target.dead)
            {
                NPC.TargetClosest();
            }//获取目标,不用说了

            if (Target.dead)//死亡
            {
                NPC.velocity.X += NPC.velocity.X;//逃跑
                if (NPC.velocity.X < 0.1f) NPC.velocity.X = 0.1f;
                if (Math.Abs(NPC.velocity.X) > 10) NPC.active = false;
                return;
            }
            switch (State)
            {
                case 0://落地装逼
                    {
                        NPC.velocity.Y = 3;//改变Y速度
                        NPC.rotation = MathHelper.PiOver2 + MathHelper.PiOver4;
                        if (NPC.collideY)//这个东西是检测npc有没有碰到Y轴
                        {
                            NPC.velocity.Y = 0;
                            State++;
                        }
                        break;
                    }
                case 1://插地,蓄力,飞行到玩家头顶后落下
                    {
                        if (Timer1 == 0)//重置一下插地状态
                        {
                            NPC.rotation = MathHelper.PiOver2 + MathHelper.PiOver4;
                        }
                        Timer1++;//增加计时器

                        if (Timer1 == 60)//跳跃
                        {
                            NPC.velocity.Y = -15;
                            NPC.velocity.X = (Target.Center - NPC.Center).X / 60f;
                        }
                        else if (Timer1 > 120 || NPC.velocity.Y > 3)//超过最大飞行时间,或者开始下坠
                        {
                            NPC.velocity.Y = 8;//向下冲刺
                            NPC.velocity.X = 0;
                            NPC.rotation = MathHelper.PiOver2 + MathHelper.PiOver4;
                            if (NPC.collideY || Timer1 > 180)//碰到Y轴时候,或者时间太长,切换状态
                            {
                                Timer1 = 0;
                                State++;
                            }
                        }
                        else
                        {
                            if (Timer1 % 5 == 0 && Timer1 < 60)//生成装饰用粒子
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Gold);
                                }
                            }
                            if (!NPC.collideY && NPC.velocity.Y != 0f)//当npc脱离地面的时候
                            {
                                NPC.rotation += 0.3f;
                            }
                        }
                        break;
                    }
                case 2://蓄力冲刺,碰到物块或者时间长起飞
                    {
                        Timer1++;//增加计时器
                        if (Timer1 == 30)//冲刺
                        {
                            NPC.rotation = MathHelper.PiOver2 + MathHelper.PiOver4;
                            NPC.velocity.X = (Target.Center - NPC.Center).X > 0 ? 15 : -15;
                        }
                        if (Timer1 > 60 || NPC.collideX)//时间长或者碰到X轴碰到物块
                        {
                            Timer1 = 0;
                            State++;
                            NPC.velocity.Y = -19;//欸,起飞
                        }
                        else
                        {
                            if (Timer1 % 5 == 0 && Timer1 < 60)//生成装饰用粒子
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Gold);
                                }
                            }
                        }
                        break;
                    }
                case 3://旋转冲刺
                    {
                        NPC.rotation += 0.2f;
                        if (NPC.velocity.Y > 0 && Timer1 == 0)//NPC开始下落那一刻
                        {
                            Timer1++;
                            NPC.velocity = (Target.Center - NPC.Center).RealSafeNormalize() * 20f;
                        }
                        if (NPC.collideY)//碰到Y轴
                        {
                            Timer1 = 0;
                            State++;
                            NPC.velocity.X *= 0.95f;
                            NPC.velocity.Y -= 5;

                            if (Main.netMode != NetmodeID.MultiplayerClient)//虽然不联机同步但我还是写了的屑
                            {
                                for (int i = 0; i < 10; i++)//散发弹幕
                                {
                                    Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.Pi / 5 * i) * 10f;
                                    Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center,
                                        vel, ProjectileID.EnchantedBeam, NPC.damage, 1.4f, Main.myPlayer);
                                    projectile.hostile = true;
                                    projectile.friendly = false;
                                    projectile.tileCollide = false;
                                }
                            }
                        }
                        break;
                    }
                case 4://连续跳跃10次
                    {
                        if (Timer1 < 10)//小于10次时
                        {
                            Timer2++;
                            NPC.rotation += 0.35f;

                            if (NPC.collideY || Timer2 > 120)//碰到Y轴,或者大于120秒(卡住)
                            {
                                NPC.velocity.Y -= 8;//起跳

                                if (Math.Abs(Target.Center.X - NPC.Center.X) > 500)//X距离过远
                                {
                                    NPC.velocity.X = Target.Center.X - NPC.Center.X > 0 ? 8 : -8;
                                }
                                Timer1++;
                                Timer2 = 0;
                            }
                            if (NPC.collideX)//碰到X轴
                            {
                                NPC.velocity.X = -NPC.velocity.X;//方向相反
                                if (Math.Abs(NPC.velocity.X) < 8)//如果速度太小
                                {
                                    NPC.velocity.X = NPC.velocity.X > 0 ? 8 : -8;
                                }
                            }
                        }
                        else
                        {
                            NPC.rotation = MathHelper.PiOver2 + MathHelper.PiOver4;
                            Timer1 = 0;
                            Timer2 = 0;
                            State++;
                        }
                        break;
                    }
                case 5://芜湖,五角星
                    {
                        NPC.noTileCollide = true;//可以穿墙
                        NPC.noGravity = true;//可以飞行
                        float i = 0;
                        switch (Timer1)
                        {
                            case 1:
                                i = 3;
                                break;
                            case 2:
                                i = 1;
                                break;
                            case 3:
                                i = 4;
                                break;
                            case 4:
                                i = 2;
                                break;
                        }
                        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                        Vector2 center = Target.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 5 * i) * 300;//获取五角星位置
                        NPC.velocity = (center - NPC.Center).RealSafeNormalize() * 30f;
                        if (Vector2.Distance(NPC.Center, center) < 40)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int j = 1; j < 3; j++)
                                {
                                    Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center,
                                        NPC.velocity * -0.3f * (j / 3f), ProjectileID.EnchantedBeam, NPC.damage, 1.4f, Main.myPlayer);
                                    projectile.hostile = true;
                                    projectile.friendly = false;
                                    projectile.tileCollide = false;
                                }
                            }
                            Timer1++;
                            if (Timer1 > 6)
                            {
                                Timer1 = 0;
                                NPC.noTileCollide = false;
                                NPC.noGravity = false;
                                State++;
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
        public override void BossHeadRotation(ref float rotation) => rotation = NPC.rotation;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition - new Vector2(0, -16 * NPC.scale), null, drawColor, NPC.rotation, texture.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
