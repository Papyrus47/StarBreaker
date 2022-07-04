using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;

namespace StarBreaker.NPCs
{
    [AutoloadBossHead]
    internal class FrostFist : ModNPC
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
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("霜拳");
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
            NPC.defense = 30;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 16;
            NPC.height = 16;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.scale = 2;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/StarGloveProvingGround");
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
            NPC.spriteDirection = NPC.direction;
            StarGlobalNPC.StarFrostFist = NPC.whoAmI;
            if (!SkyManager.Instance["StarBreaker:FrostFistSky"].IsActive())//开启天空
            {
                SkyManager.Instance.Activate("StarBreaker:FrostFistSky");
            }
            if (Target.dead)
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
                        Target.GetModPlayer<StarPlayer>().FrostFistModScr = NPC.whoAmI;
                        Timer1++;
                        NPC.dontTakeDamage = true;
                        if (Timer1 % 50 == 0)
                        {
                            if (Timer1 / 50 > 6f)
                            {
                                NPC.dontTakeDamage = false;
                                break;
                            }
                            Timer2++;
                            if (Timer2 == 1)
                            {
                                NPC.rotation = 0;
                            }
                            string Text = Language.GetTextValue("Mods.StarBreaker.FrostFist.Boss.T" + (int)Timer2);
                            PopupText.NewText(new()
                            {
                                Text = Text,
                                Color = Color.LightSkyBlue,
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
                        if (Timer1 > 7)
                        {
                            Timer1 = 0;
                            Timer2++;
                            if (Timer2 > 20)
                            {
                                Timer2 = 0;
                                State++;
                                break;
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 center = NPC.Center - NPC.velocity * 10f;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), center, NPC.velocity.RotatedByRandom(0.02) * 4f, ModContent.ProjectileType<Projs.IceThorn>(),
                                    NPC.damage, 1.3f, Main.myPlayer);
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 30 * i);
                                    int dust = Dust.NewDust(center, 5, 5, DustID.Ice);
                                    Main.dust[dust].velocity = vel;
                                }
                            }
                        }
                        break;
                    }
                case 2://蓄力冲刺
                    {
                        Timer1++;

                        if (Timer1 == 50)
                        {
                            NPC.velocity = toTarget.RealSafeNormalize() * 20;
                        }
                        else if (Timer1 > 50)
                        {
                            if (Timer1 % 8 == 0)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = -1; i <= 1; i += 2)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.RotatedBy(i * MathHelper.PiOver2), ModContent.ProjectileType<Projs.IceThorn>(),
                                            NPC.damage, 1.3f, Main.myPlayer);
                                    }
                                }
                                NPC.velocity *= 0.96f;
                            }
                            if (Timer1 > 90)
                            {
                                Timer1 = 0;
                                State++;
                            }
                        }
                        else
                        {
                            NPC.velocity = (NPC.velocity * 10f + toTarget.RealSafeNormalize()) / 11f;
                            if (Timer1 % 10 == 0)
                            {
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 center = NPC.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 30 * i) * 50;
                                    Vector2 vel = (NPC.Center - center) * 0.1f;
                                    int dust = Dust.NewDust(center, 5, 5, DustID.Ice);
                                    Main.dust[dust].velocity = vel;
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                        }
                        break;
                    }
                case 3://到玩家头上,落下,然后发射弹幕
                    {
                        Timer1++;
                        if (Timer1 < 50)
                        {
                            NPC.alpha += (int)(255f / 50f);
                        }
                        else if (Timer1 == 50)
                        {
                            NPC.alpha = 0;
                            NPC.Center = Target.Center + new Vector2(0, -300);
                            NPC.velocity = Vector2.UnitY * 10;
                        }
                        else if (Timer1 > 90)
                        {
                            Timer1 = 0;
                            State++;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 15; i++)
                                {
                                    Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 15 * i) * 15;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<Projs.IceThorn>(),
                                        NPC.damage, 1.5f, Main.myPlayer);
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
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapon.FrostFistW>(), 1));
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapon.BurnFistW>(), 1));
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int n = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 300, (int)NPC.Center.Y, ModContent.NPCType<BurnFist>(), NPC.whoAmI);
                Main.npc[n].ai[1] = -1;
            }
        }
        public override void OnKill()
        {
            StarBreakerSystem.downedStarFist = true;
        }
        public override bool CheckDead()
        {
            return true;
        }
        public override bool CheckActive()
        {
            return Target.dead;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
