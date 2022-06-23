using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Particle
{
    public class StarParticle : Particle
    {
        public StarParticle(Color color,Vector2 position,Vector2? vel = null) : base(color,ModContent.Request<Texture2D>("StarBreaker/Particle/Star",ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, position, vel) { }
        public override void OnSpawn()
        {
            timeLeft = 150;
            rotation = Main.rand.NextFloat(6.28f);
            scale = Main.rand.NextFloat(0.1f, 0.2f);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, position + Texture.Size() * 0.5f - Main.screenPosition,null, color, rotation,Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
