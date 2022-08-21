using StarBreaker.Projs.Bosses;
using Terraria.GameContent.ItemDropRules;

namespace StarBreaker.NPCs
{
    [AutoloadBossHead]
    public class StarSpiralBladeN : FSMNPC
    {
        private Vector2 targetOldPos;
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰旋刃");
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 600000;
            NPC.knockBackResist = 0;
            NPC.defense = 12;
            NPC.damage = 90;
            NPC.noGravity = NPC.noTileCollide = true;
            NPC.width = 112;
            NPC.height = 114;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.friendly = false;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Bloodtower2");
                SceneEffectPriority = SceneEffectPriority.BossMedium;//曲子优先度
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange( //添加信息
               new IBestiaryInfoElement[]//创建一个数组
               {
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,//说明本npc为星空
                    new FlavorTextBestiaryInfoElement("他的主人在发疯的时候,他一直在身边,现在他被他的主人叫来寻找其他星辰武器,最后发现全在你身上后,不得不开始战斗")//信息
               }
               );
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapon.StarSpiralBlade>()));
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target > 200 || !Target.active || Target.dead)
            {
                NPC.TargetClosest(true);
            }
            NPC.rotation += NPC.localAI[3];
            NPC.localAI[3] += 0.1f;
            if (Target.immuneTime > 5)
            {
                Target.immuneTime = 5;
            }
            if (NPC.rotation > 31415)
            {
                NPC.rotation = 0;
            }
            if (Target.dead)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y > 80)
                {
                    NPC.active = false;
                }
                return;
            }
            if (State < 7)
            {
                if (NPC.life < NPC.lifeMax * 0.75f)
                {
                    State = 7;
                    Main.NewText(Language.GetTextValue("Mods.StarBreaker.StarSpiralBladeText.Boss.BossText.T5"), Color.Purple);
                }
            }
            else if (State < 14)
            {
                if (NPC.life < NPC.lifeMax * 0.35f)
                {
                    State = 14;
                    Main.NewText(Language.GetTextValue("Mods.StarBreaker.StarSpiralBladeText.Boss.BossText.T6"), Color.Purple);
                }
            }
            switch (State)
            {
                case 0://开幕
                    {
                        Timer1++;
                        NPC.rotation = -MathHelper.PiOver4;
                        if (Timer1 > 50)
                        {
                            Timer1 = 0;
                            string sayText = Language.GetTextValue("Mods.StarBreaker.StarSpiralBladeText.Boss.BossText.T" + (int)(Timer2 + 1));
                            Color color = Color.LightBlue;//霜拳
                            string weaponText = Language.GetTextValue("Mods.StarBreaker.StarSpiralBladeText.Boss.WeaponText.T" + (int)(Timer2 + 1));
                            if (Timer2 == 0 || Timer2 == 3)//炎拳说话颜色
                            {
                                color = Color.Orange;
                            }
                            if (Timer2 >= 4)
                            {
                                Timer2 = 0;
                                State++;
                                break;
                            }
                            Main.NewText(sayText, Color.Purple);
                            PopupText.NewText(new()
                            {
                                Color = color,
                                DurationInFrames = 120,
                                Text = weaponText,
                                Velocity = Vector2.UnitY * 3
                            }, Target.Center);
                            Timer2++;
                        }
                        break;
                    }
                case 1://记录位置冲刺
                    {
                        if (Timer1 <= 0)
                        {
                            Timer1 = 40;
                            targetOldPos = Target.Center;
                            if (Timer2 > 5)
                            {
                                Timer1 = Timer2 = 0;
                                State++;
                            }
                            Timer2++;
                        }
                        else
                        {
                            Timer1--;
                            NPC.velocity = (targetOldPos - NPC.Center) * 0.3f;//高速追击旧位置

                            if (Math.Abs(NPC.localAI[3]) > 20)//消耗旋转速度使用弹幕攻击
                            {
                                NPC.localAI[3] *= 0.4f;
                                ShootProj(Target.Center, Vector2.Zero);
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

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            //if (State <= 21 && State > 0)
            //{
            //    NPC.life = 1;
            //    return false;
            //}
            return base.CheckDead();
        }
        public override void BossHeadSlot(ref int index)
        {
            base.BossHeadSlot(ref index);
            if (State == 20 || (State == 21 && Timer3 == 1))
            {
                index = -1;
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (State == 20 || (State == 21 && Timer3 == 1))
            {
                return false;
            }
            return base.DrawHealthBar(hbPosition, ref scale, ref position);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            StarBreakerUtils.NPCDrawTail(NPC, Color.White, drawColor);
            Utils.DrawBorderString(spriteBatch, ((int)NPC.localAI[3] * 60).ToString(), NPC.Center + new Vector2(-10, -50) - Main.screenPosition, Color.MediumPurple);
            switch (State)
            {
                case 3://幻影
                    {
                        Texture2D texture = TextureAssets.Npc[Type].Value;
                        Vector2 origin = texture.Size() * 0.5f;
                        Main.spriteBatch.Draw(texture, Target.Center - (NPC.Center - Target.Center) - screenPos, null, Color.White * 0.7f, NPC.rotation,
                            origin, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texture, new Vector2(NPC.Center.X, (Target.Center - (NPC.Center - Target.Center)).Y) - screenPos, null, Color.White * 0.7f, NPC.rotation,
                            origin, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texture, new Vector2((Target.Center - (NPC.Center - Target.Center)).X, NPC.Center.Y) - screenPos, null, Color.White * 0.7f, NPC.rotation,
                            origin, 1f, SpriteEffects.None, 0);
                        break;
                    }
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
        private int ShootProj(Vector2 center, Vector2 vel)
        {
            int i = 0;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                i = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, vel, ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(),
                    Damage, 2.3f, Main.myPlayer);
            }
            return i;
        }
    }
}
