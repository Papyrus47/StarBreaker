using StarBreaker.Projs.UltimateCopperShortsword;

namespace StarBreaker.NPCs.UltimateCopperShortsword.BossB
{
    [AutoloadBossHead]
    public class UltimateCopperPick : FSMNPC
    {
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate Copper Pickaxe");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终铜稿");
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
            Vector2 ToTarget = Target.position - NPC.position;
            if (NPC.life < NPC.lifeMax * 0.1f)
            {
                if (Main.npc[(int)NPC.localAI[2]].active)
                {
                    NPC.dontTakeDamage = true;
                }
                else
                {
                    NPC.dontTakeDamage = false;
                }
            }
            if (Target.dead)
            {
                NPC.life = 0;
                return;
            }
            switch (State)
            {
                case 0://旋转发射弹幕
                    {
                        Vector2 center = Target.Center + new Vector2(-400, 0);
                        NPC.velocity = (NPC.velocity * 20 + (center - NPC.position).SafeNormalize(default) * 10f) / 21;
                        NPC.rotation += 0.85f;
                        Timer1++;
                        if (Timer1 > 60)
                        {
                            Timer2++;
                            Timer1 = 0;
                            if (Timer2 > 5)
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
                                    Projectile.NewProjectile(null, NPC.Center, vel, ModContent.ProjectileType<LostSword2>(),
                                        80, 2, Main.myPlayer, 3);
                                }
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
                            if (Timer2 > 1)
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
            NPC npc = Main.npc[(int)NPC.localAI[3]];
            npc.ai[0] = npc.ai[1] = npc.ai[2] = 0;
            npc.ai[3]++;
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
    }
}
