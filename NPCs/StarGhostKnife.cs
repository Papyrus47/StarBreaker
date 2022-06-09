using StarBreaker.Projs;
using Terraria.Graphics.Effects;
using Filters = Terraria.Graphics.Effects.Filters;

namespace StarBreaker.NPCs
{
    [AutoloadBossHead]
    public class StarGhostKnife : ModNPC
    {
        private float Timer1
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        private float Timer2
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        private float Timer3
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        private int State
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        private Player Target => Main.player[NPC.target];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰鬼刀");
            //Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.lifeMax = 100000000;
            NPC.knockBackResist = 0f;
            NPC.defense = 18;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 32;
            NPC.height = 20;
            NPC.damage = 500;
            NPC.defense = 10;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/StarGhostBladeDemonSwordization");
                SceneEffectPriority = SceneEffectPriority.BossHigh;//曲子优先度
            }
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        public override void AI()
        {
            if (Target.dead || !Target.active || NPC.target == 255 || NPC.target <= 0)
            {
                NPC.TargetClosest(true);
            }
            Vector2 toTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? 1 : -1;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4 + (NPC.spriteDirection == -1 ? 0f : MathHelper.PiOver2);
            StarGlobalNPC.StarGhostKnife = NPC.whoAmI;//改变GlobalNPC的一个静态字段

            if (!SkyManager.Instance["StarBreaker:Portal"].IsActive() && (State != 0 || (State == 0 && Timer3 == 1)))//开启天空
            {
                SkyManager.Instance.Activate("StarBreaker:Portal");
            }
            #region 去世机制
            if (!Target.active || Target.dead)
            {
                NPC.active = false;
                return;
            }
            #endregion
            switch (State)
            {
                case 0://刚刚出现落地吸收能量
                    {
                        switch (Timer3)
                        {
                            case 0:
                                {
                                    NPC.noTileCollide = false;
                                    NPC.dontTakeDamage = true;
                                    Timer1 += 0.01f;
                                    NPC.velocity.Y = Timer1;
                                    NPC.velocity.X = 0;
                                    if (NPC.collideY)
                                    {
                                        Timer1 = 0;
                                        Timer3++;
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    NPC.noTileCollide = true;
                                    NPC.velocity.Y = -0.2f;
                                    NPC.velocity.X = 0;
                                    Timer1++;
                                    if (Timer1 > 60)
                                    {
                                        Timer1 = 0;
                                        Timer3 = 0;
                                        NPC.dontTakeDamage = false;
                                        State = 1;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 1://鬼影闪
                    {
                        Vector2 start = new Vector2(Target.Center.X + 500, Target.Center.Y);
                        Vector2 end = new Vector2(Target.Center.X - 500, Target.Center.Y);
                        switch (Timer3)
                        {
                            case 0:
                                {
                                    float len = (NPC.Center - start).Length();
                                    Timer1 += 0.5f;
                                    NPC.velocity = (start - NPC.Center) / Math.Max(60 - Timer1, 1);
                                    if (len < 10)
                                    {
                                        Timer1 = 0;
                                        Timer3++;
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (Timer1 == 0)
                                    {
                                        Timer1++;
                                        NPC.velocity = end - start;
                                        if (Main.netMode != 1)
                                        {
                                            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(),
                                                NPC.Center, Vector2.Zero, ModContent.ProjectileType<GhostSlash>(), 430, 2f, Main.myPlayer);
                                            GhostSlash slash = proj.ModProjectile as GhostSlash;
                                            slash.StartingPoint = start;
                                            slash.EndPoint = end;
                                        }
                                        if (!Filters.Scene["StarBreaker:GhostSlash"].Active)
                                        {
                                            // 开启滤镜
                                            Filters.Scene.Activate("StarBreaker:GhostSlash");
                                        }
                                    }
                                    else
                                    {
                                        NPC.velocity *= 0;
                                        NPC.dontTakeDamage = true;
                                        Timer1++;
                                        if (Timer1 > 150)
                                        {
                                            foreach (Projectile proj in Main.projectile)
                                            {
                                                if (proj.active && proj.type == ModContent.ProjectileType<GhostSlash>())
                                                {
                                                    proj.Kill();
                                                    break;
                                                }
                                            }
                                            Timer1 = 0;
                                            Timer3 = 0;
                                            NPC.life -= 2034500;
                                            NPC.checkDead();
                                            NPC.dontTakeDamage = false;
                                            State++;
                                        }
                                        else if (Timer1 > 75)
                                        {
                                            if (Filters.Scene["StarBreaker:GhostSlash"].Active)
                                            {
                                                //卸载滤镜
                                                Filters.Scene.Deactivate("StarBreaker:GhostSlash");
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 2://召唤鬼墓碑
                    {
                        Timer1++;
                        NPC.velocity = toTarget * 4.3f;
                        if (Timer1 >= 10 && Main.netMode != 1)
                        {
                            Vector2 center = Target.Center + new Vector2(Main.rand.Next(-100, 100), -300);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(),
                                center, Vector2.Normalize(center - Target.Center), ModContent.ProjectileType<Projs.BeamLight>(),
                                1000, 2.3f, Main.myPlayer, 0, 1);
                            Timer2++;
                            NPC.life -= 103450;
                            NPC.checkDead();
                        }
                        if (Timer2 > 20)
                        {
                            Timer1 = 0;
                            Timer2 = 0;
                            State++;
                        }
                        break;
                    }
                case 3://冰霜之萨亚
                    {
                        bool canSum = false;
                        foreach (Projectile proj in Main.projectile)
                        {
                            if (proj.active && proj.type == ModContent.ProjectileType<Projs.TheGhost.Saya>())
                            {
                                canSum = true;
                            }
                        }
                        if (Main.netMode != 1 && !canSum)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(),
                                NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projs.TheGhost.Saya>(),
                                    1000, 2.3f, Main.myPlayer, NPC.whoAmI);
                            State++;
                        }
                        else
                        {
                            State++;
                        }
                        break;
                    }
                default:
                    {
                        State = 1;
                        Timer1 = 0;
                        Timer2 = 0;
                        Timer3 = 0;
                        break;
                    }
            }
        }
        public override bool CheckActive()
        {
            return !Target.active;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ModContent.ProjectileType<Projs.BeamLight>() || projectile.type == ModContent.ProjectileType<Projs.TheGhost.Saya>())
            {
                damage = 0;
            }
        }
    }
}
