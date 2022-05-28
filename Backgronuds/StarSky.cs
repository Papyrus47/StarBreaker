using StarBreaker.NPCs;
using Terraria.Graphics.Effects;

namespace StarBreaker.Backgronuds
{
    public class StarSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        public override void Update(GameTime gameTime)
        {
            if (StarGlobalNPC.StarBreaker != -1)
            {
                int whoAmi = StarGlobalNPC.StarBreaker;
                if (Main.npc[whoAmi].active && (Main.npc[whoAmi].type == ModContent.NPCType<StarBreakerN>() || Main.npc[whoAmi].type == ModContent.NPCType<StarBreakerEX>()))
                {
                    if (intensity < 1f)
                    {
                        intensity += 0.03f;
                    }
                }
                else
                {
                    intensity -= 0.03f;
                    if (intensity < 0f)
                    {
                        intensity = 0f;
                        Deactivate();
                    }
                }
            }
            else
            {
                intensity -= 0.03f;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    Deactivate();
                }
            }
        }
        public override void Reset()
        {
            isActive = false;
            intensity -= 0.03f;
        }
        public override bool IsActive()
        {
            if (Main.gameMenu)
            {
                intensity -= 0.03f;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    Deactivate();
                }
            }
            return isActive;
        }
        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
            if (intensity >= 1)
            {
                intensity = 1;
            }
        }
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("StarBreaker/Backgronuds/StarSky").Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * intensity);

            }
        }
        public override void Deactivate(params object[] args)
        {
            if (intensity <= 0)
            {
                isActive = false;
                StarGlobalNPC.StarBreaker = -1;
            }
        }
        public override Color OnTileColor(Color inColor)
        {
            return new Color(Vector4.Lerp(new Vector4(0.6f, 0.9f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }
        public override float GetCloudAlpha()
        {
            return 1f - intensity;
        }
    }
}
