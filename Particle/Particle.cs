namespace StarBreaker.Particle
{
    public class Particle
    {
        public Texture2D Texture { get; protected set; }

        public Color color;
        public Color Maincolor;

        public Vector2 position;
        public Vector2 oldPosition;
        public Vector2 velocity;

        public float rotation;
        public float scale;

        public int timeLeft;
        public int frame;
        public int frameCount = 1;
        public virtual void OnSpawn() { }

        public virtual void Update()
        {
            oldPosition = position;
            position += velocity;

            velocity *= 0.975f;
            scale *= 0.975f;

            if (--timeLeft <= 0 || scale <= 0.1f) this.Kill();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            var rect = new Rectangle((int)Main.screenPosition.X - 25, (int)Main.screenPosition.Y - 25, Main.screenWidth + 25, Main.screenHeight + 25);
            if (!rect.Contains((int)position.X, (int)position.Y)) return;

            var height = (int)(Texture.Height / frameCount);
            spriteBatch.Draw(Texture, position - Main.screenPosition, new Rectangle(0, height * frame, Texture.Width, height), color, rotation, new Vector2(Texture.Width, height) * 0.5f, scale, SpriteEffects.None, 0f);
        }

        protected virtual bool PreKill() { return true; }


        public void Kill()
        {
            if (PreKill())
            {
                StarBreakerSystem.Particles.Remove(this);
            }
        }
        public Particle(Color color, Texture2D texture, Vector2 position, Vector2? velocity = null)
        {
            this.Texture = texture;
            Maincolor = color;
            this.color = Maincolor * 0.95f;
            this.timeLeft = 60 * 3;
            this.position = position;
            this.velocity = velocity ?? Vector2.Zero;
            this.rotation = 0f;
            this.scale = 1f;
        }
    }
}
