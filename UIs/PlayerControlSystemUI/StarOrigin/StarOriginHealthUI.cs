using StarBreaker.Content.ControlPlayerSystem;
using StarBreaker.UIs.PlayerControlSystemUI.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace StarBreaker.UIs.PlayerControlSystemUI.StarOrigin
{
    public class StarOriginHealthUI : UIElement
    {
        public Asset<Texture2D> ElementBarTex;
        public Asset<Texture2D> Bar;
        public Asset<Texture2D> ElementsTex;
        public Asset<Texture2D> OriginBar;
        public byte NowElementsID;
        public HealthUIHelper healthUIHelper;
        public override void OnInitialize()
        {
            base.OnInitialize();
            string name = GetType().Namespace.Replace('.', '/');
            ElementBarTex = ModContent.Request<Texture2D>(name + "/ElementBar");
            Bar = ModContent.Request<Texture2D>(name + "/Bar");
            ElementsTex = ModContent.Request<Texture2D>(name + "/Elements");
            OriginBar = ModContent.Request<Texture2D>(name + "/OriginBar");
            Top.Set(100, 0f);
            Left.Set(100, 0f);
            healthUIHelper = new();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!BasicControlPlayerSystem.ReleaseChangeAuxiliary && BasicControlPlayerSystem.ControlChangeAuxiliary) NowElementsID++;
            if (NowElementsID > 4) NowElementsID = 0;
            if (StarBreakerSystem.playerSystem is StarOriginControlSystem system) system.ElementID = NowElementsID;
            healthUIHelper.Update(Main.LocalPlayer.statLife,Main.LocalPlayer.statMana);
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            byte alpha = healthUIHelper.Alpha;
            if(alpha <= 15)return;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            Color White = new Color(255,255,255) * (alpha / 255f);
            White.A = 255;
            Color lineColor_Red = new Color(255, 50, 50) * (alpha / 255f);
            Color lineColor_Blue = new Color(50, 50, 255) * (alpha / 255f);
            var dimension = GetDimensions();
            Vector2 DrawPos = dimension.Center();
            Vector2 pos = DrawPos;
            Texture2D elemnetBarTex = ElementBarTex.Value;
            pos.X += 25;

            DrawOriginBar(spriteBatch, OriginBar.Value, pos, White, lineColor_Red, Main.LocalPlayer.statLife, 100);
            DrawExtraBar(spriteBatch, TextureAssets.BlackTile.Value, Bar.Value, pos + new Vector2(76, 0), White, lineColor_Red, Main.LocalPlayer.statLife, Main.LocalPlayer.statLifeMax2, 100);
            pos.Y += 20;

            DrawOriginBar(spriteBatch, OriginBar.Value, pos, White, lineColor_Blue, Main.LocalPlayer.statMana, 50);
            DrawExtraBar(spriteBatch, TextureAssets.BlackTile.Value, Bar.Value, pos + new Vector2(76, 0), White, lineColor_Blue, Main.LocalPlayer.statMana, Main.LocalPlayer.statManaMax2, 50);

            #region ECh绘制
            Content.ControlPlayerSystem.StarOriginControlSystem starOrigin = StarBreakerSystem.playerSystem as Content.ControlPlayerSystem.StarOriginControlSystem;
            DrawOriginBar(spriteBatch, null, pos, White, lineColor_Blue, starOrigin.ECh_Now, 50);
            DrawExtraBar(spriteBatch, TextureAssets.BlackTile.Value, Bar.Value, pos + new Vector2(76, 0), White, lineColor_Blue, starOrigin.ECh_Now, starOrigin.ECh_Max, 50);
            #endregion
            spriteBatch.Draw(elemnetBarTex, DrawPos, null, White, 0, elemnetBarTex.Size() * 0.5f, 2f, SpriteEffects.None, 0);
            Rectangle sourceRectangle = new(0, (int)(81 / 5f) * NowElementsID, 16, (int)(81 / 5f));
            spriteBatch.Draw(ElementsTex.Value, DrawPos + new Vector2(7,7), sourceRectangle, White, 0, sourceRectangle.Size() * 0.5f, 2f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
        }
        private static void DrawOriginBar(SpriteBatch spriteBatch, Texture2D texture, Vector2 Pos, Color white,Color LineColor, float NowBarValue, float MaxBarValue)
        {
            NowBarValue = MathHelper.Clamp(NowBarValue, 0, MaxBarValue);
            if (texture != null) spriteBatch.Draw(texture, Pos, null, white, 0f, new Vector2(0, texture.Height * 0.5f), new Vector2(2f, 2.5f), SpriteEffects.None, 0f); // 绘制原血条
            Vector2 lineOffest = new(2, 0.15f);
            Rectangle rectangle = new(2, 4, (int)(NowBarValue / MaxBarValue * 34f), 2);
            spriteBatch.Draw(TextureAssets.BlackTile.Value, Pos + lineOffest, rectangle, LineColor, 0f, new(0, rectangle.Height * 0.5f), new Vector2(2f, 2.7f), SpriteEffects.None, 0f);
        }
        private static void DrawExtraBar(SpriteBatch spriteBatch, Texture2D barline, Texture2D texture, Vector2 Pos, Color white, Color LineColor, float NowBarValue, float MaxBarValue, float Process)
        {
            Vector2 lineOffest = new(0, 0.15f);
            NowBarValue -= Process;
            Vector2 pos = Pos;
            float width = 1;
            if (texture != null)
            {
                for (float i = 0; i < MaxBarValue - Process; i += Process) // 循环绘制条
                {
                    spriteBatch.Draw(texture, pos, null, white, 0f, new(0, texture.Height * 0.5f), new Vector2(2f, 2.5f), SpriteEffects.None, 0f);
                    pos.X += 12;
                    width += 5.8f;
                }
            }
            if (NowBarValue <= 0) return;
            Rectangle rectangle = new(0, 0, (int)(NowBarValue / (MaxBarValue - Process) * width), 2);
            //spriteBatch.Draw(texture, pos, new Rectangle(0,0, (int)(1f / (MaxBarValue - Process) * width * 12f),8), Color.White, 0f, new(0, texture.Height * 0.5f), new Vector2(2f, 2.5f), SpriteEffects.None, 0f);
            spriteBatch.Draw(barline, Pos + lineOffest, rectangle, LineColor, 0f, new(0, rectangle.Height * 0.5f), new Vector2(2f, 2.7f), SpriteEffects.None, 0f);

        }
    }
}
