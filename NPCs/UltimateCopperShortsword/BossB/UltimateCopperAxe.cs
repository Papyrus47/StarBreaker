using StarBreaker.Projs.UltimateCopperShortsword;

namespace StarBreaker.NPCs.UltimateCopperShortsword.BossB
{
    [AutoloadBossHead]
    public class UltimateCopperAxe : FSMNPC
    {
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate Copper Axe");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终铜斧");
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
            NPC.rotation += 1.05f;
            if (NPC.rotation > MathHelper.TwoPi)
            {
                NPC.rotation = 0;
            }
            Vector2 center = Target.position + new Vector2(400, 0);
            NPC.velocity = (NPC.velocity * 20 + (center - NPC.position).SafeNormalize(default) * 10f) / 21;
            Timer1++;
            if (Target.dead)
            {
                NPC.life = 0;
                return;
            }
            if (Timer1 > 60)
            {
                Timer1 = 0;
                Timer2++;
                if (Timer2 > 3)
                {
                    if (Timer2 > 6)
                    {
                        Timer2 = 0;
                    }
                    return;
                }
                if (Main.netMode != 1)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        Vector2 vel = (Target.position - NPC.position).SafeNormalize(default).RotatedBy(i * MathHelper.Pi / 10);
                        Projectile.NewProjectile(null, NPC.Center, vel * 8f, ModContent.ProjectileType<LostSword2>(), 80, 1.3f, Main.myPlayer, 3);
                    }
                }
            }
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
    }
}

