using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.MyGraphics.RenderTargetBloom
{
    public class BloomStsyem
    {
        public delegate void BloomDraw();
        Asset<Effect> effect;
        public List<BloomDraw> DrawBlooms;
        public void Load(AssetRepository asset)
        {
            DrawBlooms = new();
            effect = asset.Request<Effect>("Effects/Content/Bloom");
        }
        public void UnLoad()
        {
            DrawBlooms = null;
            effect.Dispose();
            effect = null;
        }
        public void Draw(RenderTarget2D render1,RenderTarget2D render2,RenderTarget2D render3)
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;
            Effect bloom = effect.Value;

            #region 保存原图像
            gd.SetRenderTarget(render2);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
            sb.Draw(render1, Vector2.Zero, Color.White);
            sb.End();
            #endregion
            #region 绘制所有需要bloom绘制的东西
            gd.SetRenderTarget(render3);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            DrawBlooms.ForEach(s => s.Invoke());
            DrawBlooms.TrimExcess();//整理内存
            DrawBlooms.Clear();//清楚所有的绘制
            sb.End();
            #endregion
            #region 绘制Bloom
            gd.SetRenderTarget(render1);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);//截取亮度
            bloom.Parameters["m"].SetValue(0.75f);
            bloom.CurrentTechnique.Passes[0].Apply();
            sb.Draw(render3,Vector2.Zero,Color.White);//绘制所有图像到render1上
            sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            bloom.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            bloom.Parameters["uRange"].SetValue(2.5f);
            bloom.Parameters["uIntensity"].SetValue(0.94f);
            for (int i = 0; i < 3; i++)//交替使用两个RenderTarget2D，进行多次模糊
            {
                bloom.CurrentTechnique.Passes[2].Apply();//横向
                gd.SetRenderTarget(render3);
                gd.Clear(Color.Transparent);
                Main.spriteBatch.Draw(render1, Vector2.Zero, Color.White);

                bloom.CurrentTechnique.Passes[1].Apply();//纵向
                gd.SetRenderTarget(render1);
                gd.Clear(Color.Transparent);
                Main.spriteBatch.Draw(render3, Vector2.Zero, Color.White);
            }
            sb.End();

            gd.SetRenderTarget(render3);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive);//Additive把模糊后的部分加到render3里
            sb.Draw(render2,Vector2.Zero,Color.White);
            sb.Draw(render1, Vector2.Zero, Color.White);
            sb.End();

            gd.SetRenderTarget(render1);//切换画布为render1
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(render3, Vector2.Zero, Color.White);
            sb.End();
            #endregion
        }
    }
}
