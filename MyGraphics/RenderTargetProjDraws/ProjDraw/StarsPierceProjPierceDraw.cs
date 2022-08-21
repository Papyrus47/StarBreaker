using StarBreaker.Projs.Waste;
using static StarBreaker.StarBreaker;

namespace StarBreaker.MyGraphics.RenderTargetProjDraws.ProjDraw
{
    internal class StarsPierceProjPierceDraw : RenderTargetProjDraws
    {
        public override bool CanDraw(int projWhoAmI)
        {
            return Main.projectile[projWhoAmI].type == ModContent.ProjectileType<StarsPierceProj_Pierce>();
        }

        public override void Draw(int projwhoAmI)
        {
            Projectile Projectile = Main.projectile[projwhoAmI];
            List<CustomVertexInfo> bars = new();

            // 把所有的点都生成出来，按照顺序
            for (int i = 1; i < Projectile.oldPos.Length; ++i)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    break;
                }

                int width = 5 * (i < 10 ? i : 10);
                var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
                normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

                var factor = i / (float)Projectile.oldPos.Length;
                var color = Color.Lerp(Color.White, Color.Purple, factor);
                var w = MathHelper.Lerp(1f, 0.05f, factor);

                bars.Add(new CustomVertexInfo(Projectile.oldPos[i] + Projectile.Size * 0.5f + normalDir * width - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
                bars.Add(new CustomVertexInfo(Projectile.oldPos[i] + Projectile.Size * 0.5f + normalDir * -width - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
            }
            if (bars.Count > 2)
            {
                List<CustomVertexInfo> triangleList = new();
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    triangleList.Add(bars[i]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 1]);

                    triangleList.Add(bars[i + 1]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 3]);
                }
                gd.SetRenderTarget(render);//在自己的画
                gd.Clear(Color.Transparent);//透明清除紫色
                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap,
                    DepthStencilState.Default, RasterizerState.CullNone);//顶点绘制
                Main.graphics.GraphicsDevice.Textures[0] = StarBreakerAssetTexture.MyExtras[1].Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
                sb.End();
                //这里这一步顶点绘制已经完成
                //我们需要切换screenTarget
                //以便绘制"星辰背景"与空间切割

                gd.SetRenderTarget(Main.screenTargetSwap);//每一次切换RenderTarget,都会是这个被切换到的RenderTarget变成纯紫色图片
                gd.Clear(Color.Transparent);//透明清除图片
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                //这一个是切割空间
                OffsetShader.CurrentTechnique.Passes[0].Apply();
                OffsetShader.Parameters["tex0"].SetValue(render);//render可以当成贴图使用或者绘制
                                                                 //(前提是当前gd.SetRenderTarget的不是这个render,否则会报错)
                                                                 //因为这个render保存的是刚刚顶点绘制的图像,所以tex0会是顶点绘制绘制到的区域
                OffsetShader.Parameters["offset"].SetValue(new Vector2(0.05f, 0.01f));//偏移度
                OffsetShader.Parameters["invAlpha"].SetValue(0);//反色
                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//这个Draw是空间切割对应的
                                                                      //Draw目前的世界图像,也就是screenTarget内容
                sb.End();

                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                //这一个是绘制星辰
                gd.Textures[1] = StarBreakerAssetTexture.LightB.Value;
                LightStar.CurrentTechnique.Passes[0].Apply();
                LightStar.Parameters["m"].SetValue(0.5f);
                LightStar.Parameters["n"].SetValue(0.01f);

                sb.Draw(render, Vector2.Zero, Color.White);//这里把自己的画 画上去
                                                           //通过上面的shader,会影响其绘制的内容
                sb.End();

                gd.SetRenderTarget(Main.screenTarget);//设置为screenTarget
                gd.Clear(Color.Transparent);//透明清除图片
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//Draw开始
                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);//绘制修改的画
                sb.End();
            }
        }
    }
}
