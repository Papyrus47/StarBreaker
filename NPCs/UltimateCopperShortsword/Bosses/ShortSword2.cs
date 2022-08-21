using StarBreaker.Projs.UltimateCopperShortsword;
using System.IO;

namespace StarBreaker.NPCs.UltimateCopperShortsword.Bosses
{
    [AutoloadBossHead]
    public class ShortSword2 : FSMNPC
    {
        private Vector2 WingVib;//翅振
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
                Text = Main.rand.Next<string>(new string[7]
                {
                    "随着时间,我逐渐生锈...",
                    "你承受不住,这份剑的重量",
                    "我只是想被人使用,但是为什么你却把我丢箱子底",
                    "没有我,你哪来的今天?",
                    "即便生锈,我的力量也不减",
                    "我要让你见识武器的力量",
                    "..."
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
                    Color color = Color.Lerp(Color.Orange, Color.Green, 0.5f);
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
            NPCID.Sets.TrailCacheLength[NPC.type] = 20;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 40000;
            NPC.defense = 10;
            NPC.damage = 55;
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
            Music = MusicLoader.GetMusicSlot("StarBreaker/Music/Atk2");
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                base.SendExtraAI(writer);
                writer.WritePackedVector2(WingVib);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                base.ReceiveExtraAI(reader);
                WingVib = reader.ReadPackedVector2();
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
                case 0://翅振
                case 6:
                    {
                        if (Timer1 <= 0)
                        {
                            WingVib = Target.position + ToTarget.SafeNormalize(default) * 300;
                            Timer1 = 30 - (Timer2 * 2);
                            Timer2++;
                            if (Timer2 > 6)
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
                                    Dust dust = Dust.NewDustDirect(WingVib, 1, 1, DustID.FireworkFountain_Green);
                                    dust.velocity = (i * MathHelper.TwoPi / 10).ToRotationVector2();
                                    dust.noGravity = true;
                                }
                            }
                        }
                        else
                        {
                            NPC.velocity = (WingVib - NPC.position) * 0.2f;
                            if (NPC.velocity.Length() > 30f)
                            {
                                NPC.velocity = NPC.velocity.SafeNormalize(default) * 30;
                            }

                            Timer1--;
                        }
                        break;

                    }
                case 1://黄蜂
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
                                    break;
                                }
                            case 2://等待
                                {
                                    Timer1++;
                                    if (Timer1 % 5 == 0)
                                    {
                                        if (Main.netMode != 1)
                                        {
                                            for (int i = 0; i <= 1; i++)
                                            {
                                                Vector2 vel = NPC.velocity.SafeNormalize(default).RotatedBy(i == 0 ? MathHelper.PiOver2 : -MathHelper.PiOver2);
                                                Projectile.NewProjectile(null, NPC.Center, vel * 10, ModContent.ProjectileType<LostSword2>(),
                                                    damage, 1.2f, Main.myPlayer, 0, 1);
                                            }
                                        }
                                    }
                                    else if (Timer1 > 30)
                                    {
                                        Timer3 = 0;
                                        Timer1 = 0;
                                        State++;
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
                case 2://庄严哀悼
                    {
                        NPC.velocity *= 0.9f;
                        if (NPC.velocity.Length() < 0.2f)
                        {
                            Timer1++;
                            if (Timer1 > 30)
                            {
                                Timer1 = 0;
                                Timer2++;
                                if (Timer2 % 3 == 0)
                                {
                                    ShootSword();
                                }
                                else if (Timer2 > 9)
                                {
                                    ShootSword();
                                    Timer1 = 0;
                                    Timer2 = 0;
                                    State++;
                                    break;
                                }
                                if (Main.netMode != 1)
                                {
                                    for (int i = 0; i <= 8; i++)
                                    {
                                        Projectile.NewProjectile(null, NPC.Center, ToTarget.SafeNormalize(default).RotatedByRandom(0.8f) * 20,
                                            ModContent.ProjectileType<LostSword2>(), damage, 1.2f, Main.myPlayer, 0, 1);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 3://赤瞳
                    {
                        switch (Timer3)
                        {
                            case 0://原地上升
                                {
                                    NPC.velocity += new Vector2(0, -1);
                                    if (NPC.position.Y < Target.position.Y - 300)
                                    {
                                        damage = 130;
                                        Timer3++;
                                    }
                                    break;
                                }
                            case 1://靠近玩家
                                {
                                    if (NPC.velocity.Y < 0)
                                    {
                                        NPC.velocity.Y = 0;
                                    }

                                    if (NPC.velocity.Y < 20)
                                    {
                                        NPC.velocity.Y++;
                                    }

                                    NPC.velocity.X = ToTarget.X * 0.02f;
                                    Timer1++;
                                    if (Timer1 > 50 || NPC.Distance(Target.Center) < 20)
                                    {
                                        Timer1 = 0;
                                        Timer3++;
                                    }
                                    break;
                                }
                            case 2://发射散弹
                                {
                                    State++;
                                    Timer1 = 0;
                                    Timer3 = 0;
                                    if (Main.netMode != 1)
                                    {
                                        for (float i = 0; i < 10; i++)
                                        {
                                            Vector2 vel = (ToTarget.SafeNormalize(default) * 5).RotatedBy(i * (MathHelper.Pi / 5));
                                            Main.projectile[Projectile.NewProjectile(null, NPC.Center, vel, ModContent.ProjectileType<LostSword2>(),
                                                damage, 1.2f, Main.myPlayer, 3)].timeLeft = 1000;
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 4://和弦
                    {
                        NPC.velocity *= 0f;
                        NPC.rotation = MathHelper.ToRadians(Timer1 * 2.3f) + MathHelper.PiOver4;
                        damage = 60;
                        if (Timer1 % 10 == 0)
                        {
                            for (int i = 0; i <= 100; i++)
                            {
                                if (Main.netMode != 1)
                                {
                                    Dust dust = Dust.NewDustDirect(NPC.Center + ((i * MathHelper.TwoPi / 100).ToRotationVector2() * 300), 1, 1, DustID.FireworkFountain_Green);
                                    dust.noGravity = true;
                                }
                            }
                        }
                        if (NPC.Distance(Target.Center) > 310)
                        {
                            Target.Center = NPC.Center + ToTarget.SafeNormalize(default) * 300;
                        }
                        else if (NPC.Distance(Target.Center) > 300)
                        {
                            Target.velocity = -ToTarget.SafeNormalize(default) * 3;
                        }
                        if (Main.netMode != 1)
                        {
                            Projectile.NewProjectile(null, NPC.Center, MathHelper.ToRadians(Timer1 * 2.3f).ToRotationVector2(), ModContent.ProjectileType<LostSwordLaser2>(),
                                damage, 1.2f, Main.myPlayer, Timer1 < 30 ? 1 : 0);
                        }
                        Timer1++;
                        if (Timer1 > 180)
                        {
                            Timer1 = 0;
                            State++;
                        }
                        break;
                    }
                case 5://插地，受攻击回血
                    {
                        Timer1++;
                        if (Timer1 < 60)
                        {
                            for (int i = 0; i < 200; i++)
                            {
                                Tile tile = Main.tile[(int)(NPC.position.X / 16), (int)((NPC.position.Y / 16) + i)];
                                if (tile != null)
                                {
                                    if (tile.HasTile)
                                    {
                                        Timer2 = NPC.position.X;
                                        Timer3 = NPC.position.Y + (i * 16);
                                        Timer1++;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Timer2 > 0 && Timer3 > 0)
                            {
                                NPC.velocity = (new Vector2(Timer2, Timer3) - NPC.Center) * 0.2f;
                            }
                            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                            if (NPC.velocity.Length() < 0.1f)
                            {
                                if (Timer1 > 180)
                                {
                                    Timer1 = 0;
                                    Timer2 = 0;
                                    Timer3 = 0;
                                    State++;
                                }
                            }
                        }
                        break;
                    }
                case 7://静止不动,释放最后的招（
                    {
                        NPC.velocity *= 0;
                        damage = 60;
                        NPC.rotation = MathHelper.ToRadians(Timer1 * 1.01f) + MathHelper.PiOver4;
                        if (Timer1 % 10 == 0)
                        {
                            for (int i = 0; i <= 100; i++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Dust dust = Dust.NewDustDirect(NPC.Center + ((i * MathHelper.TwoPi / 100).ToRotationVector2() * 800), 1, 1, DustID.FireworkFountain_Green);
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
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 center = NPC.Center + (i * MathHelper.PiOver4).ToRotationVector2() * (Timer1 * 0.4f);
                            Vector2 vel = ((Timer1 * 0.01f) + (i * MathHelper.PiOver4)).ToRotationVector2();
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
            Color color = new Color(0.4f, 0.5f, 0f, 0);
            if (Timer3 == 2 || Timer3 == 5)
            {
                color = new Color(0.3f, 0.8f, 0.1f, 0.2f);
            }

            StarBreakerUtils.NPCDrawTail(NPC, drawColor, color);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 2;
            if (State == 5 && Timer1 > 0)
            {
                NPC.life += damage;
                CombatText.NewText(NPC.Hitbox, Color.Green, damage);
                if (NPC.life > NPC.lifeMax)
                {
                    NPC.life = NPC.lifeMax;
                }

                damage = 0;
            }
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "轻微氧化铜短剑";
            potionType = ItemID.SuperHealingPotion;
        }
        public override bool CheckActive()
        {
            if (Target.dead)
            {
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.type == ModContent.ProjectileType<LostSword2>())
                    {
                        projectile.Kill();
                    }
                }
                return true;
            }
            return false;
        }
        public override bool CheckDead()
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<LostSword2>())
                {
                    projectile.Kill();
                }
            }
            return true;
        }
        public override void OnKill()
        {
            NPC.NewNPC(NPC.GetSource_Death(), (int)Target.Center.X + Main.rand.Next(500, 600),
                (int)Target.Center.Y + Main.rand.Next(500, 600), ModContent.NPCType<ShortSword3>());
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
