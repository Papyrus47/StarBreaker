using StarBreaker.Projs.UltimateCopperShortsword;
using System.IO;

namespace StarBreaker.NPCs.UltimateCopperShortsword.Bosses
{
    //第一阶段的最终同志短剑
    [AutoloadBossHead]
    public class ShortSword : FSMNPC
    {
        private int damage
        {
            get => (int)(NPC.damage * (Main.expertMode ? 0.25f : 1f));
            set => NPC.damage = value;
        }
        private Vector2 MK5_2_Center;
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
                    "对...是你抛弃了我...",
                    "我要对你实行复仇,因为你抛弃了我",
                    "体验这份被抛弃的感受吧!",
                    "我不会就此消失...",
                    "我是那么害怕被不被人重视..."
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
                    Color color = Color.Orange;
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
            DisplayName.SetDefault("Short Sword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铜短剑");
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 35000;
            NPC.defense = 5;
            NPC.damage = 50;
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
            Music = MusicLoader.GetMusicSlot("StarBreaker/Music/Atk1");
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode == 2)
            {
                base.SendExtraAI(writer);
                writer.WritePackedVector2(MK5_2_Center);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != 1)
            {
                base.ReceiveExtraAI(reader);
                MK5_2_Center = reader.ReadPackedVector2();
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
                NPC.life = 0;
                return;
            }
            switch (State)
            {
                case 0://压抑的愤怒
                    {
                        if (Timer1 <= 0)
                        {
                            Timer1 = 65;
                            Timer2++;
                            if (Timer2 > 5)
                            {
                                Timer2 = 0;
                                Timer1 = 0;
                                State++;
                                break;
                            }
                            NPC.velocity = ToTarget.SafeNormalize(default) * 15;
                        }
                        else
                        {
                            Timer1--;
                            if (Timer1 % 10 == 0)
                            {
                                damage = 100;
                                if (Main.netMode != 1)
                                {
                                    Projectile.NewProjectile(null, Target.Center + new Vector2(Main.rand.Next(-50, 50), -300),
                                        new Vector2(0, 5), ModContent.ProjectileType<LostSword>(), damage, 1.2f, Main.myPlayer, 0, 1);
                                }
                            }
                            if (Timer1 < 2)
                            {
                                ShootSword();
                            }
                        }
                        break;
                    }
                case 1://MK5-2
                    {
                        if (Timer1 <= 0)
                        {
                            MK5_2_Center = Target.position + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 80;
                            Timer1 = 20;
                            Timer2++;
                            if (Timer2 > 12)
                            {
                                Timer2 = 0;
                                Timer1 = 0;
                                State++;
                                break;
                            }
                            for (float i = 0; i <= 10; i++)
                            {
                                if (Main.netMode != 1)
                                {
                                    Dust dust = Dust.NewDustDirect(MK5_2_Center, 1, 1, DustID.Confetti_Yellow);
                                    dust.velocity = (i * MathHelper.TwoPi / 10).ToRotationVector2();
                                    dust.noGravity = true;
                                }
                            }
                        }
                        else
                        {
                            NPC.velocity = (MK5_2_Center - NPC.position) * 0.1f;
                            Timer1--;
                        }
                        break;
                    }
                case 2://忘却
                    {
                        switch (Timer3)
                        {
                            case 0://蓄力
                                {
                                    Timer1++;
                                    NPC.velocity *= 0.9f;
                                    if (Timer1 % 10 == 0)
                                    {
                                        for (float i = 0; i <= 10; i++)
                                        {
                                            if (Main.netMode != 1)
                                            {
                                                Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.Confetti_Yellow);
                                                dust.velocity = (i * MathHelper.TwoPi / 10).ToRotationVector2();
                                                dust.velocity.X *= 0.9f;
                                                dust.noGravity = true;
                                            }
                                        }
                                    }
                                    else if (Timer1 > 60)
                                    {
                                        Timer1 = 0;
                                        Timer3++;
                                    }
                                    break;
                                }
                            case 1://冲刺
                                {
                                    NPC.velocity = ToTarget.SafeNormalize(default) * 20;
                                    Timer1 = 0;
                                    Timer2 = 0;
                                    Timer3++;
                                    damage = 110;
                                    if (Main.netMode != 1)
                                    {
                                        for (float i = 0; i < 30; i++)
                                        {
                                            Vector2 vel = (ToTarget.SafeNormalize(default) * 5).RotatedBy(i * (MathHelper.Pi / 15));
                                            Main.projectile[Projectile.NewProjectile(null, NPC.Center, vel, ModContent.ProjectileType<LostSword>(),
                                                damage, 1.2f, Main.myPlayer, 0, 1)].timeLeft = 1000;
                                        }
                                    }
                                    break;
                                }
                            case 2://减速
                                {
                                    NPC.velocity *= 0.99f;
                                    if (NPC.velocity.Length() < 5)
                                    {
                                        State++;
                                        Timer3 = 0;
                                        ShootSword();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 3://血欲
                    {
                        if (NPC.alpha < 255 && Timer3 == 0)
                        {
                            NPC.alpha += 5;
                        }
                        else
                        {
                            switch (Timer3)
                            {
                                case 0://传送
                                    {
                                        Timer1++;
                                        NPC.position = Target.position + new Vector2(0, -600);
                                        if (Timer1 > 10)
                                        {
                                            Timer1 = 0;
                                            Timer3++;
                                            NPC.alpha = 0;
                                        }
                                        break;
                                    }
                                case 1://向下冲刺
                                    {
                                        NPC.velocity = new Vector2(0, 20);
                                        if (Main.netMode != 1)
                                        {
                                            for (int i = -20; i <= 20; i++)
                                            {
                                                Vector2 center = NPC.Center + new Vector2(i * Target.width * 8, Math.Abs(i) * 30);
                                                Projectile.NewProjectile(null, center, NPC.velocity / 2, ModContent.ProjectileType<LostSword>(),
                                                    damage, 1.2f, Main.myPlayer, 3);
                                            }
                                        }
                                        Timer3++;
                                        break;
                                    }
                                case 2:
                                    {
                                        NPC.velocity *= 0.9f;
                                        if (NPC.velocity.Length() < 1)
                                        {
                                            State++;
                                            Timer1 = 0;
                                            Timer3 = 0;
                                        }
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case 4://我们的小小银河
                    {
                        NPC.velocity *= 0.9f;
                        if (NPC.velocity.Length() < 1)
                        {
                            Timer1++;
                            if (Timer1 <= 1)
                            {
                                if (Main.netMode != 1)
                                {
                                    for (int i = -20; i <= 20; i++)
                                    {
                                        Vector2 center = Target.Center + new Vector2(i * 250, -700);
                                        Main.projectile[Projectile.NewProjectile(null, center, new Vector2(-1, 0.97f) * 15, ModContent.ProjectileType<LostSword>(),
                                                    damage, 1.2f, Main.myPlayer, 0, 2)].timeLeft = 1000;
                                    }
                                }
                            }
                            else if (Timer1 > 20)
                            {
                                Timer1 = 0;
                                State++;
                            }
                        }
                        break;
                    }
                case 5://绕圈旋转的激光
                    {
                        if (Timer3 == 0)
                        {
                            NPC.velocity *= 0.9f;
                            Timer1++;
                            if (Timer1 > 100)
                            {
                                Timer1 = 0;
                                ShootSword();
                                Timer3++;
                            }
                        }
                        else
                        {
                            if (NPC.velocity.Length() > 5 || NPC.velocity.Length() < 1)
                            {
                                NPC.velocity = ToTarget.SafeNormalize(default) * 2;
                            }
                            else
                            {
                                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(2.1f));
                            }

                            NPC.dontTakeDamage = true;
                            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                            Timer1++;
                            damage = 150;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(null, NPC.Center, NPC.velocity.SafeNormalize(default), ModContent.ProjectileType<LostSwordLaser>(),
                                    damage, 2.3f, Main.myPlayer, Timer1 < 100 ? 1 : 0);
                            }
                            if (Timer1 > 300)
                            {
                                Timer1 = 0;
                                Timer3 = 0;
                                NPC.dontTakeDamage = false;
                                State++;
                            }
                        }
                        break;
                    }
                default:
                    State = 0;
                    break;
            }
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
            switch (State)
            {
                case 1:
                case 2:
                    {
                        StarBreakerUtils.NPCDrawTail(NPC, drawColor, Color.Purple);
                        break;
                    }
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "铜短剑";
            potionType = ItemID.SuperHealingPotion;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 2;
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage *= 4;
        }
        public override bool CheckActive()
        {
            if (Target.dead)
            {
                return true;
            }

            return false;
        }
        public override bool CheckDead()
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<LostSword>())
                {
                    projectile.Kill();
                }
            }
            return true;
        }
        public override void OnKill()
        {
            NPC.NewNPC(NPC.GetSource_Death(), (int)Target.Center.X + Main.rand.Next(500, 600),
                (int)Target.Center.Y + Main.rand.Next(500, 600), ModContent.NPCType<ShortSword2>());
            foreach (Player player in Main.player)
            {
                int healLife = player.statLifeMax2 - player.statLife;
                player.statLife += healLife;
                player.HealEffect(healLife);
            }
        }
        private void ShootSword()
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<LostSword>() && projectile.ai[0] < 2 && !projectile.friendly && projectile.hostile)
                {
                    projectile.ai[0] = 2;
                    projectile.ai[1] = 0;
                    projectile.timeLeft = 500;
                }
            }
        }
    }
}
