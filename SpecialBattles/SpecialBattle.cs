namespace StarBreaker.SpecialBattles
{
    public abstract class SpecialBattle
    {
        public bool active = false;
        public virtual Texture2D Texture => null;
        public int DrawX;
        public SpecialBattle()
        {
            active = true;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null && active)
            {
                Lighting.Mode = Terraria.Graphics.Light.LightMode.Retro;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                spriteBatch.Draw(Texture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Rectangle(DrawX, 0, Texture.Width, Texture.Height), Color.White);

                spriteBatch.End();
                spriteBatch.Begin();
            }
        }
        public virtual void Update()
        {
            DrawX += 3;
            if (DrawX > Texture.Width)
            {
                DrawX = 0;
            }
        }
    }
}
