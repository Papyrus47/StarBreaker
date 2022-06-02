namespace StarBreaker.SpecialBattles
{
    public abstract class SpecialBattle
    {
        public bool active = false;
        protected Texture2D texture = null;
        public int DrawX;
        public SpecialBattle(Texture2D texture)
        {
            this.texture = texture;
            active = true;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(texture != null && active)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                spriteBatch.Draw(texture,new Rectangle(0,0,Main.screenWidth,Main.screenHeight),new Rectangle(DrawX, 0,texture.Width,texture.Height), Color.White);

                spriteBatch.End();
                spriteBatch.Begin();
            }
        }
        public virtual void Update()
        {
            DrawX += 3;
            if(DrawX > texture.Width)
            {
                DrawX = 0;
            }
        }
    }
}
