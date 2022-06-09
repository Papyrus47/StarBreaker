namespace StarBreaker.NPCs
{
    [AutoloadBossHead]
    internal class BurnFist : ModNPC
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
        private NPC FrostFist => Main.npc[(int)NPC.localAI[3]];
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("炎拳");
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.lifeMax = 60000;
            NPC.damage = 50;
            NPC.knockBackResist = 0f;
            NPC.defense = 35;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 16;
            NPC.height = 16;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.scale = 1.8f;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/StarGloveProvingGround");
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            if (source is EntitySource_Parent parent && parent.Entity is NPC n && n.ModNPC is FrostFist)
            {
                NPC.localAI[3] = n.whoAmI;
            }
        }
        public override void AI()
        {
            if (Target.dead || !Target.active || NPC.target == 255 || NPC.target < 0)
            {
                NPC.TargetClosest();
            }
            Vector2 toTarget = Target.position - NPC.position;
            NPC.rotation = toTarget.ToRotation() + MathHelper.PiOver4;
            NPC.spriteDirection = NPC.direction; if (Target.dead)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y > 30)
                {
                    NPC.active = false;
                }

                return;
            }
            switch (State)
            {
                case 0://与炎拳的对话
                    {
                        Timer1++;
                        NPC.dontTakeDamage = true;
                        if (Timer1 % 50 == 0)
                        {
                            if (Timer1 / 50 > 6f)
                            {
                                NPC.dontTakeDamage = false;
                                Timer1 = Timer2 = 0;
                                State++;
                                FrostFist.ai[0] = FrostFist.ai[1] = 0;//控制霜拳的
                                FrostFist.ai[3]++;
                                break;
                            }
                            Timer2++;
                            if (Timer2 < 2)
                            {
                                NPC.rotation = (FrostFist.Center - NPC.Center).ToRotation() + MathHelper.PiOver4;
                            }
                            if (Timer2 < 1)
                            {
                                break;
                            }

                            string Text = Language.GetTextValue("Mods.StarBreaker.BurnFist.Boss.T" + (int)Timer2);
                            PopupText.NewText(new()
                            {
                                Text = Text,
                                Color = Color.OrangeRed,
                                DurationInFrames = 120,
                                Velocity = Vector2.UnitY * 5
                            }, NPC.Center);
                        }
                        break;
                    }
                case 1://魔法攻击
                    {
                        NPC.velocity = (NPC.velocity * 10f + toTarget.RealSafeNormalize() * 4f) / 11f;
                        Timer1++;
                        if (Timer1 > 21)
                        {
                            Timer1 = 0;
                            Timer2++;
                            if (Timer2 > 8)
                            {
                                Timer2 = 0;
                                State++;
                                break;
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 center = NPC.Center - NPC.velocity * 10f;
                                for (int i = -1; i <= 1; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), center, NPC.velocity.RotatedBy(0.1 * i) * 4f, ModContent.ProjectileType<Projs.FireFist>(),
                                        NPC.damage, 1.3f, Main.myPlayer);
                                }
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 30 * i);
                                    int dust = Dust.NewDust(center, 5, 5, DustID.Torch);
                                    Main.dust[dust].velocity = vel;
                                }
                            }
                        }
                        break;
                    }
                case 2://蓄力散射
                    {
                        Timer1++;
                        NPC.velocity = (NPC.velocity * 10f + toTarget.RealSafeNormalize()) / 11f;
                        if (Timer1 % 10 == 0)
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                Vector2 center = NPC.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 30 * i) * 50;
                                Vector2 vel = (NPC.Center - center) * 0.1f;
                                int dust = Dust.NewDust(center, 5, 5, DustID.Torch);
                                Main.dust[dust].velocity = vel;
                                Main.dust[dust].noGravity = true;
                            }
                        }

                        if (Timer1 >= 50)
                        {
                            Timer1 = 0;
                            State++;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = -8; i <= 8; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.RotatedBy(0.1 * i).RealSafeNormalize() * 14f, ModContent.ProjectileType<Projs.FireFist>(),
                                        NPC.damage, 1.3f, Main.myPlayer);
                                }
                            }

                        }
                        break;
                    }
                case 3://连续冲3次,释放火球
                    {
                        Timer1++;
                        if (Timer1 == 30)
                        {
                            NPC.velocity = toTarget.RealSafeNormalize() * 20;
                        }
                        else if (Timer1 > 30)
                        {
                            if (Timer1 % 10 == 0)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = -1; i <= 1; i += 2)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.RotatedBy(i * MathHelper.PiOver2), ModContent.ProjectileType<Projs.FireFist>(),
                                            NPC.damage, 1.3f, Main.myPlayer);
                                    }
                                }
                                NPC.velocity *= 0.96f;
                            }
                            if (Timer1 > 50)
                            {
                                Timer2++;
                                Timer1 = 0;
                                if (Timer2 > 3)
                                {
                                    Timer2 = 0;
                                    State++;
                                }
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
            if (NPC.spriteDirection == -1)
            {
                NPC.rotation += MathHelper.Pi;
            }
            else
            {
                NPC.rotation += MathHelper.PiOver2 + MathHelper.Pi;
            }
        }
        public override bool CheckActive()
        {
            return Target.dead;
        }

        public override bool CheckDead()
        {
            Main.npc[(int)NPC.localAI[0]].ai[0] = 11;
            Main.npc[(int)NPC.localAI[0]].ai[1] = 0;
            Main.npc[(int)NPC.localAI[0]].ai[2] = 0;
            Main.npc[(int)NPC.localAI[0]].ai[3] = 0;
            Main.npc[(int)NPC.localAI[0]].active = false;
            Main.npc[(int)NPC.localAI[0]].checkDead();
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
