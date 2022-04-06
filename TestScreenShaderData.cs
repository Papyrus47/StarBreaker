using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace StarBreaker
{
    public class TestScreenShaderData : ScreenShaderData
    {
        public TestScreenShaderData(string passName) : base(passName)
        {
        }
        public TestScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
        {
        }
        public override void Apply()
        {
        }
    }
}
