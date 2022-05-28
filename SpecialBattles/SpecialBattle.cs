namespace StarBreaker.SpecialBattles
{
    public abstract class SpecialBattle
    {
        public bool active = false;
        protected Texture2D texture = null;
        public SpecialBattle(Texture2D texture)
        {
            this.texture = texture;
            active = true;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(texture != null && active)
            {
                spriteBatch.Draw(texture,new Rectangle(0,0,Main.screenWidth,Main.screenHeight), null, Color.White);
            }
        }
        public virtual void Update()
        {

        }
    }
}
