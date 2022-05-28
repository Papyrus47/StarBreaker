using StarBreaker.Projs.UltimateCopperShortsword;

namespace StarBreaker.NPCs.UltimateCopperShortsword.BossB
{
    [AutoloadBossHead]
    public class UltimateCopperBow : FSMNPC
    {
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate Copper Bow");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终铜弓");
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 30000;
            NPC.defense = 2;
            NPC.damage = 95;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.width = 32;
            NPC.height = 32;
            NPC.friendly = false;
            NPC.aiStyle = -1;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.boss = true;
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
            Vector2 ToTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            NPC.rotation = ToTarget.ToRotation();
            if (Target.dead)
            {
                NPC.life = 0;
                return;
            }
            switch (State)
            {
                case 0://到玩家头顶
                    {
                        Vector2 center = Target.position + new Vector2(0, -250);
                        NPC.velocity = (NPC.velocity * 10 + (center - NPC.position).SafeNormalize(default) * 10) / 11;
                        Timer1++;
                        if (Timer1 > 100)
                        {
                            Timer1 = 0;
                            State++;
                        }
                        break;
                    }
                case 1://发射一串箭
                    {
                        NPC.velocity = ToTarget.SafeNormalize(default) * 0.3f;
                        Timer1++;
                        if (Timer1 > 20)
                        {
                            Timer2++;
                            Timer1 = 0;
                            if (Timer2 > 10)
                            {
                                Timer2 = 0;
                                State++;
                                break;
                            }
                            if (Main.netMode != 1)
                            {
                                for (int i = 1; i <= 5; i++)
                                {
                                    Vector2 vel = ToTarget.SafeNormalize(default) * i * 2.5f;
                                    Projectile projectile = Main.projectile[Projectile.NewProjectile(null, NPC.Center, vel, ModContent.ProjectileType<PatinaArrow>(),
                                        40, 2, Main.myPlayer)];
                                    projectile.friendly = false;
                                    projectile.hostile = true;
                                }
                            }
                        }
                        break;
                    }
                case 2://发射一散弹箭
                    {
                        NPC.velocity = ToTarget.SafeNormalize(default) * 0.3f;
                        Timer1++;
                        if (Timer1 > 30)
                        {
                            Timer1 = 0;
                            Timer2++;
                            if (Timer2 > 6)
                            {
                                Timer2 = 0;
                                State++;
                                return;
                            }
                            if (Main.netMode != 1)
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    Vector2 vel = (Target.position - NPC.position).SafeNormalize(default).RotatedBy(i * MathHelper.Pi / 10);
                                    Projectile projectile = Main.projectile[Projectile.NewProjectile(null, NPC.Center, vel * 8f, ModContent.ProjectileType<PatinaArrow>(),
                                        80, 2, Main.myPlayer)];
                                    projectile.friendly = false;
                                    projectile.hostile = true;
                                }
                            }
                        }
                        break;
                    }
                default:
                    State = 0;
                    break;
            }

        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void OnKill()
        {
            foreach (Player player in Main.player)
            {
                int healLife = player.statLifeMax2 - player.statLife;
                player.statLife += healLife;
                player.HealEffect(healLife);
            }
            NPC n = Main.npc[(int)NPC.localAI[3]];
            n.ai[0] = n.ai[1] = n.ai[2] = 0;
            n.ai[3]++;
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
    }
}
