using StarBreaker.Projs.CupricOxideSword;

namespace StarBreaker.NPCs.CupricOxideSword
{
    [AutoloadBossHead]
    public class CupricOxideSword : FSMNPC
    {
        private Vector2 TargetOldPos;
        public override string BossHeadTexture => base.Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cupric Oxide Sword");
            DisplayName.AddTranslation(7, "氧化铜短剑");
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new()
            {
                Rotation = -MathHelper.PiOver4
            });
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 500000;
            NPC.defense = 21;
            NPC.damage = 53;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.boss = true;
            NPC.width = NPC.height = 32;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.aiStyle = -1;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/Argalia");
            }
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("为了复仇,真的有必要把自己生锈后再氧化吗?")
            });
        }
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target > 200 || !Target.active || Target.dead)
            {
                NPC.TargetClosest();
            }
            if (Target.dead)
            {
                NPC.velocity.Y++;
                if (NPC.timeLeft == 0)//我就怕什么不脱战
                {
                    NPC.active = false;
                }
                return;
            }
            NPC.timeLeft = 300;
            switch (State)
            {
                case 0://氧化铜短剑开幕
                    {
                        NPC.rotation = -MathHelper.PiOver4;
                        NPC.dontTakeDamage = true;
                        Timer1++;
                        if (Timer1 % 50 == 0)
                        {
                            string Text;
                            switch ((int)Timer1 / 50)
                            {
                                case 1: Text = "看来我拥有了自我意识啊"; break;

                                case 2: Text = "那么准备好了?"; break;
                                case 3: Text = "这会是我最后的复仇"; break;
                                default:
                                    {
                                        Text = "开始吧";
                                        Timer1 = 0;
                                        State++;
                                        NPC.dontTakeDamage = false;
                                        break;
                                    }
                            }
                            Main.NewText(Text, Color.Black);
                        }
                        break;
                    }
                case 1://轮回冲刺
                    {
                        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                        Timer1--;
                        if (Timer1 <= 0)
                        {
                            Timer1 = 40;//重置计时器
                            NPC.velocity = (Target.Center - NPC.Center).RealSafeNormalize() * 25;
                        }
                        else//这才是冲刺部分
                        {
                            if (Timer1 % 5 == 0)
                            {
                                _ = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity / 5, ModContent.ProjectileType<CupricOxide>(),
                                    Damage, 1.3f, Main.myPlayer);
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
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
        }
        public override bool CheckActive()
        {
            return !Target.active;
        }
    }
}
