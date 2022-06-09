using Terraria.ID;

namespace StarBreaker.Projs.Type
{
    public abstract class BaseMeleeItemProj : ModProjectile
    {
        public Player Player => Main.player[Projectile.owner];
        public Vector2[] oldVels;
        /// <summary>
        /// 控制挥舞的玩意
        /// </summary>
        protected float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public Color LerpColor, LerpColor2;
        /// <summary>
        /// 顶点绘制向量取长,和伤害判定
        /// </summary>
        public float DrawLength;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Projectile.velocity * DrawLength, Projectile.width / 2, ref Projectile.localAI[1]);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            if (CanDraw())
            {
                Vector2 scale = Projectile.velocity.AbsVector2() / texture.Size() + Vector2.One;

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White,
                    Projectile.rotation, new Vector2(5, texture.Height - 10), scale, 0, 0);

                #region 顶点绘制
                bool flag = !Main.drawToScreen && Main.netMode != NetmodeID.Server && !Main.gameMenu && !Main.mapFullscreen && Lighting.NotRetro && Terraria.Graphics.Effects.Filters.Scene.CanCapture();
                if (!flag)
                {
                    DrawVectrx(Projectile, oldVels, LerpColor, LerpColor2, DrawLength);
                }
                #endregion
            }
            return false;
        }
        public static void DrawVectrx(Projectile projectile, Vector2[] oldVels, Color LerpColor, Color LerpColor2, float DrawLength)
        {
            List<CustomVertexInfo> bars = new();
            for (int i = 1; i < oldVels.Length; i++)
            {
                if (oldVels[i] == Vector2.Zero)
                {
                    continue;
                }

                Vector2 vel = oldVels[i - 1] - oldVels[i];
                vel = vel.NormalVector();
                var factor = i / (float)oldVels.Length;
                var color = Color.Lerp(LerpColor, LerpColor2, factor);
                var w = MathHelper.Lerp(0.5f, 0.05f, factor);
                bars.Add(new(projectile.Center + (oldVels[i] + vel.RealSafeNormalize()) * DrawLength, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
                bars.Add(new(projectile.Center + (oldVels[i] + vel.RealSafeNormalize()).RealSafeNormalize() * 5f, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
            }
            if (bars.Count > 2)
            {
                List<CustomVertexInfo> triangleList = new();
                for (int i = 0; i < bars.Count - 2; i += 2)//取三角形
                {
                    triangleList.Add(bars[i]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 1]);

                    triangleList.Add(bars[i + 1]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 3]);
                }

                if (StarBreakerWay.InBegin())
                {
                    Main.spriteBatch.End();
                }

                if (!StarBreakerWay.InBegin())
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                }

                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

                #region 这一段是你们需要根据自己的mod用于切换的顶点绘制shader
                Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("StarBreaker/Images/MyExtra_2").Value;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                StarBreaker.UseSwordShader.Parameters["uTransform"].SetValue(model * projection);
                StarBreaker.UseSwordShader.CurrentTechnique.Passes[0].Apply();
                #endregion
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
            if (StarBreakerWay.InBegin())
            {
                Main.spriteBatch.End();
            }

            if (!StarBreakerWay.InBegin())
            {
                Main.spriteBatch.Begin();
            }
        }
        public virtual bool CanDraw()//可以绘制特效
        {
            return true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Projectile.timeLeft = 5;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction,
                Projectile.velocity.X * Projectile.direction);
            player.ChangeDir(Projectile.direction);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);

            for (int i = oldVels.Length - 1; i > 0; i--)
            {
                oldVels[i] = oldVels[i - 1];
            }
            oldVels[0] = Projectile.velocity;
        }
        /// <summary>
        /// 使用挥动ai
        /// </summary>
        /// <param name="Length">椭圆的最大长度</param>
        /// <param name="X">X轴缩放</param>
        /// <param name="Y">Y轴缩放</param>
        /// <param name="RotBy">椭圆的旋转,目的是为了对着鼠标</param>
        /// <param name="rot">椭圆旋转初始角度</param>
        protected void UseAI(float Length, float X, float Y, float RotBy, float rot = 0f)
        {
            Vector2 pos = Projectile.Center + (Timer - rot).ToRotationVector2() * Length;
            Projectile.velocity = pos - Projectile.Center;
            Projectile.velocity.Y *= Y;
            Projectile.velocity.X *= X;
            Projectile.velocity = Projectile.velocity.RotatedBy(RotBy);
        }
        /// <summary>
        /// 挥舞速度的改变
        /// </summary>
        /// <param name="player"></param>
        /// <param name="RotSpeed">旋转速度</param>
        /// <returns>这一次旋转应该旋转的弧度</returns>
        protected static float UseSpeed(Player player, float RotSpeed = 0.2f)
        {
            return (float)Math.Sin(RotSpeed) * (player.GetTotalAttackSpeed(DamageClass.Melee) + player.GetWeaponAttackSpeed(player.HeldItem) / 60);
        }

        /// <summary>
        /// 给UseAI用的
        /// </summary>
        /// <param name="player"></param>
        /// <returns>挥舞时,固定旋转的弧度</returns>
        protected float UseRot(Player player)
        {
            float r = 0;
            if (Main.myPlayer == player.whoAmI)
            {
                r = (Main.MouseWorld - Projectile.Center).ToRotation() + Main.rand.NextFloatDirection() * 0.5f;
            }
            return r;
        }
    }
}
