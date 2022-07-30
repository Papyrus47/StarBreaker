using Terraria.GameContent.Bestiary;

namespace StarBreaker.NPCs.NorBoss.SkyFracture
{
    [AutoloadBossHead]
    public class SkyFracture : FSMNPC
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.SkyFracture;//使用原版贴图
        public override string BossHeadTexture => Texture;//调用同Texture一样的算法
        public override void SetDefaults()
        {
            NPC.boss = true;//设置这个npc为boss
            NPC.lifeMax = 15000;//设置这个npc的血量上限
            NPC.damage = 54;//设置这个npc的伤害
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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)//这一个是图鉴信息
        {
            bestiaryEntry.Info.AddRange( //添加信息
                new IBestiaryInfoElement[]//创建一个数组
                {
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,//npc地形
                    new FlavorTextBestiaryInfoElement("一把光明之剑")//信息
                }
                );
        }
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active)
            {
                NPC.TargetClosest();
            }//获取玩家
            if (Target.dead)//玩家死亡ai
            {
                NPC.velocity.Y -= 0.3f;
                if (NPC.velocity.Y < -30)
                {
                    NPC.active = false;
                }

                return;
            }
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
            switch (State)
            {
                case 0://正常发射弹幕
                    {
                        NPC.velocity = (NPC.velocity * 8 + (Target.Center - NPC.Center).RealSafeNormalize() * 5) / 9;
                        Timer1++;
                        if (Timer1 > 4)
                        {
                            Timer1 = 0;
                            if (Timer2 > 15)
                            {
                                Timer2 = 0;
                                State++;
                                break;
                            }

                            Vector2 center = NPC.Center - (NPC.velocity.RotatedByRandom(0.2) * 50);
                            ShootProj(center, (Target.Center - center).RealSafeNormalize() * 20);
                            Timer2++;
                        }
                        break;
                    }
                case 1://大 风 车
                    {
                        NPC.velocity = (NPC.velocity * 3 + (Target.Center - NPC.Center).RealSafeNormalize()) / 4;
                        Timer1++;
                        if (Timer1 % 6 == 0)
                        {
                            if (Timer1 == 6)
                            {
                                Timer2 = Target.Center.X;
                                Timer3 = Target.Center.Y;
                            }
                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 pos = new Vector2(Timer2, Timer3);
                                Vector2 center = pos + MathHelper.ToRadians(Timer1 * 0.4f).ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i) * 600;
                                ShootProj(center, (pos - center).RealSafeNormalize() * 20);
                            }
                        }
                        if (Timer1 > 90)
                        {
                            Timer1 = Timer2 = Timer3 = 0;
                            State++;
                        }
                        break;
                    }
                case 2://给玩家休息
                    {
                        NPC.velocity = (NPC.velocity * 8 + (Target.Center - NPC.Center) * 0.07f) / 9;
                        Timer1++;
                        if (Timer1 > 120)
                        {
                            Timer1 = 0;
                            State++;
                        }
                        break;
                    }
                default:
                    {
                        State = 0;
                        break;
                    }
            }
        }
        private void ShootProj(Vector2 center, Vector2 vel)//私自定义的方法
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, vel, ProjectileID.SkyFracture, Damage, 1.4f, Main.myPlayer, Main.rand.Next(9));
                Main.projectile[proj].friendly = false;//修改友善与敌对
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].scale = 1.2f;
            }
        }
    }
}
