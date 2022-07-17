using ReLogic.Content;
using StarBreaker.Effects;
using StarBreaker.Items.Weapon.DoomFight;
using StarBreaker.Projs.Type;
using StarBreaker.Projs.Waste;
using StarBreaker.StarUI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.UI;
using static On.Terraria.Graphics.Effects.FilterManager;
using Filters = Terraria.Graphics.Effects.Filters;

namespace StarBreaker
{
    public class StarBreaker : Mod
    {
        private static StarBreaker instantiate;//这一个就是静态字段,在哪里都能调用,但是要加一个类名前缀
        public static StarBreaker Instantiate { get => instantiate; private set => instantiate = value; }
        public static ModKeybind ToggleGhostSwordAttack;
        public static Effect FrostFistHealMagic;
        public static Effect LightStar;
        public static Effect EliminateRaysShader;
        public static Effect OffsetShader;//偏移用的shader
        public static Effect UseSwordShader;//剑类挥动

        private RenderTarget2D render;

        public override void Load()
        {
            Instantiate = this;//加载时，获取Mod实例
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
            #region 鼠标按键
            ToggleGhostSwordAttack = KeybindLoader.RegisterKeybind(this, "星辰鬼刀-状态切换", "E");
            #endregion
            #region 普通shader
            FrostFistHealMagic = ModContent.Request<Effect>("StarBreaker/Effects/Content/FrostFistHealMagic",mode).Value;
            LightStar = ModContent.Request<Effect>("StarBreaker/Effects/Content/LightStar", mode).Value;
            EliminateRaysShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/EliminateRaysShader", mode).Value;
            OffsetShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/Offset", mode).Value;
            UseSwordShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/UseSwordShader", mode).Value;
            #endregion
            #region 屏幕shader
            if (!Main.dedServ)
            {
                Filters.Scene["StarBreaker:GhostSlash"] = new Filter(
                    new GhostSlash(new Ref<Effect>(ModContent.Request<Effect>("StarBreaker/Effects/Content/GhostSlash",mode).Value), "GhostSlash"), EffectPriority.Medium);
                Filters.Scene["StarBreaker:GhostSlash"].Load();//星辰鬼刀

                Filters.Scene["StarBreaker:ShockWave"] = new Filter(new ScreenShaderData(
                    new Ref<Effect>(ModContent.Request<Effect>("StarBreaker/Effects/Content/ShockWave", mode).Value), "ShockWave"), EffectPriority.VeryHigh);
                Filters.Scene["StarBreaker:ShockWave"].Load();//屏幕震撼shader
            }
            #endregion
            #region sky
            SkyManager.Instance["StarBreaker:StarSky"] = new Backgronuds.StarSky();
            SkyManager.Instance["StarBreaker:Portal"] = new Backgronuds.Portal();
            SkyManager.Instance["StarBreaker:FrostFistSky"] = new Backgronuds.FrostFistSky();
            #endregion
            #region RT2D
            On.Terraria.Graphics.Effects.FilterManager.EndCapture += FilterManager_EndCapture;
            On.Terraria.Main.LoadWorlds += Main_LoadWorlds;
            Terraria.Main.OnResolutionChanged += Main_OnResolutionChanged;
            #endregion
            #region 音乐加载
            MusicLoader.AddMusic(this, "Music/StarGhostBladeDemonSwordization");
            MusicLoader.AddMusic(this, "Music/AttackOfTheKillerQueen");
            MusicLoader.AddMusic(this, "Music/StarGloveProvingGround");
            MusicLoader.AddMusic(this, "Music/DarkPurgatoryIntrusion");
            MusicLoader.AddMusic(this, "Music/RedMist");
            MusicLoader.AddMusic(this, "Sounds/Music/StarBreakerOP");
            MusicLoader.AddMusic(this, "Music/Atk1");
            MusicLoader.AddMusic(this, "Music/Atk2");
            MusicLoader.AddMusic(this, "Music/Atk3");
            MusicLoader.AddMusic(this, "Sounds/Music/Bloodtower2");
            MusicLoader.AddMusic(this, "Music/Argalia");
            MusicLoader.AddMusic(this, "Music/UnderfootEnterenceBoss");
            #endregion
            #region 特殊战背景
            On.Terraria.Main.DrawTiles += Main_DrawTiles;
            On.Terraria.Player.WaterCollision += Player_WaterCollision;
            On.Terraria.Main.DrawWaters += Main_DrawWaters;
            On.Terraria.Player.DryCollision += Player_DryCollision;
            On.Terraria.Player.HoneyCollision += Player_HoneyCollision;
            #endregion
            On.Terraria.Main.UpdateAudio_DecideOnNewMusic += Main_UpdateAudio_DecideOnNewMusic;//可以修改原版曲子
        }

