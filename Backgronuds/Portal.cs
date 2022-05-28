using StarBreaker.NPCs;
using Terraria.Graphics.Effects;

namespace StarBreaker.Backgronuds
{
    public class Portal : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        public override void Update(GameTime gameTime)
        {
            if (StarGlobalNPC.StarGhostKnife != -1)
            {
                int whoAmi = StarGlobalNPC.StarGhostKnife;
                if (Main.npc[whoAmi].active)
                {
                    if (intensity < 1f)
                    {
                        intensity += 0.01f;
                    }
                }
                else
                {
                    intensity -= 0.01f;
                    if (intensity < 0f)
                    {
                        intensity = 0f;
                        Deactivate();
                    }
                }
            }
            else
            {
                intensity -= 0.01f;
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
            intensity -= 0.01f;
        }
        public override bool IsActive()
        {
            if (Main.gameMenu)
            {
                intensity -= 0.01f;
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
                Main.spriteBatch.Draw(
                    TextureAssets.BlackTile.Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                     Color.Black * intensity * 0.4f);

                Texture2D tex = ModContent.Request<Texture2D>("StarBreaker/Backgronuds/Portal").Value;
                Main.spriteBatch.Draw(
                    tex,
                    new Vector2(Main.screenWidth / 2, Main.screenHeight / 2),
                    null,
                    new Color(200, 0, 255, 0),
                    Main.GlobalTimeWrappedHourly,
                    tex.Size() / 2,
                    intensity + 1f,
                    SpriteEffects.None,
                    0f);

            }
            if (maxDepth >= 3.4028235E+38f && minDepth < 3.4028235E+38f)
            {
                Texture2D tex = ModContent.Request<Texture2D>("StarBreaker/Backgronuds/LightB").Value;
                Main.spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * intensity);
            }
        }
        public override void Deactivate(params object[] args)
        {
            if (intensity <= 0)
            {
                isActive = false;
                StarGlobalNPC.StarGhostKnife = -1;
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
