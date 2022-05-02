using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.NPCs
{
    [AutoloadBossHead]
    internal class FrostFist : ModNPC
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
        private float Timer3
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        private int State
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        private readonly string[] sayText = new string[]
        {
            #region 一阶段台词
            "我想知道...",
            "没人待见我主人的理由",
            "人们通常说他很自私...但那是真的吗?",
            "星辰击碎者...一个沙雕...但是却很可靠",
            "终末之星杖...一个疯子，也确实是疯子",
            "星辰鬼刀...被封印也比我强大",
            "还有某个不知道去了哪里的旋刃...",
            "无论如何...我都是这样只能独自一对吗?",
            "可怜又可悲啊，我和我的主人都不能逃避被别人的优点伤害",
            "人也不一样,最后的最后，不也是被别人嘲笑吗?",
            "...",
            "你好...那个",
            "你听到了吧",
            "我想询问你,为什么人们会互相伤害",
            "...我不想听,即使你什么也没有说",
            "我只想...让我的主人不在因为人们对他的恶意而哭泣...",
            "如果你准备获得我...",
            "请你不要伤害我的主人...如果你赢了那也是前主人罢了...",
            "他也是我们的造物主...",
            "所以我会这么对他",
            "我不想抒情了,开始吧",
            #endregion
        };
        private Player Target => Main.player[NPC.target];
        public override string BossHeadTexture => this.Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("霜拳");
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.lifeMax = 50000;
            NPC.damage = 120;
            NPC.knockBackResist = 0f;
            NPC.defense = 18;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 16;
            NPC.height = 16;
            NPC.HitSound = SoundID.NPCHit4;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/StarGloveProvingGround");
            }
        }
        public override void AI()
        {
            if (Target.dead || !Target.active || NPC.target == 255 || NPC.target < 0)
            {
                NPC.TargetClosest();
            }
            Vector2 toTarget = Target.position - NPC.position;
            NPC.rotation = toTarget.ToRotation() + MathHelper.PiOver4;
            NPC.spriteDirection = NPC.direction;
            StarGlobalNPC.StarFrostFist = NPC.whoAmI;
            if (!SkyManager.Instance["StarBreaker:FrostFistSky"].IsActive())//开启天空
            {
                SkyManager.Instance.Activate("StarBreaker:FrostFistSky");
            }
            if (Target.dead)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y > 30) NPC.active = false;
                return;
            }
            switch (State)
            {
                case -1://生草对话
                    {
                        Timer1++;
                        NPC.position = Target.position + new Vector2(0, -150);
                        Color color = Color.Blue;
                        NPC.boss = false;
                        Music = -1;
                        NPC.dontTakeDamage = true;
                        Target.GetModPlayer<StarPlayer>().FrostFistModScr = NPC.whoAmI;
                        if (Timer1 / 50 >= 3)
                        {
                            SetDefaults();
                            NPC.boss = true;
                            NPC.dontTakeDamage = false;
                            NPC.life = 0;
                            NPC.checkDead();
                        }
                        else if (Timer1 % 50 == 0)
                        {
                            string Text = "等等我还没做完";
                            switch ((int)Timer1 / 50)
                            {
                                case 1:
                                    {
                                        Text = "那我是不是...要先加入你";
                                        break;
                                    }
                                case 2:
                                    {
                                        Text = "...我来了";
                                        break;
                                    }
                                case 3:
                                    {
                                        Text = "星 辰 拳 套 合 体 前 提 武 器 - 霜 拳 与 炎 拳 加 入 了 队 伍 !";
                                        break;
                                    }
                            }
                            PopupText.NewText(new AdvancedPopupRequest
                            {
                                Text = Text,
                                Color = color,
                                Velocity = new Vector2(0, -2),
                                DurationInFrames = 50
                            }, NPC.position);
                        }
                        break;
                    }
                case 0://开幕对话
                    {
                        Timer1++;
                        NPC.position = Target.position + new Vector2(0, -150);
                        Color color = Color.Blue;
                        NPC.boss = false;
                        Music = -1;
                        NPC.dontTakeDamage = true;
                        Target.GetModPlayer<StarPlayer>().FrostFistModScr = NPC.whoAmI;
                        if (Timer1 / 50 >= 21)
                        {
                            SetDefaults();
                            NPC.boss = true;
                            NPC.dontTakeDamage = false;
                            if (Target.name == "paparyus")
                            {
                                int npc = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)NPC.position.Y - 100, ModContent.NPCType<BurnFist>());
                                Main.npc[npc].localAI[0] = NPC.whoAmI;
                                State++;
                            }
                            else
                            {
                                State = -1;
                            }
                            Timer1 = 0;
                        }
                        else if (Timer1 % 50 == 0)
                        {
                            PopupText.NewText(new AdvancedPopupRequest
                            {
                                Text = sayText[(int)Timer1 / 50],
                                Color = color,
                                Velocity = new Vector2(0, -2),
                                DurationInFrames = 50
                            }, NPC.position);
                        }
                        break;
                    }
                case 1://发射冰柱
                    {
                        NPC.velocity = (NPC.velocity * 10 + toTarget.SafeNormalize(toTarget)) / 11;
                        Timer1++;
                        if (Timer1 > 20)
                        {
                            Vector2 center = NPC.Center + (Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 50);
                            for (float r = 0; r <= MathHelper.TwoPi; r += MathHelper.Pi / 10)
                            {
                                Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.IceGolem);
                                dust.noGravity = true;
                                dust.scale = 1.5f;
                                dust.velocity = r.ToRotationVector2() * 3;
                                dust.velocity.X /= 2.5f;
                                dust.velocity = dust.velocity.RotatedBy(NPC.rotation - MathHelper.PiOver4);
                            }
                            if (Main.netMode != 1)
                            {
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, toTarget.SafeNormalize(toTarget) * 10, ModContent.ProjectileType<Projs.IceThorn>(), 110, 1.2f, Main.myPlayer);
                                Main.projectile[proj].friendly = false;
                                Main.projectile[proj].hostile = true;
                            }
                            Timer1 = 0;
                            Timer2++;
                            if (Timer2 > 10)
                            {
                                Timer2 = 0;
                                State++;
                            }
                        }
                        break;
                    }
                case 2://蓄力冲刺
                    {
                        switch (Timer3)
                        {
                            case 0://蓄力
                                {
                                    NPC.velocity *= 0.9f;
                                    if (NPC.velocity.X < 0.1f && NPC.velocity.X > -0.1f)
                                    {
                                        Timer1++;
                                        if (Timer1 % 20 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 10)
                                            {
                                                Vector2 center = NPC.Center + i.ToRotationVector2() * 50;
                                                Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.IceGolem);
                                                dust.noGravity = true;
                                                dust.scale = 1.5f;
                                                dust.velocity = (NPC.Center - center) / 13;
                                            }
                                            Timer2++;
                                            if (Timer2 > 5)
                                            {
                                                Timer1 = 0;
                                                Timer2 = 0;
                                                Timer3++;
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 1://冲刺
                                {
                                    if (Timer1 <= 0)
                                    {
                                        Timer1 = 30;
                                        Timer2++;
                                        NPC.velocity = toTarget.SafeNormalize(toTarget) * 20;
                                        if (Timer2 > 3)
                                        {
                                            Timer2 = 0;
                                            Timer3 = 0;
                                            State++;
                                        }
                                    }
                                    else Timer1--;
                                    if (Main.rand.NextBool(20))
                                    {
                                        Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.IceGolem);
                                        dust.noGravity = true;
                                        dust.scale = 1.5f;
                                    }
                                    break;
                                }
                            default:
                                {
                                    Timer3 = 0;
                                    break;
                                }
                        }
                        break;
                    }
                case 3://幻影拳
                    {
                        float speed = toTarget.Length() > 500 ? 500 : toTarget.Length();
                        NPC.velocity = toTarget.SafeNormalize(toTarget) * speed / 50;
                        Timer1++;
                        Vector2 ves = NPC.velocity.SafeNormalize(toTarget).RotateRandom(MathHelper.ToRadians(60));
                        if (Main.netMode != 1)
                        {
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (ves * 50), ves, ModContent.ProjectileType<Projs.OuLa>(),
                                170, 2f, Main.myPlayer);
                            Main.projectile[proj].alpha = Main.rand.Next(100);
                        }
                        if (Timer1 > 120)
                        {
                            State++;
                            Timer1 = 0;
                        }
                        break;
                    }
                case 4://坠击
                    {
                        switch (Timer3)
                        {
                            case 0://飞到玩家头顶
                                {
                                    NPC.velocity = (NPC.velocity * 5 + ((Target.position + new Vector2(0, -400) - NPC.position)) / 10) / 6;
                                    if (NPC.velocity.Length() < 3f)
                                    {
                                        Timer3++;
                                    }
                                    break;
                                }
                            case 1://下坠
                                {
                                    NPC.velocity.Y = 10;
                                    Timer1++;
                                    if (Timer1 > 30)
                                    {
                                        if (Main.netMode != 1)
                                        {
                                            for (int i = -1; i <= 1; i++)
                                            {
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.position, toTarget.SafeNormalize(toTarget).RotatedBy(i * MathHelper.Pi / 20) * 10, ModContent.ProjectileType<Projs.IceThorn>(), 110, 1.2f, Main.myPlayer);
                                            }
                                        }
                                        Timer1 = 0;
                                        Timer3 = 0;
                                        State++;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 5://圣拳连击
                    {
                        switch (Timer3)
                        {
                            case 0://刺拳
                                {
                                    if (Timer1 <= 0)
                                    {
                                        NPC.velocity = toTarget.SafeNormalize(toTarget) * 10;
                                        Timer1 = 30;
                                        Timer2++;
                                        if (Timer2 > 1)
                                        {
                                            Timer1 = 0;
                                            Timer2 = 0;
                                            Timer3++;
                                        }
                                    }
                                    else Timer1--;
                                    if (Main.netMode != 1)
                                    {
                                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.SafeNormalize(Vector2.Zero), ModContent.ProjectileType<Projs.OuLa>(),
                                            170, 2f, Main.myPlayer);
                                        Main.projectile[proj].alpha = Main.rand.Next(100);
                                    }
                                    break;
                                }
                            case 1://直拳
                                {
                                    if (Main.netMode != 1)
                                    {
                                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.SafeNormalize(Vector2.Zero), ModContent.ProjectileType<Projs.OuLa>(),
                                            170, 2f, Main.myPlayer);
                                        Main.projectile[proj].alpha = Main.rand.Next(100);
                                        Main.projectile[proj].timeLeft = 90;
                                    }
                                    Timer3++;
                                    break;
                                }
                            case 2://摆拳
                                {
                                    NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / 60);
                                    Timer1++;
                                    if (Timer1 > 10)
                                    {
                                        if (Main.netMode != 1)
                                        {
                                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.SafeNormalize(Vector2.Zero), ModContent.ProjectileType<Projs.OuLa>(),
                                                170, 2f, Main.myPlayer);
                                            Main.projectile[proj].alpha = Main.rand.Next(100);
                                            Main.projectile[proj].timeLeft = 30;
                                        }
                                        Timer2++;
                                        Timer1 = 0;
                                        if (Timer2 > 3)
                                        {
                                            Timer2 = 0;
                                            Timer3 = 0;
                                            State++;
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 6://破碎之锤
                    {
                        Timer1++;
                        if (Timer1 < 50)
                        {
                            NPC.velocity = ((NPC.velocity * 2) + -toTarget.SafeNormalize(toTarget) * 8) / 3;
                        }
                        else
                        {
                            NPC.velocity.X = 0;
                            NPC.velocity.Y++;
                            if (NPC.velocity.Y > 10)
                            {
                                State++;
                                Timer1 = 0;
                                if (Main.netMode != 1)
                                {
                                    for (float r = 0; r <= MathHelper.TwoPi; r += MathHelper.Pi / 10)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.position, r.ToRotationVector2() * 5, ModContent.ProjectileType<Projs.IceThorn>(),
                                            110, 2.3f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 7://毁灭之冲
                    {
                        if (NPC.life >= NPC.lifeMax * 0.5f)
                        {
                            State = 1;
                            break;
                        }
                        else
                        {
                            switch (Timer3)
                            {
                                case 0://蓄力
                                    {
                                        NPC.velocity *= 0.9f;
                                        if (NPC.velocity.X < 0.1f && NPC.velocity.X > -0.1f)
                                        {
                                            Timer1++;
                                            if (Timer1 % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                                            {
                                                for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 10)
                                                {
                                                    Vector2 center = NPC.Center + i.ToRotationVector2() * 50;
                                                    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.IceGolem);
                                                    dust.noGravity = true;
                                                    dust.scale = 1.5f;
                                                    dust.velocity = (NPC.Center - center) / 13;
                                                }
                                                Timer2++;
                                                if (Timer2 > 5)
                                                {
                                                    Timer1 = 0;
                                                    Timer2 = 0;
                                                    Timer3++;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case 1://冲刺
                                    {
                                        if (Timer1 <= 0)
                                        {
                                            Timer1 = 30;
                                            Timer2++;
                                            NPC.velocity = toTarget.SafeNormalize(toTarget) * 25;
                                            if (Timer2 > 1)
                                            {
                                                Timer2 = 0;
                                                Timer3 = 0;
                                                State++;
                                            }
                                        }
                                        else Timer1--;
                                        if (Main.rand.NextBool(20))
                                        {
                                            Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.IceGolem);
                                            dust.noGravity = true;
                                            dust.scale = 1.5f;
                                            if (Main.netMode != 1)
                                            {
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center,
                                                    NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2) * 8, ModContent.ProjectileType<Projs.IceThorn>(),
                                                    170, 2f, Main.myPlayer);
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        Timer3 = 0;
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case 8://出来吧！天堂之主！
                    {
                        switch (Timer3)
                        {
                            case 0:
                                {
                                    NPC.velocity *= 0.6f;
                                    if (NPC.velocity.Length() < 0.1f)
                                    {
                                        Timer3++;
                                        Timer1 = 0;
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (Timer1 == 1)
                                    {
                                        PopupText.NewText(new AdvancedPopupRequest
                                        {
                                            Text = "出来吧!天堂之主!",
                                            Color = Color.Blue,
                                            Velocity = new Vector2(0, -5),
                                            DurationInFrames = 200
                                        }, Target.position);
                                    }
                                    Timer1++;
                                    NPC.velocity *= 0.9f;
                                    if (Timer1 % 15 == 0)
                                    {
                                        for (int i = -2; i <= 2; i++)
                                        {
                                            Vector2 center = Target.position + new Vector2((400 - (Timer1 * 2)) * i, Main.screenPosition.Y - Target.position.Y);
                                            if (Main.netMode != 1)
                                            {
                                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, new Vector2(0, 5), ModContent.ProjectileType<Projs.IceThorn>(),
                                                    120, 4f, Main.myPlayer);
                                                Main.projectile[proj].alpha = 255;
                                            }
                                        }
                                    }
                                    else if (Timer1 > 100)
                                    {
                                        Timer1 = 0;
                                        Timer3 = 0;
                                        State++;
                                    }
                                    break;
                                }
                            default:
                                {
                                    Timer3 = 0;
                                    break;
                                }
                        }
                        break;
                    }
                case 9://狙击冲刺
                    {
                        switch (Timer3)
                        {
                            case 0://狙击
                                {
                                    NPC.velocity *= 0.9f;
                                    if (NPC.velocity.X < 0.1f && NPC.velocity.X > -0.1f)
                                    {
                                        Timer1++;
                                        if (Timer1 % 20 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Timer2++;
                                            if (Timer2 > 1)
                                            {
                                                Timer1 = 0;
                                                Timer2 = 0;
                                                Timer3++;
                                                break;
                                            }
                                            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 2)
                                            {
                                                Vector2 center = Target.position + i.ToRotationVector2() * 500;
                                                if (Main.netMode != 1)
                                                {
                                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), center, (Target.position - center).SafeNormalize(toTarget) * 20,
                                                        ModContent.ProjectileType<Projs.IceThorn>(), 120, 4f, Main.myPlayer);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 1://冲刺
                                {
                                    if (Timer1 <= 0)
                                    {
                                        Timer1 = 30;
                                        Timer2++;
                                        NPC.velocity = toTarget.SafeNormalize(toTarget) * 25;
                                        if (Timer2 > 1)
                                        {
                                            Timer2 = 0;
                                            Timer3 = 0;
                                            State++;
                                        }
                                    }
                                    else Timer1--;
                                    break;
                                }
                        }
                        break;
                    }
                case 10://星辰拳套特殊连携攻击
                    {
                        switch (Timer3)
                        {
                            case 0://霜拳去到玩家头顶
                                {
                                    Vector2 center = Target.position + new Vector2(0, -300);
                                    NPC.velocity = (NPC.velocity * 100 + (center - NPC.position) * 0.1f) / 101;
                                    break;
                                }
                            case 1://向下冲刺
                                {
                                    NPC.velocity = new Vector2(0, 5);
                                    if (toTarget.Y < 200)
                                    {
                                        Timer3++;
                                    }
                                    break;
                                }
                            case 2://绕圈
                                {
                                    Vector2 center = NPC.position + (((Target.position - NPC.position).ToRotation() - 0.1f).ToRotationVector2() * 300);
                                    NPC.velocity = (center - NPC.position) * 0.5f;
                                    break;
                                }
                            case 3://重新冲刺
                                {
                                    if (Timer1 <= 0)
                                    {
                                        Timer2++;
                                        NPC.velocity = toTarget.SafeNormalize(toTarget) * 20;
                                        if (Timer2 > 1)
                                        {
                                            Timer2 = 0;
                                            Timer3++;
                                        }
                                    }
                                    else Timer1--;
                                    break;
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
            if (NPC.spriteDirection == -1) NPC.rotation += MathHelper.Pi;
            else NPC.rotation += MathHelper.PiOver2 + MathHelper.Pi;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapon.FrostFistW>(), 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapon.BurnFistW>(), 1));
        }
        public override void OnKill()
        {
            StarBreakerSystem.downedStarFist = true;
        }
        public override bool CheckDead()
        {
            if (State < 11 && State > -1)
            {
                NPC.life = NPC.lifeMax;
                NPC.dontTakeDamage = true;
                return false;
            }
            return true;
        }
        public override bool CheckActive()
        {
            return Target.dead;
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
            #region Draw总体
            Texture2D NPCTexture = TextureAssets.Npc[NPC.type].Value;
            int frameCount = Main.npcFrameCount[NPC.type];
            Vector2 DrawOrigin = new Vector2(NPCTexture.Width / 2, NPCTexture.Height / frameCount / 2);
            switch (State)
            {
                case 1:
                    {
                        if (Timer3 == 0)
                        {
                            Texture2D texture = ModContent.Request<Texture2D>("StarBreaker/Projs/Type/EnergyProj").Value;
                            Color color = Color.Blue;
                            color.A = (byte)(Timer1 / 120f * 255);
                            color.B = color.A;
                            spriteBatch.Draw(texture, NPC.Center - screenPos, null, color, (Target.position - NPC.position).ToRotation(), new Vector2(0, texture.Height / 2), new Vector2(100, 3), 0f, 0f);
                        }
                        else
                        {
                            for (int i = 0; i < NPC.oldPos.Length; i += 2)
                            {
                                Vector2 DrawPosition = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                                DrawPosition -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * NPC.scale / 2f;
                                DrawPosition += DrawOrigin * NPC.scale + new Vector2(0f, 4f + NPC.gfxOffY);
                                spriteBatch.Draw(NPCTexture,
                                    DrawPosition,
                                    new Rectangle?(NPC.frame),
                                    new Color(0.5f, 0.5f, i / 6, (NPC.oldPos.Length - i) / NPC.oldPos.Length),
                                    NPC.oldRot[i],
                                    DrawOrigin,
                                    1f,
                                    NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                    0f);
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            Vector2 DrawPosition = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                            DrawPosition -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * NPC.scale / 2f;
                            DrawPosition += DrawOrigin * NPC.scale + new Vector2(0f, 4f + NPC.gfxOffY);
                            spriteBatch.Draw(NPCTexture,
                                DrawPosition,
                                new Rectangle?(NPC.frame),
                                new Color(0.5f, 0.5f, i / 6, (NPC.oldPos.Length - i) / NPC.oldPos.Length),
                                NPC.rotation,
                                DrawOrigin,
                                1f,
                                NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                0f);
                        }
                        break;
                    }
                case 7:
                    {
                        if (NPC.life >= NPC.lifeMax * 0.5f)
                        {
                            break;
                        }
                        else
                        {
                            if (Timer3 == 0)
                            {
                                Texture2D texture = ModContent.Request<Texture2D>("StarBreaker/Projs/Type/EnergyProj").Value;
                                Color color = Color.Blue;
                                color.A = (byte)(Timer1 / 120f * 255);
                                color.B = color.A;
                                spriteBatch.Draw(texture, NPC.Center - screenPos, null, color, (Target.position - NPC.position).ToRotation(), new Vector2(0, texture.Height / 2), new Vector2(100, 3), 0f, 0f);
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (Timer3 == 0)
                        {
                            Texture2D texture = ModContent.Request<Texture2D>("StarBreaker/Projs/SniperGloves").Value;
                            spriteBatch.Draw(texture, Target.Center - Main.screenPosition, null, new Color(0, 100, 255, 0) * MathHelper.Min(1, Timer1 / 60),
                                0, texture.Size() * 0.5f, 1f, 0, 0f);
                        }
                        break;
                    }
            }
            #endregion
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
