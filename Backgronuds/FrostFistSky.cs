using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarBreaker.NPCs;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace StarBreaker.Backgronuds
{
    public class FrostFistSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private IceThorn[] iceThorn;
        private struct IceThorn
        {
            public Vector2 Vel;
            public Vector2 Position;
            public bool Active;
            public float Scale;
            public float Alpha;
        }
        public override void OnLoad()
        {
            iceThorn = new IceThorn[50];//最多存在50个冰刺
        }
        public override void Update(GameTime gameTime)
        {
            if (StarGlobalNPC.StarFrostFist != -1)
            {
                int whoAmi = StarGlobalNPC.StarFrostFist;
                if (Main.npc[whoAmi].active)
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
            for (int i = 0; i < 50; i++)
            {
                if (!iceThorn[i].Active)
                {
                    iceThorn[i].Position = new Vector2(Main.rand.Next(Main.screenWidth), 0);
                    iceThorn[i].Vel = new Vector2(0, Main.rand.Next(3, 10));
                    iceThorn[i].Scale = Main.rand.NextFloat(1, 3);
                    iceThorn[i].Alpha = Main.rand.NextFloat(1);
                    iceThorn[i].Active = true;
                    break;
                }
                if (iceThorn[i].Position.Y > Main.screenHeight)
                {
                    iceThorn[i].Active = false;
                }
                iceThorn[i].Position += iceThorn[i].Vel;
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
            if (intensity >= 1) intensity = 1;
        }
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (Main.screenPosition.Y > 10000f || Main.gameMenu)
            {
                return;
            }
            if (maxDepth >= 0 && minDepth < 0)
            {
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("StarBreaker/Backgronuds/FrostFistSky").Value,
                    new Rectangle(Main.screenWidth, Main.screenHeight, Main.screenWidth, Main.screenHeight), null, Color.White * 0.35f * intensity, MathHelper.Pi, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            }
            if (maxDepth >= 3.4028235E+35f && minDepth < 3.4028235E+35f)
            {
                for (int i = 0; i < 50; i++)
                {
                    if (iceThorn[i].Active)
                    {
                        spriteBatch.Draw(Terraria.GameContent.TextureAssets.Projectile[ModContent.ProjectileType<Projs.IceThorn>()].Value,
                            iceThorn[i].Position, null, Color.White * iceThorn[i].Alpha, 0, Vector2.Zero, iceThorn[i].Scale, SpriteEffects.None, 0);
                    }
                }
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
