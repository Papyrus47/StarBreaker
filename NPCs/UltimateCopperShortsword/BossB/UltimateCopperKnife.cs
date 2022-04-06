using Microsoft.Xna.Framework;
using StarBreaker.Projs.UltimateCopperShortsword;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.NPCs.UltimateCopperShortsword.BossB
{
    [AutoloadBossHead]
    public class UltimateCopperKnife : FSMNPC
    {
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜投刀");
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 35000;
            NPC.defense = 3;
            NPC.damage = 40;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.width = 10;
            NPC.height = 24;
            NPC.friendly = false;
            NPC.aiStyle = -1;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.HitSound = SoundID.NPCHit4;
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
            Vector2 ToTarget = Target.position - NPC.position;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.PiOver2;//常态是一直对着玩家的
            if (Target.dead)
            {
                NPC.life = 0;
                return;
            }
            switch (State)
            {
                case 0://朝玩家投掷飞刀
                    {
                        NPC.velocity = ToTarget.SafeNormalize(default) * 5;
                        Timer1++;
                        if (Timer1 > 10)
                        {
                            Timer2++;
                            Timer1 = 0;
                            if (Timer2 > 10)
                            {
                                Timer2 = 0;
                                State++;
                            }
                            else
                            {
                                if (Main.netMode != 1)
                                {
                                    Projectile.NewProjectile(null, NPC.Center, NPC.velocity * 2, ModContent.ProjectileType<CopperKnife>(),
                                        60, 1.2f, Main.myPlayer);
                                }
                            }
                        }
                        break;
                    }
                case 1://朝天上散发大量飞刀
                    {
                        NPC.velocity *= 0.99f;
                        if (NPC.velocity.Length() < 0.5f)
                        {
                            Timer1++;
                            if (Timer1 > 30)
                            {
                                Timer1 = 0;
                                Timer2++;
                                if (Timer2 > 1)
                                {
                                    Timer2 = 0;
                                    State++;
                                }
                                else
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        for (int i = -8; i <= 8; i++)
                                        {
                                            Vector2 vector = new Vector2(i, -3);
                                            Projectile.NewProjectile(null, NPC.Center, vector, ModContent.ProjectileType<CopperKnife>(),
                                                50, 1.3f, Main.myPlayer);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 2://向玩家冲刺,散发弹幕
                    {
                        switch (Timer3)
                        {
                            case 0://蓄力
                                {
                                    Timer1++;
                                    for (float i = 0; i <= 10; i++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.Firefly);
                                            dust.velocity = (i * (MathHelper.TwoPi / 10)).ToRotationVector2() * 5;
                                        }
                                    }
                                    if (Timer1 > 100)
                                    {
                                        Timer1 = 0;
                                        Timer3++;
                                    }
                                    break;
                                }
                            case 1://冲刺
                                {
                                    if (Timer1 <= 0)
                                    {
                                        Timer1 = 60;
                                        Timer2++;
                                        NPC.velocity = ToTarget.SafeNormalize(default) * 10f;
                                        if (Timer2 > 1)
                                        {
                                            Timer3 = 0;
                                            Timer2 = 0;
                                            Timer1 = 0;
                                            State++;
                                        }
                                        if (Main.netMode != 1)
                                        {
                                            for (int i = -2; i <= 2; i++)
                                            {
                                                Projectile.NewProjectile(null, NPC.Center, ToTarget.SafeNormalize(default).RotatedBy(i * 0.1f) * 5,
                                                    ModContent.ProjectileType<CopperKnife>(), 60, 1.3f, Main.myPlayer);
                                            }
                                        }
                                    }
                                    else Timer1--;
                                    break;
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
