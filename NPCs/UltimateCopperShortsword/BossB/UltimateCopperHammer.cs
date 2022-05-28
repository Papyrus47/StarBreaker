using StarBreaker.Projs.UltimateCopperShortsword;

namespace StarBreaker.NPCs.UltimateCopperShortsword.BossB
{
    [AutoloadBossHead]
    public class UltimateCopperHammer : FSMNPC
    {
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate Copper Hammer");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终铜锤");
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 30000;
            NPC.defense = 15;
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
            NPC.rotation += 0.2f;
            if (NPC.rotation > 6.28f)
            {
                NPC.rotation -= 6.28f;
            }
            if (Target.dead)
            {
                NPC.life = 0;
                return;
            }
            switch (State)
            {
                case 0://发射弹幕
                    {
                        NPC.velocity = ToTarget.SafeNormalize(default) * 3;
                        NPC.rotation += 0.85f;
                        Timer1++;
                        if (Timer1 > 20)
                        {
                            Timer1 = 0;
                            Timer2++;
                            if (Timer2 > 10)
                            {
                                Timer2 = 0;
                                State++;
                                break;
                            }
                            if (Main.netMode != 1)
                            {
                                Projectile.NewProjectile(null, NPC.Center, ToTarget.SafeNormalize(default) * 10, ModContent.ProjectileType<LostSword2>(),
                                    80, 2, Main.myPlayer, 3);
                            }
                        }
                        break;
                    }
                case 1://冲刺
                    {
                        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                        if (Timer1 <= 0)
                        {
                            Timer1 = 40;
                            Timer2++;
                            if (Timer2 > 2)
                            {
                                Timer1 = 0;
                                Timer2 = 0;
                                State++;
                                break;
                            }
                            NPC.velocity = ToTarget.SafeNormalize(default) * 20;
                        }
                        else
                        {
                            Timer1--;
                        }

                        break;
                    }
                case 2://四边形冲刺法
                    {
                        Vector2 center;
                        switch (Timer3)
                        {
                            case 0:
                                {
                                    center = Target.position + new Vector2(300, 300);
                                    break;
                                }
                            case 1:
                                {
                                    center = Target.position + new Vector2(-300, 300);
                                    break;
                                }
                            case 2:
                                {
                                    center = Target.position + new Vector2(-300, -300);
                                    break;
                                }
                            case 3:
                                {
                                    center = Target.position + new Vector2(300, -300);
                                    break;
                                }
                            default:
                                {
                                    center = Target.position;
                                    break;
                                }
                        }
                        NPC.velocity = (center - NPC.position).SafeNormalize(default) * 10;
                        Timer1++;
                        if (Timer1 > 15)
                        {
                            Timer1 = 0;
                            if (Main.netMode != 1)
                            {
                                Projectile.NewProjectile(null, NPC.Center, NPC.velocity.SafeNormalize(default) * 10, ModContent.ProjectileType<LostSword2>(),
                                    80, 2, Main.myPlayer, 3);
                            }
                        }
                        if (NPC.Distance(center) < 30)
                        {
                            Timer3++;
                            if (Timer3 > 3)
                            {
                                Timer3 = 0;
                                State = 0;
                                Timer1 = 0;
                                Timer2 = 0;
                            }
                        }
                        break;
                    }
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
            NPC Npc = Main.npc[(int)NPC.localAI[3]];
            Npc.ai[0] = Npc.ai[1] = Npc.ai[2] = 0;
            Npc.ai[3]++;
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}