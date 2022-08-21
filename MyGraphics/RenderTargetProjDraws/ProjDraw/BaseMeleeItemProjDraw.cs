using StarBreaker.Projs.Type;
using static StarBreaker.StarBreaker;

namespace StarBreaker.MyGraphics.RenderTargetProjDraws.ProjDraw
{
    internal class BaseMeleeItemProjDraw : RenderTargetProjDraws
    {
        public override bool CanDraw(int projWhoAmI)
        {
            return Main.projectile[projWhoAmI].ModProjectile is Projs.Type.BaseMeleeItemProj;
        }

        public override void Draw(int projwhoAmI)
        {
            Projectile Projectile = Main.projectile[projwhoAmI];
            BaseMeleeItemProj proj = Projectile.ModProjectile as BaseMeleeItemProj;
            if (!proj.CanDraw())
            {
                return;
            }
            gd.SetRenderTarget(render);
            gd.Clear(Color.Transparent);
            if (!StarBreakerUtils.InBegin())
            {
                sb.Begin();
            }
            BaseMeleeItemProj.DrawVectrx(Projectile, proj.oldVels, proj.LerpColor, proj.LerpColor2, proj.DrawLength);

            gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            OffsetShader.CurrentTechnique.Passes[0].Apply();
            OffsetShader.Parameters["tex0"].SetValue(render);//render可以当成贴图使用或者绘制
                                                             //(前提是当前gd.SetRenderTarget的不是这个render,否则会报错)
                                                             //因为这个render保存的是刚刚顶点绘制的图像,所以tex0会是顶点绘制绘制到的区域
            OffsetShader.Parameters["offset"].SetValue(new Vector2(0.02f, 0.01f));//偏移度
            OffsetShader.Parameters["invAlpha"].SetValue(0);//反色
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//这个Draw是空间切割对应的
                                                                  //Draw目前的世界图像,也就是screenTarget内容
            sb.End();
            sb.Begin();
            sb.Draw(render, Vector2.Zero, Color.White);//这里把自己的画 画上去

            gd.SetRenderTarget(Main.screenTarget);//设置为screenTarget
            gd.Clear(Color.Transparent);//透明清除图片
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);//绘制修改的画
            sb.Draw(render, Vector2.Zero, Color.White);
            sb.End();
        }
    }
}
