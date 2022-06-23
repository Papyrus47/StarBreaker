using Terraria.Graphics.Shaders;

namespace StarBreaker.Effects
{
    public class GhostSlash : ScreenShaderData
    {
        public GhostSlash(string passName) : base(passName)
        {
        }
        public GhostSlash(Ref<Effect> shader, string passName) : base(shader, passName)
        {
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Shader.Parameters["uOpacity"].SetValue(Shader.Parameters["uOpacity"].GetValueSingle() * 2f);
        }
        public override void Apply()
        {
            base.Apply();
        }
    }
}
