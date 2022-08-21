using Terraria.Graphics.Renderers;

namespace StarBreaker.Particle
{
    public class TheBall : ABasicParticle
    {
        public Color color;
        public int TimeLeft;
        public override void Update(ref ParticleRendererSettings settings)
        {
            base.Update(ref settings);
            if (--TimeLeft <= 0 || Scale.X <= 0 || Scale.Y <= 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
            if(TimeLeft < 40)
            {
                Velocity *= 0.9f;
            }
        }
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            color.A = 0;
            spritebatch.Draw(_texture.Value, LocalPosition - Main.screenPosition, _frame, color, Rotation, _origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
