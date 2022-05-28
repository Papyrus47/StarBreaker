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

            Vector3[] Vector3Pos = new Vector3[4]//定义需要旋转的三维向量f
            {
                    new(60,180,180),
                    new(60,180,-180),
                    new(60,-180,180),
                    new(60,-180,-180)
            };
            Matrix matrix = Matrix.CreateRotationX(Main.GlobalTimeWrappedHourly) * Matrix.CreateRotationY(-MathHelper.Pi / 24) *
                Matrix.CreateRotationZ(Projectile.rotation);

            for (int i = 0; i < 4; i++)
            {
                Vector3Pos[i] = Vector3.Transform(Vector3Pos[i], matrix);
            }
            Vector2[] pos = new Vector2[4]
            {
                    1800 / (1800 - Vector3Pos[0].Z) * new Vector2(Vector3Pos[0].X,Vector3Pos[0].Y) + Projectile.Center,
                    1800 / (1800 - Vector3Pos[1].Z) * new Vector2(Vector3Pos[1].X,Vector3Pos[1].Y)+ Projectile.Center,
                    1800 / (1800 - Vector3Pos[2].Z) * new Vector2(Vector3Pos[2].X,Vector3Pos[2].Y)+ Projectile.Center,
                    1800 / (1800 - Vector3Pos[3].Z) * new Vector2(Vector3Pos[3].X,Vector3Pos[3].Y)+ Projectile.Center
            };
            customs[0] = customs[5] = new(pos[0], Color.White, new Vector3(0, 0, 1));
            customs[1] = new(pos[2], Color.White, new Vector3(0, 1, 1));
            customs[2] = customs[3] = new(pos[3], Color.White, new Vector3(1, 1, 1));
            customs[4] = new(pos[1], Color.White, new Vector3(1, 0, 1));

            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

            StarBreaker.FrostFistHealMagic.Parameters["uTransform"].SetValue(model * projection);
            StarBreaker.FrostFistHealMagic.Parameters["uTime"].SetValue(0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap,
                DepthStencilState.Default, RasterizerState.CullNone);
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("StarBreaker/Images/Extra_34").Value;

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            StarBreaker.FrostFistHealMagic.CurrentTechnique.Passes[0].Apply();

            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                customs, 0, customs.Length / 3);

            //        triangleList.Count / 3 是三角形的个数

            Main.graphics.GraphicsDevice.RasterizerState = originalState;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
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
            Main.spriteBatch.Begin();
        }
    }
}
