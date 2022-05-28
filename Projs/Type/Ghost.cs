namespace StarBreaker.Projs.Type
{
    public abstract class Ghost : ModProjectile
    {
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public sealed override void SetDefaults()
        {
            SetDef();
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.hide = false;
        }
        public sealed override void AI()
        {
            Alive();
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            Projectile.active = npc.active;
        }
        public override void PostDraw(Color lightColor)
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            Projectile.Center = npc.Center;
            List<CustomVertexInfo> customs = new List<CustomVertexInfo>();
            for (float r = 0; r <= MathHelper.TwoPi + 1; r += MathHelper.Pi / 100)
            {
                Vector2 center = r.ToRotationVector2() * 1000 + npc.Center;
                customs.Add(new CustomVertexInfo(center, LineColor, new Vector3(0, 0, 1)));
            }
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

            StarBreaker.FrostFistHealMagic.Parameters["uTransform"].SetValue(model * projection);
            //StarBreaker.TheDrawEffect.Parameters["uTime"].SetValue((float)Projectile.timeLeft * 0.04f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap,
                DepthStencilState.Default, RasterizerState.CullNone);

            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
            Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>(TheColorTex).Value;

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;

            StarBreaker.FrostFistHealMagic.CurrentTechnique.Passes[0].Apply();

            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip,
                customs.ToArray(), 0, customs.Count - 1);
            //EnW.DefaultEffect.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.RasterizerState = originalState;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
        }
        public NPC MyOwenr;
        protected Color LineColor;
        protected string TheColorTex;
        public abstract void SetDef();
        public abstract void Alive();


    }
}
