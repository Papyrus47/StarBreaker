using StarBreaker.Projs.UltimateCopperShortsword;

namespace StarBreaker.NPCs.UltimateCopperShortsword.BossB
{
    [AutoloadBossHead]
    public class UltimateCopperSpear : FSMNPC
    {
        private List<Vector2> targetOldPos = new List<Vector2>();
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜矛");
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 30000;
            NPC.defense = 15;
            NPC.damage = 95;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.Size = new Vector2(54, 54);
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
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.target <= 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Vector2 ToTarget = Target.Center - NPC.Center;
            if (Target.dead)
            {
                NPC.life = 0;
                return;
            }
            NPC.velocity *= 0;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.PiOver2 + MathHelper.PiOver4;
            if (NPC.soundDelay <= 0)
            {
                NPC.soundDelay = 10;
                for (int i = 0; i <= 100; i++)
                {
                    if (Main.netMode != 1)
                    {
                        Dust dust = Dust.NewDustDirect(NPC.Center + ((i * MathHelper.TwoPi / 100).ToRotationVector2() * 800), 1, 1, DustID.FireworkFountain_Green);
                        dust.noGravity = true;
                    }
                }
            }
            if (NPC.Distance(Target.Center) > 810)
            {
                Target.Center = NPC.Center + ToTarget.SafeNormalize(default) * 400;
            }
            else if (NPC.Distance(Target.Center) > 800)
            {
                Target.velocity = -ToTarget.SafeNormalize(default) * 3;
            }
            switch (State)
            {
                case 0://散发投矛
                    {
                        Timer1++;
                        if (Timer1 > 3)
                        {
                            Timer1 = 0;
                            Timer2++;
                            if (Timer2 > 5)
                            {
                                Timer2 = 0;
                                State++;
                            }
                            if (Main.netMode != 1)
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    int proj = Projectile.NewProjectile(null, NPC.Center, ToTarget.SafeNormalize(default).RotatedBy(i * 0.1f) * 10, ModContent.ProjectileType<FlySpearProj>(),
                                        10, 1.2f, Main.myPlayer, 1);
                                    Main.projectile[proj].friendly = false;
                                    Main.projectile[proj].hostile = true;
                                }
                            }
                        }
                        break;
                    }
                case 1://向玩家头上急速投矛
                    {
                        Timer1++;
                        Vector2 ToHead = Target.Center + new Vector2(0, -400) - NPC.Center;
                        NPC.rotation = ToHead.ToRotation() + MathHelper.PiOver2 + MathHelper.PiOver4;
                        if (Timer1 > 20)
                        {
                            Timer1 = 18;
                            Timer2++;
                            if (Timer2 > 10)
                            {
                                Timer1 = 0;
                                Timer2 = 0;
                                State++;
                            }
                            if (Main.netMode != 1)
                            {
                                int proj = Projectile.NewProjectile(null, NPC.Center, ToHead.SafeNormalize(default) * 10, ModContent.ProjectileType<FlySpearProj>(),
                                    10, 1.2f, Main.myPlayer);
                                Main.projectile[proj].friendly = false;
                                Main.projectile[proj].hostile = true;
                            }
                        }
                        break;
                    }
                case 2://记录10个旧位置,对着旧位置发射
                    {
                        Timer1++;
                        NPC.rotation = Main.GlobalTimeWrappedHourly * 10;
                        if (Timer1 > 20)
                        {
                            Timer1 = 0;
                            Timer2++;
                            if (Timer2 < 10)
                            {
                                if (targetOldPos != null)
                                {
                                    targetOldPos.Add(Target.position);
                                }
                            }
                            else
                            {
                                if (Timer2 > 21)
                                {
                                    Timer2 = 0;
                                    State++;
                                    if (targetOldPos != null)
                                    {
                                        targetOldPos.Clear();
                                    }
                                    break;
                                }
                                else
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        for (int i = 0; i < targetOldPos.Count; i++)
                                        {
                                            Vector2 ToOldPos = targetOldPos[i] - NPC.position;
                                            int proj = Projectile.NewProjectile(null, NPC.Center, ToOldPos.RealSafeNormalize() * 10, ModContent.ProjectileType<FlySpearProj>(),
                                                10, 1.2f, Main.myPlayer, 1);
                                            Main.projectile[proj].friendly = false;
                                            Main.projectile[proj].hostile = true;
                                        }
                                        Timer1 = 15;
                                    }
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
        public override void OnKill()
        {
            foreach (Player player in Main.player)
            {
                int healLife = player.statLifeMax2 - player.statLife;
                player.statLife += healLife;
                player.HealEffect(healLife);
            }
            NPC npc = Main.npc[(int)NPC.localAI[3]];
            npc.ai[0] = npc.ai[1] = npc.ai[2] = 0;
            npc.ai[3]++;
        }
    }
}