        private void Player_HoneyCollision(On.Terraria.Player.orig_HoneyCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (StarBreakerSystem.SpecialBattle == null || !StarBreakerSystem.SpecialBattle.active)
            {
                orig.Invoke(self, fallThrough, ignorePlats);
            }
            else
            {
                self.position += self.velocity;
            }
        }

        private void Player_DryCollision(On.Terraria.Player.orig_DryCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (StarBreakerSystem.SpecialBattle == null || !StarBreakerSystem.SpecialBattle.active)
            {
                orig.Invoke(self, fallThrough, ignorePlats);
            }
            else
            {
                self.position += self.velocity;
            }
        }

        private void Main_DrawWaters(On.Terraria.Main.orig_DrawWaters orig, Main self, bool isBackground)
        {
            if (StarBreakerSystem.SpecialBattle == null || !StarBreakerSystem.SpecialBattle.active)
            {
                orig.Invoke(self, isBackground);
            }
        }
        private void Player_WaterCollision(On.Terraria.Player.orig_WaterCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (StarBreakerSystem.SpecialBattle == null || !StarBreakerSystem.SpecialBattle.active)
            {
                orig.Invoke(self, fallThrough, ignorePlats);
            }
            else
            {
                self.position += self.velocity;
            }
        }

        private void Main_DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
        {
            if (StarBreakerSystem.SpecialBattle != null && StarBreakerSystem.SpecialBattle.active)
            {
                StarBreakerSystem.SpecialBattle.Draw(Main.spriteBatch);
            }
            else
            {
                try
                {
                    orig.Invoke(self, solidLayer, forRenderTargets, intoRenderTargets, waterStyleOverride);
                }
                catch { }
            }
        }

        private void Main_UpdateAudio_DecideOnNewMusic(On.Terraria.Main.orig_UpdateAudio_DecideOnNewMusic orig, Main self)
        {
            orig(self);
            if (Main.gameMenu)
            {
                return;
            }

            if (Main.LocalPlayer.TryGetModPlayer(out StarPlayer starPlayer))
            {
                if (starPlayer.EGO)
                {
                    Main.newMusic = MusicLoader.GetMusicSlot(this, "Music/RedMist");
                }
            }
        }

        public override void PostSetupContent()
        {
            #region 标题更改
            try
            {
                if (Main.rand.NextBool(5))
                {
                    const int MaxTitle = 6;
                    int i = Main.rand.Next(MaxTitle);
                    switch (i++)
                    {
                        case 1:
                            {
                                Text("泰拉瑞亚:星辰击碎者!", "Terraria:Star Breaker!");
                                break;
                            }
                        case 2:
                            {
                                Text("泰拉瑞亚:星辰旋刃,你在做什么!", "Terraria:Star Spiral Blade,What are you do in!");
                                break;
                            }
                        case 3:
                            {
                                Text("泰拉瑞亚:试试废墟图书馆", "Terraria:Try Library of Ruina");
                                break;
                            }
                        case 4:
                            {
                                Text("泰拉瑞亚:试试地下层与勇士", "Terraria:Try Dungeon and Fighter");
                                break;
                            }
                        case 5:
                            {
                                Text("泰拉瑞亚:试试Toby Fox的传说之下和三角符文", "Terraria:Try Deltarune and UnderTale by Toby Fox");
                                break;
                            }
                        case 6:
                            {
                                Text("星辰之主:额...我是不是强行霸占了标题?欢迎来到A-245a-0-1时间线,泰拉瑞亚(?)",
                                    "Star Owenr:Emmm...I'm control title just now? Welcome to the A-245a-0-1 Time Line,Terraria(?)");
                                break;
                            }
                    }
                }
            }
            catch { }

            static void Text(string Cn_Name, string Name)
            {
                if (Language.ActiveCulture == GameCulture.FromCultureName(GameCulture.CultureName.Chinese))
                {
                    Main.instance.Window.Title = Cn_Name;
                }
                else
                {
                    Main.instance.Window.Title = Name;
                }
            }
            #endregion
        }
        public override void Unload()
        {
            Instantiate = null;
            FrostFistHealMagic = null;
            LightStar = null;
            ToggleGhostSwordAttack = null;
            EliminateRaysShader = null;

            EndCapture -= FilterManager_EndCapture;
            On.Terraria.Main.LoadWorlds -= Main_LoadWorlds;
            On.Terraria.Main.UpdateAudio_DecideOnNewMusic -= Main_UpdateAudio_DecideOnNewMusic;
            Terraria.Main.OnResolutionChanged -= Main_OnResolutionChanged;
            On.Terraria.Main.DrawTiles -= Main_DrawTiles;
            On.Terraria.Player.WaterCollision -= Player_WaterCollision;
            On.Terraria.Main.DrawWaters -= Main_DrawWaters;
            On.Terraria.Player.DryCollision -= Player_DryCollision;
            On.Terraria.Player.HoneyCollision -= Player_HoneyCollision;
        }

