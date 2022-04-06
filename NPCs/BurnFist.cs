using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.NPCs
{
    [AutoloadBossHead]
    internal class BurnFist : ModNPC
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
        private Player Target => Main.player[NPC.target];
        public override string BossHeadTexture => this.Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("炎拳");
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.lifeMax = 45000;
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
            NPC FrostFist = Main.npc[(int)NPC.localAI[0]];
            if (FrostFist.type != ModContent.NPCType<FrostFist>() || !FrostFist.active)
            {
                NPC.active = false;
            }
            if (Target.dead)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y > 30) NPC.active = false;
                return;
            }
            switch (State)
            {
                case 0://发射火拳
                    {
                        NPC.velocity = (NPC.velocity * 10 + toTarget.SafeNormalize(toTarget)) / 11;
                        Timer1++;
                        if (Timer1 > 120)
                        {
                            Vector2 center = NPC.Center + ((NPC.rotation - MathHelper.PiOver4).ToRotationVector2() * -20);
                            for (float r = 0; r <= MathHelper.TwoPi; r += MathHelper.Pi / 10)
                            {
                                Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.FireworkFountain_Red);
                                dust.noGravity = true;
                                dust.velocity = r.ToRotationVector2() * 3;
                                dust.velocity.X /= 2.5f;
                                dust.velocity = dust.velocity.RotatedBy(NPC.rotation - MathHelper.PiOver4);
                            }
                            if (Main.netMode != 1)
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), center, toTarget.SafeNormalize(toTarget).RotatedBy(i * MathHelper.Pi / 18) * 10, ModContent.ProjectileType<Projs.FireFist>(), 110, 1.2f, Main.myPlayer);
                                    Main.projectile[proj].friendly = false;
                                    Main.projectile[proj].hostile = true;
                                }
                            }
                            Timer1 = 115;
                            Timer2++;
                            if (Timer2 > 5)
                            {
                                Timer2 = 0;
                                State++;
                            }
                        }
                        break;
                    }
                case 1://蓄力冲刺
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
                                                Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.FireworkFountain_Red);
                                                dust.noGravity = true;
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
                                        Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.FireworkFountain_Red);
                                        dust.noGravity = true;
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
                case 2://幻影拳
                    {
                        float speed = toTarget.Length() > 500 ? 500 : toTarget.Length();
                        NPC.velocity = toTarget.SafeNormalize(toTarget) * speed / 50;
                        Timer1++;
                        Vector2 ves = NPC.velocity.SafeNormalize(toTarget).RotateRandom(MathHelper.ToRadians(60));
                        if (Main.netMode != 1)
                        {
                            int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center + (ves * 50), ves, ModContent.ProjectileType<Projs.OuLa>(),
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
                case 3://强化结界
                    {
                        Timer1++;
                        NPC.velocity = (NPC.velocity * 50 + (FrostFist.position + new Vector2(0, -50) - NPC.position).SafeNormalize(toTarget) * 10) / 51;
                        if (Vector2.Distance(FrostFist.position, NPC.position) < 800)
                        {
                            FrostFist.defense = 100;
                            FrostFist.damage = 200;
                        }
                        else
                        {
                            FrostFist.defense = FrostFist.defDefense;
                            FrostFist.damage = FrostFist.defDamage;
                        }
                        for (float r = 0; r <= MathHelper.TwoPi; r += MathHelper.Pi / 20)
                        {
                            Vector2 center = NPC.position + (r.ToRotationVector2() * 800);
                            Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.FireworkFountain_Red);
                            dust.velocity *= 0f;
                            dust.noGravity = true;
                        }
                        if (Timer1 > 240)
                        {
                            FrostFist.defense = FrostFist.defDefense;
                            FrostFist.damage = FrostFist.defDamage;
                            Timer1 = 0;
                            State++;
                        }
                        break;
                    }
                case 4://从地而出的火
                    {
                        NPC.velocity *= 0.9f;
                        if (NPC.velocity.Length() < 0.1f)
                        {
                            Timer1++;
                            Vector2 center = new(Timer2, Timer3);
                            if (Timer1 % 30 == 0)
                            {
                                for (int i = 1; i <= 40; i++)
                                {
                                    Tile tile = Main.tile[(int)(Target.position.X / 16), (int)((Target.position.Y / 16) + i)];
                                    center = Target.position + new Vector2(0, 16 * i);
                                    Timer2 = center.X;
                                    Timer3 = center.Y;
                                    if (tile.HasTile) break;
                                }
                                for (float r = 0; r <= MathHelper.TwoPi; r += MathHelper.Pi / 100)
                                {
                                    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.FireworkFountain_Red);
                                    dust.noGravity = true;
                                    dust.velocity = r.ToRotationVector2() * 10;
                                    dust.velocity.X /= 10f;
                                    dust.velocity = dust.velocity.RotatedBy(MathHelper.PiOver2);
                                }
                            }
                            if (Timer1 >= 85)
                            {
                                if (Main.netMode != 1)
                                {
                                    for (int i = -2; i <= 2; i++)
                                    {
                                        Vector2 projCenter = center + new Vector2(i * 50, 0);
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), projCenter, new Vector2(0, -5), ModContent.ProjectileType<Projs.FireFist>(),
                                            110, 1.5f, Main.myPlayer);
                                    }
                                }
                                Timer1 = 0;
                                Timer2 = 0;
                                Timer3 = 0;
                                State++;
                            }
                        }
                        break;
                    }
                case 5://抓取玩家
                    {
                        if (Timer3 == 0)
                        {
                            Timer1++;
                            NPC.velocity = (NPC.velocity * 50 + toTarget.SafeNormalize(toTarget) * 10) / 51;
                            for (float r = 0; r <= MathHelper.TwoPi; r += MathHelper.Pi / 20)
                            {
                                Vector2 center = NPC.position + (r.ToRotationVector2() * 50);
                                Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.FireworkFountain_Red);
                                dust.velocity *= 0f;
                                dust.noGravity = true;
                            }
                            if (Vector2.Distance(Target.position, NPC.position) < 50)
                            {
                                Timer3++;
                                Timer1 = 0;
                            }
                            if (Timer1 > 60)
                            {
                                Timer1 = 0;
                                State++;
                            }
                        }
                        else
                        {
                            NPC.velocity *= 0.9f;
                            Target.position = NPC.Center + NPC.velocity;
                            Timer1++;
                            if (Timer1 > 20)
                            {
                                Target.immune = false;
                                Target.immuneTime = 0;
                                if (Main.netMode != 1)
                                {
                                    Vector2 ves = toTarget.SafeNormalize(toTarget).RotateRandom(MathHelper.ToRadians(60));
                                    int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center + (ves * 50), ves, ModContent.ProjectileType<Projs.OuLa>(),
                                        20, 2f, Main.myPlayer);
                                    Main.projectile[proj].alpha = Main.rand.Next(100);
                                }
                                if (Timer1 > 100)
                                {
                                    Timer1 = 0;
                                    Timer3 = 0;
                                    State++;
                                }
                            }
                        }
                        break;
                    }
                case 6://绕着霜拳旋转
                    {
                        if (NPC.life >= NPC.lifeMax * 0.5f || FrostFist.life >= FrostFist.lifeMax * 0.5f)
                        {
                            if (NPC.life < NPC.lifeMax * 0.5f)
                            {
                                NPC.dontTakeDamage = true;
                            }
                            State = 0;
                            break;
                        }
                        else
                        {
                            NPC.dontTakeDamage = false;
                        }
                        Timer1++;
                        Vector2 center = FrostFist.position + (((NPC.position - FrostFist.position).ToRotation() - 0.1f).ToRotationVector2() * 300);
                        NPC.velocity = (center - NPC.position) * 0.5f;
                        if (Timer1 % 30 < 10)
                        {
                            Vector2 ves = toTarget.SafeNormalize(toTarget).RotateRandom(MathHelper.ToRadians(60));
                            if (Main.netMode != 1)
                            {
                                int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center + (ves * 50), ves, ModContent.ProjectileType<Projs.OuLa>(),
                                    170, 2f, Main.myPlayer);
                                Main.projectile[proj].alpha = Main.rand.Next(100);
                            }
                        }
                        if (Timer1 > 100)
                        {
                            Timer1 = 0;
                            State++;
                        }
                        break;
                    }
                case 7://二次函数冲刺
                    {
                        switch (Timer3)
                        {
                            case 0://转换位置,记录玩家位置
                                {
                                    NPC.velocity = (NPC.velocity * 5 + (Target.position + new Vector2(-250, -250) - NPC.position) * 0.1f) / 6;
                                    if (NPC.velocity.Length() < 5f)
                                    {
                                        Timer1 = Target.position.X;
                                        Timer2 = Target.position.Y;
                                        Timer3++;
                                    }
                                    break;
                                }
                            case 1://开始冲刺
                                {
                                    NPC.velocity.X = 5;
                                    NPC.velocity.Y = (NPC.velocity.Y * 3 + ((NPC.position - new Vector2(Timer1, Timer2)).X < 0 ? 10 : -10) * 1.1f) / 4;
                                    if (Vector2.Distance(NPC.position, new Vector2(Timer1, Timer2)) % 30 < 1)
                                    {
                                        if (Main.netMode != 1)
                                        {
                                            for (int i = -1; i <= 1; i++)
                                            {
                                                int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, toTarget.SafeNormalize(toTarget).RotatedBy(i * MathHelper.Pi / 18) * 10, ModContent.ProjectileType<Projs.FireFist>(), 110, 1.2f, Main.myPlayer);
                                                Main.projectile[proj].friendly = false;
                                                Main.projectile[proj].hostile = true;
                                            }
                                        }
                                    }
                                    if (NPC.position.X > Timer1 + 300)
                                    {
                                        Timer3 = 0;
                                        Timer1 = 0;
                                        Timer2 = 0;
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
                case 8://狙击火球
                    {
                        Timer1++;
                        NPC.velocity *= 0.9f;
                        if (Timer1 > 120)
                        {
                            Timer1 = 0;
                            Timer2++;
                            if (Main.netMode != 1)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.position, Vector2.One,
                                                        ModContent.ProjectileType<Projs.SniperGloves>(), 150, 4f, Main.myPlayer, Target.whoAmI, ModContent.ProjectileType<Projs.FireFist>());
                            }
                            if (Timer2 > 3)
                            {
                                Timer2 = 0;
                                State++;
                                Timer3 = 0;
                            }
                        }
                        break;
                    }
                case 9://星辰拳套特殊连携攻击
                    {
                        switch (Timer3)
                        {
                            case 0://炎拳冲刺
                                {
                                    if (FrostFist.ai[3] != 0)
                                    {
                                        if (Timer1 <= 0)
                                        {
                                            Timer1 = 10;
                                            Timer2++;
                                            NPC.velocity = toTarget.SafeNormalize(toTarget) * 20;
                                            if (Main.netMode != 1)
                                            {
                                                for (int i = -1; i <= 1; i++)
                                                {
                                                    int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, toTarget.SafeNormalize(toTarget).RotatedBy(i * MathHelper.Pi / 40) * 10, ModContent.ProjectileType<Projs.FireFist>(), 70, 1.2f, Main.myPlayer);
                                                    Main.projectile[proj].friendly = false;
                                                    Main.projectile[proj].hostile = true;
                                                }
                                            }
                                            if (Timer2 > 5)
                                            {
                                                Timer2 = 0;
                                                Timer3++;
                                                FrostFist.ai[3]++;//霜拳切换ai
                                            }
                                        }
                                        else Timer1--;
                                        if (Main.rand.NextBool(20))
                                        {
                                            Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.FireworkFountain_Red);
                                            dust.noGravity = true;
                                        }
                                    }
                                    else
                                    {
                                        NPC.velocity = toTarget * 0.1f;
                                    }
                                    break;
                                }
                            case 1://炎拳欧拉
                                {
                                    float speed = toTarget.Length() > 500 ? 500 : toTarget.Length();
                                    if (Timer1 % 60 < 1) NPC.velocity = toTarget.SafeNormalize(toTarget) * speed / 50;
                                    Timer1++;
                                    Vector2 ves = NPC.velocity.SafeNormalize(toTarget).RotateRandom(MathHelper.ToRadians(60));
                                    if (Main.netMode != 1)
                                    {
                                        int proj = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center + (ves * 50), ves, ModContent.ProjectileType<Projs.OuLa>(),
                                            170, 2f, Main.myPlayer);
                                        Main.projectile[proj].alpha = Main.rand.Next(100);
                                    }
                                    if (Timer1 > 120 && FrostFist.ai[3] == 2)
                                    {
                                        Timer1 = 0;
                                        Timer3++;
                                        FrostFist.ai[3]++;//霜拳切换ai
                                    }
                                    break;
                                }
                            case 2://炎拳摸鱼
                                {
                                    Vector2 center = Target.position + new Vector2(0, -300);
                                    NPC.velocity = (NPC.velocity * 100 + (center - NPC.position) * 0.1f) / 101;
                                    if (FrostFist.ai[0] != 10)
                                    {
                                        Timer3 = 0;
                                        State = 6;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        State = 0;
                        break;
                    }
            }
            if (NPC.spriteDirection == -1) NPC.rotation += MathHelper.Pi;
            else NPC.rotation += MathHelper.PiOver2 + MathHelper.Pi;
        }
        public override bool CheckActive()
        {
            return Target.dead;
        }

        public override bool CheckDead()
        {
            Main.npc[(int)NPC.localAI[0]].ai[0] = 11;
            Main.npc[(int)NPC.localAI[0]].ai[1] = 0;
            Main.npc[(int)NPC.localAI[0]].ai[2] = 0;
            Main.npc[(int)NPC.localAI[0]].ai[3] = 0;
            Main.npc[(int)NPC.localAI[0]].active = false;
            Main.npc[(int)NPC.localAI[0]].checkDead();
            return true;
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
            switch (State)
            {
                case 1:
                    {
                        if (Timer3 == 0)
                        {
                            Texture2D texture = ModContent.Request<Texture2D>("StarBreaker/Projs/Type/EnergyProj").Value;
                            Color color = Color.Red;
                            color.A = (byte)(Timer1 / 120f * 255);
                            color.R = color.A;
                            spriteBatch.Draw(texture, NPC.Center - screenPos, null, color, (Target.position - NPC.position).ToRotation(), new Vector2(0, texture.Height / 2), new Vector2(100, 3), 0f, 0f);
                        }
                        else
                        {
                            for (int i = 0; i < NPC.oldPos.Length; i += 2)
                            {
                                Texture2D NPCTexture = TextureAssets.Npc[NPC.type].Value;
                                int frameCount = Main.npcFrameCount[NPC.type];
                                Vector2 DrawOrigin = new Vector2(NPCTexture.Width / 2, NPCTexture.Height / frameCount / 2);
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
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
