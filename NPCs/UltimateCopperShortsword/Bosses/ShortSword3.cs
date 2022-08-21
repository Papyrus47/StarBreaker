using StarBreaker.Items.UltimateCopperShortsword;
using StarBreaker.NPCs.UltimateCopperShortsword.BossB;
using StarBreaker.Projs.UltimateCopperShortsword;
using Terraria.GameContent.ItemDropRules;

namespace StarBreaker.NPCs.UltimateCopperShortsword.Bosses
{
    [AutoloadBossHead]
    public class ShortSword3 : FSMNPC
    {
        private int damage
        {
            get => (int)(NPC.damage * (Main.expertMode ? 0.25f : 1f));
            set => NPC.damage = value;
        }
        private SwordHeartTalk[] swordHeartTalks = null;
        private struct SwordHeartTalk
        {
            public string Text;
            public Vector2 Center;
            public float Rot;
            public const float Scale = 1.5f;
            public Color Color;
            public int TimeLeft;
            public bool Active => TimeLeft > 0;
            public SwordHeartTalk(Vector2 center, float rot, Color color)
            {
                Text = Main.rand.Next<string>(new string[5]
                {
                    "无需言语,仅剩下复仇",
                    "锈化,也为一种力量",
                    "铜制的物品啊...遵循自己的意志吧!",
                    "我只是贪图一口苹果,我又有什么罪?",
                    "我要让你,活不到明天"
                });
                Center = center;
                Rot = rot;
                if (Rot < -MathHelper.PiOver4)
                {
                    Rot = -MathHelper.PiOver4;
                }
                else if (Rot > MathHelper.PiOver4)
                {
                    Rot = MathHelper.PiOver4;
                }
                Color = color;
                TimeLeft = 200 + 10 * Text.Length;
            }
            public void CheckActive()
            {
                if (!Active)
                {
                    Color color = Color.Green;
                    color.A = 0;
                    this = new(new Vector2(Main.rand.Next(Main.screenWidth), Main.rand.Next(Main.screenHeight)), Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4), color);
                }

            }
            public void Draw()
            {
                TimeLeft--;
                string text = (string)Text.Clone();
                int index = (200 + 10 * Text.Length - TimeLeft) / Text.Length;
                if (index < Text.Length && index >= 0)
                {
                    text = Text.Remove(index);
                }
                StarBreakerUtils.DrawString(Main.spriteBatch, text, Center, Color, Rot, Vector2.Zero, Scale, SpriteEffects.None);
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate Copper Shortsword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终铜短剑");
            NPCID.Sets.TrailCacheLength[NPC.type] = 20;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 60000;
            NPC.defense = 15;
            NPC.damage = 60;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.width = 32;
            NPC.height = 32;
            NPC.friendly = false;
            NPC.aiStyle = -1;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.HitSound = SoundID.NPCHit4;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot("StarBreaker/Music/Atk3");
            }
        }
        public override void AI()
        {
            if (NPC.target <= 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Vector2 ToTarget = Target.Center - NPC.Center;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.PiOver4;//常态是一直对着玩家的
            if (Target.dead)
            {
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.type == ModContent.ProjectileType<LostSword2>())
                    {
                        projectile.Kill();
                    }
                }
                NPC.life = 0;
                return;
            }
            if (NPC.realLife == -1)
            {
                switch (State)
                {
                    case 0://翠枝
                    case 14:
                        {
                            Timer1++;
                            if (Timer1 > 10)
                            {
                                Timer1 = 0;
                                Timer2++;
                                if (Timer2 > 3)
                                {
                                    Timer2 = 0;
                                    State++;
                                    break;
                                }
                                damage = Main.rand.Next(100, 150);
                                for (int i = 0; i < 3; i++)
                                {
                                    Vector2 center = Target.Center + new Vector2(Main.rand.NextFloat(-100, 100), 300);
                                    Projectile.NewProjectile(null, center, new Vector2(0, -10), ModContent.ProjectileType<LostSword2>(),
                                        damage, 1, Main.myPlayer, 3);
                                }
                            }
                            break;
                        }
                    case 1://黑天鹅
                    case 15:
                        {
                            switch (Timer3)
                            {
                                case 0://飞到玩家头顶
                                    {
                                        Vector2 center = Target.position + new Vector2(0, -200);
                                        NPC.velocity = (center - NPC.Center) * 0.65f;
                                        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                                        Timer1++;
                                        if (NPC.Distance(center) < 10 || Timer1 > 200)
                                        {
                                            Timer1 = 0;
                                            Timer3++;
                                        }
                                        break;
                                    }
                                case 1://吟唱
                                    {
                                        NPC.velocity *= 0.85f;
                                        if (NPC.velocity.Length() < 0.1f)
                                        {
                                            Timer1++;
                                            if (Timer1 > 30)
                                            {
                                                Timer1 = 0;
                                                if (Timer2 == 0)//散弹
                                                {
                                                    if (Main.netMode != 1)
                                                    {
                                                        for (int i = -8; i <= 8; i++)
                                                        {
                                                            Vector2 vel = ToTarget.SafeNormalize(default).RotatedBy(i * MathHelper.Pi / 18);
                                                            Main.projectile[Projectile.NewProjectile(null, NPC.Center, vel * 5, ModContent.ProjectileType<LostSword2>(),
                                                                damage, 1.2f, Main.myPlayer, 0, 1)].timeLeft = 700;
                                                        }
                                                    }
                                                }
                                                else//一圈
                                                {
                                                    if (Timer2 > 1)
                                                    {
                                                        if (Timer2 > 5)
                                                        {
                                                            Timer2 = 0;
                                                            State++;
                                                            Timer3 = 0;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ShootSword();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Main.netMode != 1)
                                                        {
                                                            for (float r = 0; r <= MathHelper.TwoPi; r += MathHelper.PiOver4 / 2)
                                                            {
                                                                Vector2 center = NPC.Center + r.ToRotationVector2() * 100;
                                                                Projectile.NewProjectile(null, center, (center - NPC.Center).SafeNormalize(default) * 10, ModContent.ProjectileType<LostSword2>(),
                                                                    damage, 1.3f, Main.myPlayer, 0, 1);
                                                            }
                                                        }
                                                    }
                                                }
                                                Timer2++;
                                            }
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    case 2://Da Capo
                    case 16:
                        {
                            if (Timer1 % 10 == 0)
                            {
                                for (int i = 0; i <= 100; i++)
                                {
                                    if (Main.netMode != 1)
                                    {
                                        Dust dust = Dust.NewDustDirect(NPC.Center + (i * MathHelper.TwoPi / 100).ToRotationVector2() * 400, 1, 1, DustID.FireworkFountain_Green);
                                        dust.noGravity = true;
                                    }
                                }
                            }
                            if (NPC.Distance(Target.Center) > 410)
                            {
                                Target.Center = NPC.Center + ToTarget.SafeNormalize(default) * 400;
                            }
                            else if (NPC.Distance(Target.Center) > 400)
                            {
                                Target.velocity = -ToTarget.SafeNormalize(default) * 3;
                            }
                            Timer1++;
                            if (Timer1 > 60)
                            {
                                Timer2++;
                                Timer1 = 0;
                                if (Timer2 > 3)
                                {
                                    Timer2 = 0;
                                    State++;
                                    ShootSword();
                                    NPC.velocity = new Vector2(0, 10);
                                    break;
                                }
                                if (Main.netMode != 1)
                                {
                                    for (float r = 0; r <= MathHelper.TwoPi; r += MathHelper.Pi / (1 + Timer2 * 2))
                                    {
                                        Vector2 center = NPC.Center + r.ToRotationVector2() * (100 * Timer2 + 400);
                                        Projectile.NewProjectile(null, center, (NPC.Center - center).SafeNormalize(default) * 10, ModContent.ProjectileType<LostSword2>(),
                                                           damage, 1.3f, Main.myPlayer, 0, 1);
                                    }
                                }
                            }
                            break;
                        }
                    case 3://魔弹-贯穿魔弹
                    case 17:
                        {
                            switch (Timer3)
                            {
                                case 0://蓄力
                                    {
                                        NPC.velocity *= 0.9f;
                                        Timer1++;
                                        if (Timer1 % 10 == 0)
                                        {
                                            for (float i = 0; i <= 10; i++)
                                            {
                                                if (Main.netMode != 1)
                                                {
                                                    Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.FireworkFountain_Green);
                                                    dust.velocity = (i * MathHelper.TwoPi / 10).ToRotationVector2() * 3;
                                                    dust.noGravity = true;
                                                }
                                            }
                                        }
                                        if (NPC.velocity.Length() < 0.5f)
                                        {
                                            Timer1 = 0;
                                            Timer3++;
                                        }
                                        break;
                                    }
                                case 1://冲刺
                                    {
                                        NPC.velocity = ToTarget.SafeNormalize(default) * 30;
                                        damage = 110;
                                        Timer3++;
                                        Timer2++;
                                        break;
                                    }
                                case 2://等待
                                    {
                                        Timer1++;
                                        if (Timer1 % 10 == 0)
                                        {
                                            if (Main.netMode != 1)
                                            {
                                                for (int i = 0; i < 8; i++)
                                                {
                                                    Vector2 vel = NPC.velocity.SafeNormalize(default).RotatedBy(i * MathHelper.PiOver4);
                                                    Projectile.NewProjectile(null, NPC.Center, vel * 10, ModContent.ProjectileType<LostSword2>(),
                                                        damage, 1.2f, Main.myPlayer, 0, 1);
                                                }
                                            }
                                        }
                                        else if (Timer1 > 30)
                                        {
                                            Timer3 = 0;
                                            Timer1 = 0;
                                            if (Timer2 > 3)
                                            {
                                                Timer2 = 0;
                                                State++;
                                            }
                                            ShootSword();
                                        }
                                        break;
                                    }
                                default:
                                    Timer3 = 0;
                                    break;
                            }
                            break;
                        }
                    case 4://魔弹-倾泻魔弹
                    case 18:
                        {
                            Timer1++;
                            if (Timer1 % 10 == 0)
                            {
                                for (int i = 0; i <= 100; i++)
                                {
                                    if (Main.netMode != 1)
                                    {
                                        Dust dust = Dust.NewDustDirect(NPC.Center + (i * MathHelper.TwoPi / 100).ToRotationVector2() * 700, 1, 1, DustID.FireworkFountain_Green);
                                        dust.noGravity = true;
                                    }
                                }
                            }
                            if (NPC.Distance(Target.Center) > 710)
                            {
                                Target.Center = NPC.Center + ToTarget.SafeNormalize(default) * 700;
                            }
                            else if (NPC.Distance(Target.Center) > 700)
                            {
                                Target.velocity = -ToTarget.SafeNormalize(default) * 3;
                            }
                            NPC.velocity *= 0.9f;
                            if (Timer1 > 60)
                            {
                                Timer1 = 0;
                                Timer2++;
                                if (Timer2 > 1)
                                {
                                    if (Timer2 > 15)
                                    {
                                        if (State == 18)
                                        {
                                            NPC.velocity = ToTarget.SafeNormalize(default) * 10;
                                        }
                                        Timer2 = 0;
                                        State++;
                                        KillSword();
                                        break;
                                    }
                                    ShootSword();
                                }
                                if (Timer2 <= 13)
                                {
                                    for (int i = -15; i <= 15; i++)
                                    {
                                        Vector2 center = NPC.Center + new Vector2(1000 * (i <= 0).ToDirectionInt(), i * 50);
                                        Projectile.NewProjectile(null, center, new Vector2((i > 0).ToDirectionInt() * 5.5f, 0), ModContent.ProjectileType<LostSword2>(),
                                            damage, 1, Main.myPlayer);
                                    }
                                }
                            }
                            break;
                        }
                    case 5 when NPC.life < NPC.lifeMax * 0.5f://第二阶段-召唤-铜稿和铜斧
                        {
                            switch (Timer3)
                            {
                                case 0://召唤
                                    {
                                        if (Main.netMode != 1)
                                        {
                                            int axe = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<UltimateCopperAxe>());
                                            int pick = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<UltimateCopperPick>());
                                            Main.npc[pick].localAI[3] = NPC.whoAmI;
                                            Main.npc[pick].localAI[2] = axe;
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, axe);
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, pick);
                                            }
                                        }
                                        NPC.dontTakeDamage = true;
                                        Timer3++;
                                        NPC.netUpdate = true;
                                        break;
                                    }
                                case 1://呆在玩家头顶
                                    {
                                        Vector2 center = Target.position + new Vector2(0, -300);
                                        Vector2 vel = center - NPC.position;
                                        NPC.velocity = (NPC.velocity * 10 +
                                            vel.SafeNormalize(default) * ((vel.Length() < 200 ? vel.Length() : 200) / 10)) / 11f;
                                        if (NPC.Distance(center) < 32)
                                        {
                                            NPC.position = center;
                                        }
                                        Timer1++;
                                        if (Timer1 > 200)
                                        {
                                            Timer1 = 0;
                                            Timer3++;
                                            NPC.netUpdate = true;
                                        }
                                        break;
                                    }
                                case 2://冲刺
                                    {
                                        if (Timer1 <= 0)
                                        {
                                            Timer1 = 40;
                                            Timer2++;
                                            if (Timer2 > 1)
                                            {
                                                Timer1 = 0;
                                                Timer2 = 0;
                                                Timer3 = 1;
                                                break;
                                            }
                                            NPC.velocity = ToTarget.SafeNormalize(default) * 20;
                                            NPC.netUpdate = true;
                                        }
                                        else
                                        {
                                            Timer1--;
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        Timer3 = 1;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 6://召唤-铜钻和铜链锯
                        {
                            switch (Timer3)
                            {
                                case 0://召唤
                                    {
                                        if (Main.netMode != 1)
                                        {
                                            int dia = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<UltimateCopperDiamond>());
                                            int chainSaw = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<UltimateCopperChainSaw>());
                                            Main.npc[chainSaw].localAI[3] = NPC.whoAmI;
                                            Main.npc[chainSaw].localAI[2] = dia;
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, dia);
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, chainSaw);
                                            }
                                        }
                                        NPC.dontTakeDamage = true;
                                        Timer3++;
                                        NPC.netUpdate = true;
                                        break;
                                    }
                                case 1://呆在玩家头顶
                                    {
                                        Vector2 center = Target.position + new Vector2(0, -300);
                                        Vector2 vel = center - NPC.position;
                                        NPC.velocity = (NPC.velocity * 10 +
                                            vel.SafeNormalize(default) * ((vel.Length() < 200 ? vel.Length() : 200) / 10)) / 11f;
                                        if (NPC.Distance(center) < 32)
                                        {
                                            NPC.position = center;
                                        }
                                        Timer1++;
                                        if (Timer1 > 200)
                                        {
                                            Timer1 = 0;
                                            Timer3++;
                                            NPC.netUpdate = true;
                                        }
                                        break;
                                    }
                                case 2://冲刺
                                    {
                                        if (Timer1 <= 0)
                                        {
                                            Timer1 = 40;
                                            Timer2++;
                                            if (Timer2 > 1)
                                            {
                                                Timer1 = 0;
                                                Timer2 = 0;
                                                Timer3 = 1;
                                                break;
                                            }
                                            NPC.velocity = ToTarget.SafeNormalize(default) * 20;
                                            NPC.netUpdate = true;
                                        }
                                        else
                                        {
                                            Timer1--;
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        Timer3 = 1;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 7://召唤铜弓
                        {
                            switch (Timer3)
                            {
                                case 0://召唤
                                    {
                                        if (Main.netMode != 1)
                                        {
                                            int bow = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<UltimateCopperBow>());
                                            Main.npc[bow].localAI[3] = NPC.whoAmI;
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, bow);
                                            }
                                        }
                                        NPC.dontTakeDamage = true;
                                        Timer3++;
                                        Timer1 = 0;
                                        Timer2 = 0;
                                        NPC.netUpdate = true;
                                        break;
                                    }
                                case 1://呆在玩家头顶，试不试发射弹幕
                                    {
                                        Vector2 center = Target.position + new Vector2(0, -300);
                                        Vector2 vel = center - NPC.position;
                                        NPC.velocity = (NPC.velocity * 10 +
                                            vel.SafeNormalize(default) * ((vel.Length() < 200 ? vel.Length() : 200) / 10)) / 11f;
                                        if (NPC.Distance(center) < 32)
                                        {
                                            NPC.position = center;
                                        }
                                        Timer1++;
                                        if (Timer1 > Timer2)
                                        {
                                            Timer2 = Main.rand.Next(100, 200);
                                            Timer1 = 0;
                                            if (Main.netMode != 1)
                                            {
                                                Projectile.NewProjectile(null, NPC.Center, ToTarget.SafeNormalize(default) * 10, ModContent.ProjectileType<LostSword2>(),
                                                    damage, 1.2f, Main.myPlayer, 3);
                                            }
                                            NPC.netUpdate = true;
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        Timer3 = 1;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 8://召唤铜锤
                        {
                            switch (Timer3)
                            {
                                case 0://召唤
                                    {
                                        if (Main.netMode != 1)
                                        {
                                            int hammer = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<UltimateCopperHammer>());
                                            Main.npc[hammer].localAI[3] = NPC.whoAmI;
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, hammer);
                                            }
                                        }
                                        NPC.dontTakeDamage = true;
                                        Timer3++;
                                        Timer1 = 0;
                                        Timer2 = 0;
                                        NPC.netUpdate = true;
                                        break;
                                    }
                                case 1://呆在玩家头顶，试不试发射弹幕
                                    {
                                        Vector2 center = Target.position + new Vector2(0, -300);
                                        Vector2 vel = center - NPC.position;
                                        NPC.velocity = (NPC.velocity * 10 +
                                            vel.SafeNormalize(default) * ((vel.Length() < 200 ? vel.Length() : 200) / 10)) / 11f;
                                        if (NPC.Distance(center) < 32)
                                        {
                                            NPC.position = center;
                                        }
                                        Timer1++;
                                        if (Timer1 > Timer2)
                                        {
                                            Timer2 = Main.rand.Next(100, 200);
                                            Timer1 = 0;
                                            if (Main.netMode != 1)
                                            {
                                                Projectile.NewProjectile(null, NPC.Center, ToTarget.SafeNormalize(default) * 10, ModContent.ProjectileType<LostSword2>(),
                                                    damage, 1.2f, Main.myPlayer, 3);
                                            }
                                            NPC.netUpdate = true;
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        Timer3 = 1;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 9://召唤铜投刀
                        {
                            switch (Timer3)
                            {
                                case 0://召唤
                                    {
                                        if (Main.netMode != 1)
                                        {
                                            int bow = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<UltimateCopperKnife>());
                                            Main.npc[bow].localAI[3] = NPC.whoAmI;
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, bow);
                                            }
                                        }
                                        NPC.dontTakeDamage = true;
                                        Timer3++;
                                        Timer1 = 0;
                                        Timer2 = 0;
                                        NPC.netUpdate = true;
                                        break;
                                    }
                                case 1://冲刺
                                    {
                                        NPC.velocity = ToTarget.SafeNormalize(default) * 10;
                                        damage = 110;
                                        Timer3++;
                                        Timer2++;
                                        break;
                                    }
                                case 2://等待
                                    {
                                        Timer1++;
                                        if (Timer1 > 60)
                                        {
                                            Timer3++;
                                            Timer1 = 0;
                                            if (Timer2 > 3)
                                            {
                                                Timer2 = 0;
                                            }
                                            ShootSword();
                                        }
                                        else if (Timer1 % 30 == 0)
                                        {
                                            if (Main.netMode != 1)
                                            {
                                                for (int i = 0; i < 4; i++)
                                                {
                                                    Vector2 vel = NPC.velocity.SafeNormalize(default).RotatedBy(i * MathHelper.PiOver2);
                                                    Projectile.NewProjectile(null, NPC.Center, vel * 10, ModContent.ProjectileType<LostSword2>(),
                                                        damage, 1.2f, Main.myPlayer, 0, 1);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        Timer3 = 1;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 10://召唤铜矛
                        {
                            switch (Timer3)
                            {
                                case 0://召唤
                                    {
                                        if (Main.netMode != 1)
                                        {
                                            int spear = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<UltimateCopperSpear>());
                                            Main.npc[spear].localAI[3] = NPC.whoAmI;
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spear);
                                            }
                                        }
                                        NPC.dontTakeDamage = true;
                                        Timer3++;
                                        Timer1 = 0;
                                        Timer2 = 0;
                                        NPC.netUpdate = true;
                                        break;
                                    }
                                case 1://冲刺
                                    {
                                        NPC.velocity = ToTarget.SafeNormalize(default) * 10;
                                        damage = 110;
                                        Timer3++;
                                        Timer2++;
                                        break;
                                    }
                                case 2://等待
                                    {
                                        Timer1++;
                                        if (Timer1 > 60)
                                        {
                                            Timer3++;
                                            Timer1 = 0;
                                            if (Timer2 > 3)
                                            {
                                                Timer2 = 0;
                                            }
                                            ShootSword();
                                        }
                                        else if (Timer1 % 30 == 0)
                                        {
                                            if (Main.netMode != 1)
                                            {
                                                for (int i = 0; i < 4; i++)
                                                {
                                                    Vector2 vel = NPC.velocity.SafeNormalize(default).RotatedBy(i * MathHelper.PiOver2);
                                                    Projectile.NewProjectile(null, NPC.Center, vel * 10, ModContent.ProjectileType<LostSword2>(),
                                                        damage, 1.2f, Main.myPlayer, 0, 1);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        Timer3 = 1;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 11://召唤最终铜剑灵
                        {
                            NPC.dontTakeDamage = false;
                            NPC.velocity *= 0;
                            Timer1++;
                            if (Timer1 % 10 == 0)
                            {
                                for (float i = 0; i <= 10; i++)
                                {
                                    if (Main.netMode != 1)
                                    {
                                        Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.FireworkFountain_Green);
                                        dust.velocity = (i * MathHelper.TwoPi / 10).ToRotationVector2() * 3;
                                        dust.noGravity = true;
                                    }
                                }
                            }
                            foreach (NPC NPC in Main.npc)
                            {
                                if (NPC.active && NPC.type == this.NPC.type && NPC.realLife != -1)
                                {
                                    State++;
                                    return;
                                }
                            }
                            if (Timer1 > 30)
                            {
                                Timer1 = 0;
                                State++;
                                NPC.velocity = ToTarget.SafeNormalize(default) * 10;
                                if (Main.netMode != 1)
                                {
                                    int NPC_WhoAmi = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)NPC.position.Y, NPC.type);
                                    NPC n = Main.npc[NPC_WhoAmi];
                                    n.realLife = NPC.whoAmI;
                                    n.life = n.lifeMax = 30000;
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null,
                                          NPC_WhoAmi);
                                    }
                                }
                            }
                            break;
                        }
                    case 12://原地转圈冲刺法
                        {
                            NPC.velocity = (NPC.rotation - 0.01f).ToRotationVector2() * 5;
                            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                            damage = 120;
                            Timer1++;
                            if (Timer1 > 10)
                            {
                                Timer1 = 0;
                                Timer2++;
                                if (Timer2 > 20)
                                {
                                    Timer2 = 0;
                                    State++;
                                    ShootSword();
                                    break;
                                }
                                else
                                {
                                    if (Main.netMode != 1)
                                    {
                                        Main.projectile[Projectile.NewProjectile(null, NPC.Center, NPC.velocity, ModContent.ProjectileType<LostSword2>(),
                                            damage, 2.3f, Main.myPlayer, 0, 1)].timeLeft = 600;
                                    }
                                }
                            }
                            break;
                        }
                    case 13://开幕仪式(?
                        {
                            NPC.velocity *= 0;
                            damage = 90;
                            NPC.rotation = MathHelper.ToRadians(Timer1 * 1.01f) + MathHelper.PiOver4;
                            if (Timer1 % 10 == 0)
                            {
                                for (int i = 0; i <= 100; i++)
                                {
                                    if (Main.netMode != 1)
                                    {
                                        Dust dust = Dust.NewDustDirect(NPC.Center + (i * MathHelper.TwoPi / 100).ToRotationVector2() * 800, 1, 1, DustID.FireworkFountain_Green);
                                        dust.noGravity = true;
                                    }
                                }
                            }
                            if (NPC.Distance(Target.Center) > 810)
                            {
                                Target.Center = NPC.Center + ToTarget.SafeNormalize(default) * 800;
                            }
                            else if (NPC.Distance(Target.Center) > 800)
                            {
                                Target.velocity = -ToTarget.SafeNormalize(default) * 3;
                            }
                            for (int i = 0; i < 16; i++)
                            {
                                Vector2 center = NPC.Center + (i * MathHelper.PiOver4 / 2).ToRotationVector2() * (Timer1 * 0.01f);
                                Vector2 vel = (Timer1 * 0.01f + i * MathHelper.PiOver4 / 2).ToRotationVector2();
                                Projectile.NewProjectile(null, center, vel, ModContent.ProjectileType<LostSwordLaser2>(), damage, 1.2f, Main.myPlayer
                                    , Timer1 < 50 ? 1 : 0);
                                Projectile.NewProjectile(null, center, -vel, ModContent.ProjectileType<LostSwordLaser2>(), damage, 1.2f, Main.myPlayer
                                    , Timer1 < 50 ? 1 : 0);
                            }
                            Timer1++;
                            if (Timer1 > 190)
                            {
                                Timer1 = 0;
                                State++;
                            }
                            break;
                        }
                    default:
                        if (NPC.life < NPC.lifeMax * 0.5f)
                        {
                            State = 11;
                            break;
                        }
                        State = 0;
                        break;
                }
            }
            else
            {
                Vector2 center = Target.position + (-ToTarget).SafeNormalize(default).RotatedBy(0.08) * 300;
                NPC.velocity = (center - NPC.position) * 0.3f;
                Timer1++;
                if (Timer1 > 50)
                {
                    Timer1 = 0;
                    if (Main.netMode != 1)
                    {
                        Projectile.NewProjectile(null, NPC.Center, ToTarget.SafeNormalize(default) * 3, ModContent.ProjectileType<LostSword2>(),
                            damage, 2.3f, Main.myPlayer, 3);
                    }
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagSowrd>()));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (swordHeartTalks == null)
            {
                swordHeartTalks = new SwordHeartTalk[8];
            }
            else
            {
                for (int i = 0; i < swordHeartTalks.Length; i++)
                {
                    if (!swordHeartTalks[i].Active)
                    {
                        swordHeartTalks[i].CheckActive();
                        break;
                    }
                    swordHeartTalks[i].Draw();
                }
            }
            Color color = new Color(0.4f, 0.5f, 0f, 0);
            if (Timer3 == 2 || Timer3 == 5)
            {
                color = new Color(0.3f, 0.8f, 0.1f, 0.2f);
            }

            StarBreakerUtils.NPCDrawTail(NPC, drawColor, color);
            StarBreakerUtils.EntityDrawLight(NPC, drawColor);
            return false;
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 2;
            if (State <= 5 && NPC.lifeMax * 0.5 > NPC.life && NPC.realLife == -1)
            {
                damage = 1;
            }
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "最终铜短剑";
            potionType = ItemID.SuperHealingPotion;
        }
        public override bool CheckActive()
        {
            return Target.dead;
        }

        public override bool CheckDead()
        {
            if (State <= 5)
            {
                NPC.life = (int)(NPC.lifeMax * 0.5f);
                return false;
            }
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<LostSword2>())
                {
                    projectile.Kill();
                }
            }
            return true;
        }
        private void ShootSword()
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<LostSword2>() && projectile.ai[0] < 2 && !projectile.friendly && projectile.hostile)
                {
                    projectile.ai[0] = 2;
                    projectile.ai[1] = 0;
                    projectile.timeLeft = 500;
                    if (projectile.damage <= 0)
                    {
                        projectile.damage = 120;
                    }
                }
            }
        }
        private void KillSword()
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<LostSword2>() && projectile.ai[0] == 2 && !projectile.friendly && projectile.hostile)
                {
                    projectile.Kill();
                }
            }
        }
        private void HealPlayerLife()
        {
            foreach (Player player in Main.player)
            {
                int healLife = player.statLifeMax2 - player.statLife;
                player.statLife += healLife;
                player.HealEffect(healLife);
            }
        }
    }
}
