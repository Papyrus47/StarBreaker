using Microsoft.Xna.Framework;
using StarBreaker.Projs.UltimateCopperShortsword;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.NPCs.UltimateCopperShortsword.BossB
{
    [AutoloadBossHead]
    public class UltimateCopperChainSaw : FSMNPC
    {
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜链锯");
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 30000;
            NPC.defense = 2;
            NPC.damage = 95;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.width = 48;
            NPC.height = 16;
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
        public override void AI()
        {
            if (NPC.target <= 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            if (Main.npc[(int)NPC.localAI[2]].active && NPC.life < NPC.lifeMax * 0.2f)
            {
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.dontTakeDamage = false;
            }
            if (Target.dead)
            {
                NPC.life = 0;
                return;
            }
            Vector2 ToTarget = Target.position - NPC.position;
            Vector2 center = Target.position + new Vector2(-300, 0);
            NPC.rotation = ToTarget.ToRotation();//常态是一直对着速度的
            switch (State)
            {
                case 0://在玩家左边发射剑
                    {
                        NPC.velocity = (NPC.velocity * 10 + (center - NPC.position).SafeNormalize(default) * 10) / 11;
                        Timer1++;
                        if (Timer1 > 20)
                        {
                            Timer1 = 0;
                            Timer2++;
                            if (Timer2 > 20)
                            {
                                Timer2 = 0;
                                State++;
                                break;
                            }
                            if (Main.netMode != 1)
                            {
                                Projectile.NewProjectile(null, NPC.Center, ToTarget.SafeNormalize(default) * 5, ModContent.ProjectileType<LostSword2>(),
                                     90, 1.3f, Main.myPlayer, 3);
                            }
                        }
                        break;
                    }
                case 1://到点冲刺
                    {
                        switch (Timer3)
                        {
                            case 0://到点
                                {
                                    center += new Vector2(100, -200);
                                    Timer1 += 0.01f;
                                    if (NPC.Distance(center) < 10 || Timer1 > 1)
                                    {
                                        Timer3++;
                                        Timer1 = 0;
                                    }
                                    else
                                    {
                                        NPC.velocity = (NPC.velocity * (10 - Timer1) + (center - NPC.position).SafeNormalize(default) * (10 + Timer1)) / (11 - Timer1);
                                    }
                                    break;
                                }
                            case 1://冲刺
                                {
                                    NPC.velocity = ToTarget.SafeNormalize(default) * 20;
                                    Timer3++;
                                    break;
                                }
                            case 2://等待
                                {
                                    Timer1++;
                                    if (Timer1 > 30)
                                    {
                                        Timer1 = 0;
                                        State++;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    State = 0;
                    Timer3 = 0;
                    break;
            }
        }
        public override void OnKill()
        {
            Main.npc[(int)NPC.localAI[3]].ai[3]++;
            Main.npc[(int)NPC.localAI[3]].ai[2] = 0;
        }
    }
}
