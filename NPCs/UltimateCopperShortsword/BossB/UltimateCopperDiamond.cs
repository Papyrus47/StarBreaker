using Microsoft.Xna.Framework;
using StarBreaker.Projs.UltimateCopperShortsword;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.NPCs.UltimateCopperShortsword.BossB
{
    [AutoloadBossHead]
    public class UltimateCopperDiamond : FSMNPC
    {
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜钻");
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
        public override void AI()
        {
            if (NPC.target <= 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Vector2 ToTarget = Target.position - NPC.position;
            Vector2 center = Target.position + new Vector2(300, 0);
            NPC.rotation = ToTarget.ToRotation() + MathHelper.PiOver2; if (Target.dead)
            {
                NPC.life = 0;
                return;
            }
            switch (State)
            {
                case 0://老老实实对玩家发射
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
                                Projectile projectile = Main.projectile[Projectile.NewProjectile(null, NPC.Center, ToTarget.SafeNormalize(default) * 5, ModContent.ProjectileType<CopperDrillBit>(),
                                    90, 1.3f, Main.myPlayer)];
                                projectile.friendly = false;
                                projectile.hostile = true;
                            }
                        }
                        break;
                    }
                case 1://到斜下方,发射散弹
                    {
                        center += new Vector2(-100, 100);
                        NPC.velocity = (NPC.velocity * 10 + (center - NPC.position).SafeNormalize(default) * 10) / 11;
                        Timer1++;
                        if (Timer1 > 50)
                        {
                            Timer1 = 0;
                            Timer2++;
                            if (Timer2 > 3)
                            {
                                Timer2 = 0;
                                State++;
                                break;
                            }
                            if (Main.netMode != 1)
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    Projectile projectile = Main.projectile[Projectile.NewProjectile(null, NPC.Center, ToTarget.SafeNormalize(default).RotatedBy(i * MathHelper.Pi / 10) * 5, ModContent.ProjectileType<CopperDrillBit>(),
                                        90, 1.3f, Main.myPlayer)];
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
    }
}
