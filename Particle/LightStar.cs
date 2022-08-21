using Terraria.Graphics.Renderers;

namespace StarBreaker.Particle
{
    public class LightStar : ABasicParticle
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
        }
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            spritebatch.Draw(_texture.Value, LocalPosition - Main.screenPosition, _frame, color, Rotation, _origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
