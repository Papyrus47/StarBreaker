namespace StarBreaker.Projs.StarDoomStaff.Boss
{
    public class EliminateRays_Boss : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("消除射线");
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 5;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0] = 2000f;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + Projectile.velocity.RealSafeNormalize() * Projectile.ai[0],
                50, ref r);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawMagic();
            DrawRays();
            return false;
        }
        private void DrawMagic()
        {
            CustomVertexInfo[] customs = new CustomVertexInfo[6];
            Vector3[] pos_Vec3 = new Vector3[5];//在你的脑海里面想想这个五角星的位置

            //旋转X轴,控制点的旋转
            //旋转Y轴,使图像像屏幕倾斜
            //旋转Z轴,使图像旋转
            Matrix matrix = Matrix.CreateRotationX(Projectile.timeLeft * 0.1f) *
                Matrix.CreateRotationY(MathHelper.Pi * -0.1f) *
                Matrix.CreateRotationZ(Projectile.rotation);
            for (int i = 0; i < 5; i++)
            {
                Vector2 Vec3Pos = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 5f * i) * 150f;
                pos_Vec3[i] = new(100, Vec3Pos.X, Vec3Pos.Y);
                pos_Vec3[i] = Vector3.Transform(pos_Vec3[i], matrix);
            }
            #region 手动为5角星的顶点赋值

            customs[0] = customs[5] = new(pos_Vec3[0].Vector3ProjectionToVectoer2(1800f) + Projectile.Center - Main.screenPosition, Color.Purple, new Vector3(0.5f, 0.5f, 0f));
            customs[1] = new(pos_Vec3[2].Vector3ProjectionToVectoer2(1800f) + Projectile.Center - Main.screenPosition, Color.Purple, new Vector3(0.5f, 0.5f, 0f));
            customs[2] = new(pos_Vec3[4].Vector3ProjectionToVectoer2(1800f) + Projectile.Center - Main.screenPosition, Color.Purple, new Vector3(0.5f, 0.5f, 0f));
            customs[3] = new(pos_Vec3[1].Vector3ProjectionToVectoer2(1800f) + Projectile.Center - Main.screenPosition, Color.Purple, new Vector3(0.5f, 0.5f, 0f));
            customs[4] = new(pos_Vec3[3].Vector3ProjectionToVectoer2(1800f) + Projectile.Center - Main.screenPosition, Color.Purple, new Vector3(0.5f, 0.5f, 0f));

            #endregion
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.Default, RasterizerState.CullNone);

            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.FishingLine.Value;

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip,
                customs, 0, customs.Length - 1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.Default, RasterizerState.CullNone);
        }
        private void DrawRays()
        {
            CustomVertexInfo[] customs = new CustomVertexInfo[6];

            Vector2 vel = Projectile.velocity.RealSafeNormalize();
            vel = new Vector2(vel.Y, -vel.X);
            const float dis = 50;
            customs[0] = new(Projectile.Center + (vel * 100 * Projectile.scale) + (Projectile.velocity.RealSafeNormalize() * dis), Color.Purple, new Vector3(0, 0, 0f));
            customs[1] = customs[3] = new(Projectile.Center + (vel * -100 * Projectile.scale) + (Projectile.velocity.RealSafeNormalize() * dis), Color.Purple, new Vector3(0, 1, 0f));
            customs[2] = customs[5] = new(Projectile.Center + (vel * 100 * Projectile.scale) + (Projectile.velocity.RealSafeNormalize() * Projectile.ai[0]), Color.Purple, new Vector3(1, 0, 1f));
            customs[4] = new(Projectile.Center + (vel * -100 * Projectile.scale) + (Projectile.velocity.RealSafeNormalize() * Projectile.ai[0]), Color.Purple, new Vector3(1, 1, 1f));

            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

            StarBreaker.EliminateRaysShader.Parameters["uTransform"].SetValue(model * projection);
            StarBreaker.EliminateRaysShader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp,
                DepthStencilState.Default, RasterizerState.CullNone);
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

            Main.graphics.GraphicsDevice.Textures[0] = Terraria.GameContent.TextureAssets.Projectile[Type].Value;

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            StarBreaker.EliminateRaysShader.CurrentTechnique.Passes[0].Apply();

            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                customs, 0, customs.Length / 3);

            //        triangleList.Count / 3 是三角形的个数

            Main.graphics.GraphicsDevice.RasterizerState = originalState;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.Default, RasterizerState.CullNone);
        }
    }
}
