using StarBreaker.Content;
using Steamworks;
using static StarBreaker.StarBreakerAssetHelper;

namespace StarBreaker.Items.StarOwner.StarsPierceWeapon
{
    public class StarsPierceProj : SkillProj
    {
        public override string Texture => GetType().Namespace.ToString().Replace('.', '/') + "/StarsPierce";
        public override void Init()
        {
            AddSkill(nameof(NotUse), new(NotUse));
            AddSkill(nameof(Stabbing), new(Stabbing, Stabbing_Draw));
            AddSkill(nameof(Stabbing_Channel) + "G", new(() => Stabbing_Channel(false), Stabbing_Draw));
            AddSkill(nameof(Stabbing_Channel) + "S", new(() => Stabbing_Channel(true), Stabbing_Draw));
            AddSkill(nameof(FlySky), new(FlySky, Slash_Draw, SlashUpAndDown_OnHit));
            AddSkill(nameof(SlashDown) + nameof(FlySky), new(FlySky, Slash_Draw, SlashUpAndDown_OnHit));
            AddSkill(nameof(SlashDown), new(SlashDown, Slash_Draw, SlashUpAndDown_OnHit));
            AddSkill(nameof(MoveForward_Spurts), new(MoveForward_Spurts, MoveForward_Spurts_Draw, null, MoveForward_Spurts_MHitNPC));
            AddSkill(nameof(Stabbing_Continuity), new(Stabbing_Continuity, Stabbing_Continuity_Draw, null,Stabbing_Continuity_ModHit));
            AddSkill(nameof(MoveBackward_StepBack), new(MoveBackward_StepBack));
        }
        public override void Init_SkillChange()
        {
            SkillProj_SkillInstance stabbing = GetSkill(nameof(Stabbing));
            SkillProj_SkillInstance stb_channelG = GetSkill(nameof(Stabbing_Channel) + "G");
            SkillProj_SkillInstance stb_channelS = GetSkill(nameof(Stabbing_Channel) + "S");
            SkillProj_SkillInstance stb_cont = GetSkill(nameof(Stabbing_Continuity));
            SkillProj_SkillInstance flySky = GetSkill(nameof(FlySky));
            SkillProj_SkillInstance slashDown = GetSkill(nameof(SlashDown));
            SkillProj_SkillInstance slash_fk = GetSkill(nameof(SlashDown) + nameof(FlySky));

            Func<bool> func = () => !starBreakerPlayer.InAir;
            #region 地面出招表
            stabbing.Wait(stb_channelG, func); // Combo1
            stabbing.To(flySky, func).To(slashDown).To(stb_channelG); // Combo2
            stabbing.To(flySky, func).Wait(stb_cont); // Combo3
            stabbing.To(flySky, func).To(slashDown).Wait(slash_fk).To(stb_cont); // Combo4
            #endregion

            #region 空中出招表
            func = () => starBreakerPlayer.InAir;
            stabbing.To(stb_channelS, func);
            stabbing.Wait(flySky, func);
            #endregion
        }
        private Player player;
        private StarBreakerPlayer starBreakerPlayer;
        private bool CanHit;
        public List<NPC> HitTargets;
        public bool IsLeftMouseOrRightMouse => starBreakerPlayer.LeftMouse;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("繁星刺破");
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.Size = new(104, 102);
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }
        public override void PreSkillAI()
        {
            player ??= Main.player[Projectile.owner];
            if (player.dead || !player.active || player.HeldItem?.ModItem is not StarsPierce)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            starBreakerPlayer ??= player.StarBreaker();
            ControlAttack = IsLeftMouseOrRightMouse && !starBreakerPlayer.InAttack;
            Control_NoCombokill();
        }
        public override void PostSkillAI()
        {
            starBreakerPlayer.InAttack = WeaponInAttack;
        }
        public void Control_NoCombokill() // 在AI执行前操控非Combo的Skills
        {
            if (!starBreakerPlayer.InAir && IsLeftMouseOrRightMouse)
            {
                if (starBreakerPlayer.MoveDir == -1) // 反方向
                {
                    WeaponInAttack = true;
                    ChangeSkill(nameof(MoveBackward_StepBack));
                    Projectile.velocity.X = Projectile.direction = -player.direction;
                }
                else if (starBreakerPlayer.MoveDir == 1) // 同方向
                {
                    WeaponInAttack = true;
                    ChangeSkill(nameof(MoveForward_Spurts));
                }
            }

            if (starBreakerPlayer.ContactGround || starBreakerPlayer.LeaveGround) //接触地面/离开地面时
            {
                ChangeSkill(nameof(NotUse), true);
            }
        }
        public override void PostModifyHitNPC()
        {
            object obj = nameof(StarsPierceProj) + State.ToString() + oldSkillsID.Count.ToString();
            starBreakerPlayer.AddAttack(obj);
        }
        public override void OnChangeSkill()
        {
            starBreakerPlayer.Pre_LeftMouse = false;
            starBreakerPlayer.Pre_RightMouse = false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (CanHit) // 近战
            {
                float r = Projectile.Size.Length();
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                    Projectile.Center - Projectile.velocity.SafeNormalize(default) * r / 2,
                    Projectile.Center + Projectile.velocity.SafeNormalize(default) * r / 2,
                    Projectile.height, ref r);
            }
            return false;
        }
        #region 不使用时的AI
        public void NotUse()
        {
            player.ChangeDir((player.Center.X < Main.MouseWorld.X).ToDirectionInt()); // 改变玩家朝向
            Projectile.Center = player.Center;
            Projectile.velocity *= 0;
            Timer = 0;
            player.channel = false;
            Projectile.hide = true;
            CanHit = false;
            WeaponInAttack = false;

            if (IsLeftMouseOrRightMouse)
            {
                ChangeSkill(nameof(Stabbing));
                ControlAttack = false;
            }
        }
        #endregion
        #region ComboAI
        public void Stabbing_Attack(int variables = 0) // 刺击调用这个
        {
            if (Timer == variables)
            {
                Projectile.velocity = (Main.MouseWorld - player.Center).SafeNormalize(default);
            }
            starBreakerPlayer.DamageFactor = 1f;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * (Projectile.width / 2 - 14) * (Timer - variables);
            Projectile.Center -= Projectile.velocity;
            Projectile.position.Y += player.gfxOffY;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }
        #region 快速刺击
        /// <summary>
        /// Combo系列全部调用,Air Combo同理
        /// </summary>
        public void Stabbing()
        {
            player.heldProj = Projectile.whoAmI;
            WeaponInAttack = true;
            if (Timer > 5)
            {
                WeaponInAttack = false;
                CanHit = false;
                if (Timer > 25)
                {
                    ChangeSkill(nameof(NotUse), true);
                }
                else if (IsLeftMouseOrRightMouse)
                {
                    WaitControl = Timer > 10;
                    Timer = 0;
                }
                FixedProj();
            }
            else // 攻击中
            {
                if (Timer == 3)
                {
                    var sound = SoundID.Item1 with
                    {
                        MaxInstances = 5,
                        Pitch = 0.8f,
                        PitchVariance = 0
                    };
                    for (int i = 0; i < 5; i++)
                    {
                        SoundEngine.PlaySound(sound, player.Center);
                    }
                }
                Stabbing_Attack();
                player.velocity.X += Projectile.velocity.X;
                CanHit = true;
                player.ChangeDir(Projectile.direction); // 改变玩家朝向
                Projectile.hide = false;
            }
            Timer++;
        }
        public void Stabbing_Change()
        {
            bool flag = Timer > 20; // 等待时间
            //if ((flag && !starBreakerPlayer.InAir) || (!flag && starBreakerPlayer.InAir)) // 在空中等待时间长
            //{
            //    ChangeSkill(nameof(Stabbing_Channel)); // 通往Combo1或Air Combo1
            //}
            //else
            //{
            //    ChangeSkill(nameof(FlySky)); // 通往Combo2和Combo3和Combo4,或Air Combo2
            //}
            WaitControl = flag;
        }
        #endregion
        #region 蓄力刺击
        /// <summary>
        /// 蓄力,Combo1有用,Air Combo1有用
        /// </summary>
        public void Stabbing_Channel(bool inAir)
        {
            Timer++;
            WeaponInAttack = true;
            player.heldProj = Projectile.whoAmI;
            starBreakerPlayer.DamageFactor = 0.8f;
            Projectile.velocity = (Main.MouseWorld - player.Center).SafeNormalize(default);
            #region 固定弹幕
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity.RealSafeNormalize() * (Projectile.width / 2 - 14);
            Projectile.position -= Projectile.velocity;
            Projectile.position.Y -= player.gfxOffY;
            #endregion
            bool flag = oldSkillsID.Count > 2 && oldSkillsID[^2].Equals(nameof(SlashDown)); // Combo2判断
            bool flag2 = oldSkillsID[0].Equals(nameof(MoveBackward_StepBack));
            if (flag2 && Timer < 15)
            {
                Timer = 15;
            }
            if (Timer >= 20)
            {
                CanHit = true;
                if (Timer < 25) // 屏幕震动+攻击
                {
                    Stabbing_Attack(20);
                    if ((int)Timer == 20) //生成粒子和声音
                    {
                        var sound = SoundID.Item1 with
                        {
                            MaxInstances = 5,
                            Pitch = 0.2f,
                            PitchVariance = 0
                        };
                        for (int i = 0; i < 5; i++)
                        {
                            SoundEngine.PlaySound(sound, player.Center);
                        }
                        for (int i = 0; i < 30; i++)
                        {
                            Dust dust = Dust.NewDustDirect(player.Center, 10, 10, DustID.PurpleTorch);
                            dust.velocity = -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(6, 10);
                            dust.noGravity = true;
                            dust.scale *= 4f;
                        }
                    }
                    Main.instance.CameraModifiers.Add(
                        new Terraria.Graphics.CameraModifiers.PunchCameraModifier(player.Center, Projectile.velocity * (Projectile.damage / (player.GetWeaponDamage(player.HeldItem) * 2f)), 5, 5, 3));//屏幕震动
                }
                else if (Timer > 35) // 跳出,因为所有AI中都是最后一击
                {
                    ChangeSkill(nameof(NotUse), true);
                    Projectile.damage = player.GetWeaponDamage(player.HeldItem);
                }
                else // 刺完了,不能攻击到敌人
                {
                    CanHit = false;
                    WaitControl = true;
                }
            }
            else if (player.channel || flag || flag2)
            {
                player.itemTime = player.itemAnimation = 2;
                player.ChangeDir((player.Center.X < Main.MouseWorld.X).ToDirectionInt()); // 改变玩家朝向
                Timer -= 0.5f;
                Projectile.damage++;

                if (inAir) // 在空中,生成向下的粒子
                {
                    Projectile.damage++;
                    starBreakerPlayer.StopVel = true;
                    Dust dust = Dust.NewDustDirect(player.Center, 5, 5, DustID.PurpleTorch);
                    dust.velocity.Y = 4;
                    dust.velocity.X = Main.rand.NextFloatDirection() * 1.5f;
                    dust.scale *= 1.5f;
                }
            }
        }
        #endregion
        #region 连续刺击
        /// <summary>
        /// 最tm麻烦的:连续刺击
        /// </summary>
        public void Stabbing_Continuity()
        {
            bool isNorm = !oldSkillsID[^2].Equals(nameof(MoveBackward_StepBack));
            if (Timer < 2)
            {
                if (isNorm)
                {
                    Vector2 vel = Main.MouseWorld - player.Center;
                    Projectile.localAI[0] = vel.ToRotation();
                    Projectile.direction = vel.X > 0 ? 1 : -1;
                    player.ChangeDir(Projectile.direction);
                }
                else
                {
                    Vector2 vel = Main.MouseWorld - player.Center;
                    Projectile.direction = vel.X > 0 ? 1 : -1;
                    Projectile.velocity.X = Projectile.direction;
                    Projectile.velocity.Y = 0;
                    Projectile.localAI[0] = Projectile.direction == 1 ? 0 : MathHelper.Pi;
                    player.ChangeDir(Projectile.direction);
                }
            }
            player.itemTime = player.itemAnimation = 2;
            player.heldProj = Projectile.whoAmI;
            Projectile.velocity = Projectile.localAI[0].ToRotationVector2(); // 就这样不动速度
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * (Projectile.width / 2 + Timer % 3 * 7 + 14f);
            Projectile.position.Y += player.gfxOffY;
            Projectile.rotation = Projectile.localAI[0] + MathHelper.PiOver4;
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction);

            Timer++;
            WeaponInAttack = true;
            CanHit = true;

            if (isNorm) // 当不属于后撤步的时候
            {
                starBreakerPlayer.DamageFactor = 0.16f;
                Stabbing_Continuity_NormalState();
            }
            else
            {
                starBreakerPlayer.DamageFactor = 0.07f;
                Stabbing_Continuity_FormMoveBack();
            }

            if (Timer > 60) // 刺一秒
            {
                CanHit = false;
                Projectile.localAI[0] = Projectile.localAI[1] = 0;
                Projectile.frameCounter = 0;
                Projectile.damage = player.GetWeaponDamage(player.HeldItem);
                ChangeSkill(isNorm ? nameof(NotUse) : nameof(Stabbing_Channel), isNorm); // 这个特判好一些
                Timer = 0;
            }
        }

        public void Stabbing_Continuity_NormalState()
        {
            if (starBreakerPlayer.LeftMouse) // 玩家按下了鼠标!
            {
                Timer -= 4;
                if (Timer < 2)
                {
                    Timer = 2;
                }

                Projectile.localAI[1]++; // 计数!
                if (Projectile.localAI[1] > 20) // 衔接Crazy Combo
                {
                    Projectile.localAI[0] = Projectile.localAI[1] = 0;
                    ChangeSkill(nameof(NotUse), true);
                }
            }
        }
        public void Stabbing_Continuity_FormMoveBack()
        {
            if (player.channel && Projectile.frameCounter == 0) // 没错,偷懒的极致
            {
                if (Timer > 2)
                {
                    Timer--;
                }

                Projectile.position.X -= Projectile.velocity.X * 20;
                CanHit = false;
                if (Projectile.localAI[1] < 10)
                {
                    Projectile.localAI[1]++; // 另一个计数器
                    Projectile.damage += 2;
                }
            }
            else
            {
                if (Projectile.frameCounter < 10)
                {
                    if (Timer > 2)
                    {
                        Timer--;
                    }

                    if (Projectile.frameCounter == 9)
                    {
                        Projectile.localAI[1] = 0; // 重置这个计时器,用于计算左键次数
                        player.velocity.X *= 0f;
                    }
                    Projectile.position.X -= Projectile.velocity.X * 20;
                    Projectile.frameCounter++; // 拒绝切换回去
                    player.velocity.X += player.direction * 5; // 前冲
                }
                else
                {
                    Stabbing_Continuity_NormalState(); // 也是要用到这个的,因为要接crazy combo
                }
            }
        }
        public void Stabbing_Continuity_ModHit(NPC target,ref NPC.HitModifiers hit)
        {
            hit.FinalDamage *= 0.3f;
        }
        #endregion
        #region 上挑和下砍
        /// <summary>
        /// 上挑动作
        /// </summary>
        public void FlySky()
        {
            SlashUpAndDown_AI(-Vector2.UnitX, nameof(FlySky), player.direction);
        }

        /// <summary>
        /// 下砍
        /// </summary>
        public void SlashDown()
        {
            SlashUpAndDown_AI(-Vector2.UnitY, nameof(SlashDown), -player.direction);
        }

        private void FixedProj()
        {
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity.RealSafeNormalize() * (Projectile.width / 2 - 14);
            Projectile.position -= Projectile.velocity;
            Projectile.position.Y -= player.gfxOffY;
            player.itemTime = player.itemAnimation = 2;
        }
        /// <summary>
        /// 上挑和下砍的施加加速度
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        /// <param name="kn"></param>
        /// <param name="crit"></param>
        public void SlashUpAndDown_OnHit(NPC target,NPC.HitInfo hitInfo, int damageDone)
        {
            if (hitInfo.Knockback != 0)
            {
                target.velocity.Y += -Projectile.velocity.Y * 15;
            }
        }

        /// <summary>
        /// 上劈和下砍的Ai
        /// </summary>
        /// <param name="startVel">初始向量</param>
        /// <param name="ID">使用的函数字符串ID</param>
        /// <param name="slashDir">砍向方向</param>
        public void SlashUpAndDown_AI(Vector2 startVel, string ID, int slashDir)
        {
            CanHit = true;
            starBreakerPlayer.DamageFactor = 1f;
            player.heldProj = Projectile.whoAmI;
            WeaponInAttack = true;
            Vector2 vel; // 速度
            bool isSlashDown = ID.Equals(nameof(SlashDown));
            int dir = isSlashDown ? 1 : player.direction;

            vel = dir * startVel.RotatedBy(MathHelper.ToRadians(slashDir * (Timer - 10) * -10));
            #region 动作代码
            if (Timer > 30) // 动作结束
            {
                Projectile.extraUpdates = 0;
                Projectile.velocity = (Projectile.velocity * 15f + vel * 0.2f) / 16f; // 渐变速度,说明在减速
                //SlashUpAndDown_Change(ID); // 使用这个函数控制切换
                CanHit = false;
                WeaponInAttack = false;
                WaitControl = false;
                if (Timer > 45)
                {
                    ChangeSkill(nameof(NotUse)); // 未进行操作
                }
                else if (Timer > 35)
                {
                    WaitControl = true; // 进行等待攻击的判定
                }
            }
            else if (Timer < 10)
            {
                Projectile.velocity = (Projectile.velocity * 5 + dir * startVel) / 6;
            }
            else // 动作还在进行
            {
                if (Timer == 20)
                {
                    var sound = SoundID.Item1 with
                    {
                        MaxInstances = 5,
                        Pitch = 0.3f,
                        PitchVariance = 0
                    };
                    for (int i = 0; i < 5; i++)
                    {
                        SoundEngine.PlaySound(sound, player.Center);
                    }
                }
                starBreakerPlayer.PreControlAction = true; // 可以进行预操作
                Projectile.velocity = vel;
                Projectile.extraUpdates = 1;
                if (!isSlashDown) //上劈
                {
                    if (starBreakerPlayer.InAir)
                    {
                        player.velocity.Y -= 1f; // 在空中
                    }

                    player.velocity.X += Projectile.velocity.X * 0.5f;
                }
            }
            #endregion
            Timer++; // 计时器
            FixedProj();
        }
        #endregion
        #region Combo的绘制
        /// <summary>
        /// 这是刺击绘制
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool Stabbing_Draw(Color color)
        {
            if (Timer < 5)
            {
                return true;
            }

            SpriteBatch spriteBatch = Main.spriteBatch;
            int lenght = Projectile.oldPos.Length;
            for (int i = 0; i < lenght; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f * Projectile.scale - Main.screenPosition;
                spriteBatch.Draw(TextureAssets.Projectile[Type].Value, drawPos, null, color * (i / (float)lenght) * 0.4f, Projectile.oldRot[i],
                    Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        /// <summary>
        /// 这是挥舞绘制
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool Slash_Draw(Color color)
        {
            if (Timer > 35 || Timer < 10)
            {
                return true; // 这里是配合AI的绘制
            }

            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            List<CustomVertexInfo> customs = new();
            float ProjSize = Projectile.Size.Length() * 0.8f;
            float oldRotLength = Projectile.oldRot.Length;
            //bool flag = InSkill(nameof(SlashDown));
            for (int i = 0; i < oldRotLength; i++)
            {
                Vector2 vector2 = (Projectile.oldRot[i] - MathHelper.PiOver4).ToRotationVector2();
                Vector2 pos = player.Center + vector2 * ProjSize;
                float factor = i / oldRotLength;
                customs.Add(new(player.Center + vector2 * -50, color, new Vector3(factor, 0f, 0.8f)));
                customs.Add(new(pos, color, new Vector3(factor, 1f, 1f)));
            }
            if (customs.Count > 10)
            {
                List<CustomVertexInfo> triangleList = StarBreakerUtils.GenerateTriangle(customs);
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                gd.Textures[0] = SB_Extra[0].Value;
                gd.Textures[1] = StarsPierceProjColor.Value;
                gd.SamplerStates[0] = SamplerState.PointWrap;
                Effect effect = StarsPierceProjEffect.Value;

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                effect.Parameters["uTransform"].SetValue(model * projection);
                effect.Parameters["m"].SetValue(0.05f);
                effect.Parameters["n"].SetValue(0.02f);
                color = Color.MediumPurple * 0.5f;
                color.A = 100;
                effect.Parameters["uColor"].SetValue(color.ToVector4());
                effect.CurrentTechnique.Passes[0].Apply();
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                    Main.Rasterizer, null, Main.Transform);
            }
            return true;
        }
        public bool Stabbing_Continuity_Draw(Color color)
        {
            if (oldSkillsID[^2].Equals(nameof(MoveBackward_StepBack)) && Projectile.localAI[1] > 0 && Projectile.frameCounter < 10)
            {
                return true; //这段期间是蓄力,静止百万突刺的绘制
                //不知道要不要用,所以就先空着
            }
            SpriteBatch sb = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D ballTex = SB_Extra[1].Value;
            const float drawCount = 5;
            //sb.End();
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, Main.Rasterizer,
            //    null, Main.Transform);
            Color ballColor = new(255, 100, 243, 0);
            ballColor *= 0.1f;
            for (int i = 0; i <= drawCount; i++)
            {
                float factor = i / drawCount;
                Vector2 pos = player.Center + Projectile.velocity.RotatedByRandom(0.3) * Main.rand.Next(60, 100);
                pos -= Main.screenPosition;
                color.A = (byte)(factor * 255);
                sb.Draw(ballTex, pos, null, ballColor, Projectile.rotation - MathHelper.PiOver4, ballTex.Size() * 0.5f, new Vector2(2.3f, 0.8f), SpriteEffects.None, 0f);
                sb.Draw(texture, pos, null, color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            }
            //sb.End();
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, Main.Rasterizer,
            //    null, Main.Transform);
            return true;
        }
        #endregion
        #endregion
        #region 非ComboAI
        #region 咿呀前冲
        public void MoveForward_Spurts()
        {
            Timer++;
            CanHit = true;
            player.velocity.X = player.direction * 35;
            Projectile.damage = player.GetWeaponDamage(player.HeldItem) * 3;
            Projectile.hide = false;
            Projectile.velocity.Y = 0; // 前冲,又不是斜着的 
            Projectile.velocity.X = player.velocity.X; // 对着玩家前冲的方向
            FixedProj(); // 固定弹幕
            player.itemTime = player.itemAnimation = 2;
            player.immune = true;
            player.immuneTime = 5;
            starBreakerPlayer.DamageFactor = 0.125f;
            player.immuneAlpha = 100;
            player.heldProj = Projectile.whoAmI;
            WeaponInAttack = true;
            if (Timer > 30)
            {
                Projectile.damage = player.GetWeaponDamage(player.HeldItem);
                player.velocity.X *= 0.1f; // 突然减速
                Main.instance.CameraModifiers.Add(
                    new Terraria.Graphics.CameraModifiers.PunchCameraModifier(player.Center, Vector2.One, 5f, 10f, 5));
                ChangeSkill(nameof(NotUse), true);
            }
        }
        public bool MoveForward_Spurts_Draw(Color color)
        {
            if (Timer < 5)
            {
                return true;
            }

            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            Texture2D texture = TextureAssets.Extra[193].Value;
            List<CustomVertexInfo> vertices = new(), vertexInfos = new();
            const float lenght = 30; // 遍历时候的长度
            Vector2 startPos = Projectile.Center + Projectile.velocity.RealSafeNormalize() * Projectile.width * 1.4f * Projectile.scale;
            Vector2 endPos = Projectile.Center - Projectile.velocity * Timer * 0.5f;
            for (int i = 1; i < lenght; i++)
            {
                float factor = i / lenght;
                Vector2 pos = startPos + (endPos - startPos) * factor /*- Main.screenPosition*/; // 获取屏幕上位置
                pos.Y += 7f;
                float width = MathF.Sin(factor * MathHelper.Pi) * 10f;

                vertices.Add(new(pos + Vector2.UnitY * width, color, new Vector3(factor + Timer * 0.05f, 0, 0.7f)));
                vertices.Add(new(pos - Vector2.UnitY * width, color, new Vector3(factor + Timer * 0.05f, 1, 1f)));

                vertexInfos.Add(new(pos + Vector2.UnitY * width * 1.25f, color, new Vector3(factor + Timer * 0.03f, 1, 1f)));
                vertexInfos.Add(new(pos - Vector2.UnitY * width * 1.25f, color, new Vector3(factor + Timer * 0.03f, 0, 0.4f)));
            }
            if (vertices.Count > 2)
            {
                List<CustomVertexInfo> triangleList = StarBreakerUtils.GenerateTriangle(vertices);
                triangleList.AddRange(StarBreakerUtils.GenerateTriangle(vertexInfos));
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                gd.Textures[0] = texture;
                gd.Textures[1] = StarsPierceProjColor1.Value;
                gd.SamplerStates[0] = SamplerState.PointWrap;
                Effect effect = StarsPierceProjEffect.Value;

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                effect.Parameters["uTransform"].SetValue(model * projection);
                effect.Parameters["m"].SetValue(0.55f);
                effect.Parameters["n"].SetValue(0.23f);

                color = new Color(25, 23, 43);
                color.A = 150;
                effect.Parameters["uColor"].SetValue(color.ToVector4());
                effect.CurrentTechnique.Passes[0].Apply();
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                    Main.Rasterizer, null, Main.Transform);
            }
            return true;
        }
        public void MoveForward_Spurts_MHitNPC(NPC target,ref NPC.HitModifiers hitModifiers)
        {
            hitModifiers.Knockback *= 5;
            hitModifiers.HitDirectionOverride = player.direction;
        }
        #endregion
        #region 后撤步
        public void MoveBackward_StepBack()
        {
            Timer++;
            CanHit = false;
            if (Timer < 8)
            {
                player.velocity.X = Projectile.velocity.X * 5;
                Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            }
            else
            {
                Timer = 0;
                ChangeSkill(nameof(Stabbing_Continuity));
                Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            }
        }
        #endregion
        #endregion
    }
}