        private void FilterManager_EndCapture(orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            if (render == null)
            {
                return;
            }

            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

            //gd.SetRenderTarget(Main.screenTargetSwap);//在上面绘制原图片，就相当于保存
            //gd.Clear(Color.Transparent);//使用透明清除这个图片
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            //sb.End();

            //gd.SetRenderTarget(render);//设置在自己的RT2D上的
            //gd.Clear(Color.Transparent);//使用透明清除这个图片
            //sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState,
            //    DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (Projectile Projectile in Main.projectile)
            {
                #region 繁星刺破 绘制
                if (Projectile.active && Projectile.type == ModContent.ProjectileType<StarsPierceProj_Pierce>())
                {
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
                        Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("StarBreaker/Images/MyExtra_1").Value;
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
                        gd.Textures[1] = ModContent.Request<Texture2D>("StarBreaker/Backgronuds/LightB").Value;
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
                #endregion
                #region 凌空之剑挥舞 特效绘制
                if (Projectile.active && Projectile.type == ModContent.ProjectileType<VolleySwordProj>() && Projectile.ai[0] < 3)
                {

                    gd.SetRenderTarget(render);
                    gd.Clear(Color.Transparent);
                    try
                    {
                        VolleySwordProj.DrawVectrx(Projectile, (Projectile.ModProjectile as VolleySwordProj).oldVels);

                        gd.SetRenderTarget(Main.screenTargetSwap);
                        gd.Clear(Color.Transparent);
                        if (!StarBreakerWay.InBegin())
                        {
                            sb.Begin();
                        }
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
                    catch { }
                }
                #endregion
                #region 基类挥舞
                if (Projectile.active && Projectile.ModProjectile is Projs.Type.BaseMeleeItemProj proj)
                {
                    if (!proj.CanDraw())
                    {
                        continue;
                    }
                    gd.SetRenderTarget(render);
                    gd.Clear(Color.Transparent);
                    BaseMeleeItemProj.DrawVectrx(Projectile, proj.oldVels, proj.LerpColor, proj.LerpColor2, proj.DrawLength);

                    gd.SetRenderTarget(Main.screenTargetSwap);
                    gd.Clear(Color.Transparent);
                    if (!StarBreakerWay.InBegin())
                    {
                        sb.Begin();
                    }
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
                #endregion
            }
            //sb.End();
            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //gd.Textures[1] = ModContent.Request<Texture2D>("StarBreaker/Backgronuds/LightB").Value;
            //LightStar.CurrentTechnique.Passes[0].Apply();
            //LightStar.Parameters["m"].SetValue(0.5f);
            //LightStar.Parameters["n"].SetValue(0.01f);
            //sb.Draw(render, Vector2.Zero, Color.White);
            //sb.End();
            if (StarBreakerWay.InBegin())
            {
                sb.End();
            }
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
        private void Main_OnResolutionChanged(Vector2 obj)
        {
            render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
        }

        private void Main_LoadWorlds(On.Terraria.Main.orig_LoadWorlds orig)
        {
            render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            orig();
        }
    }
}