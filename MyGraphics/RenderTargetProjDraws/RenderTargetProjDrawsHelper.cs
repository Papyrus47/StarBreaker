namespace StarBreaker.MyGraphics.RenderTargetProjDraws
{
    /// <summary>
    /// 使用RT2D绘制弹幕的工具箱
    /// </summary>
    internal class RenderTargetProjDrawsHelper
    {
        public Dictionary<int, IRenderTargetProjDraws> RenderTargetProjDraws = new();
        public void AddDrawProj(int whoAmI, IRenderTargetProjDraws item)
        {
            RenderTargetProjDraws.Add(whoAmI, item);
        }

        public void UpdateDraw(int whoAmI)
        {
            if (StarBreakerUtils.InBegin())
            {
                Main.spriteBatch.End();
            }

            if (RenderTargetProjDraws.TryGetValue(whoAmI, out var renderTargetProjDraws)) //绘制对应弹幕的特效
            {
                if (renderTargetProjDraws.CanDraw(whoAmI))
                {
                    renderTargetProjDraws.Draw(whoAmI);
                }
                Check_RenderTargetProjDraws_Remove(renderTargetProjDraws, whoAmI);
            }

            if (!StarBreakerUtils.InBegin())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                    SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        /// <summary>
        /// 检查时候可以移除,能的话则直接移除
        /// </summary>
        /// <param name="renderTarget"></param>
        /// <param name="whoAmI"></param>
        public void Check_RenderTargetProjDraws_Remove(IRenderTargetProjDraws renderTarget, int whoAmI)
        {
            if (renderTarget.Remove(whoAmI))
            {
                RenderTargetProjDraws.Remove(whoAmI);
            }
        }
    }
}
