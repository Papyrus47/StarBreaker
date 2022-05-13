using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarBreaker.Projs.Waste;
using StarBreaker.StarUI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static On.Terraria.Graphics.Effects.FilterManager;

namespace StarBreaker
{
    public class StarBreaker : Mod
    {
        public static StarBreaker Instantiate;
        public static ModKeybind ToggleGhostSwordAttack;
        public static Effect FrostFistHealMagic;
        public static Effect LightStar;
        public static Effect GhostSlash;
        public static Effect EliminateRaysShader;
        public static Effect OffsetShader;//偏移用的shader

        RenderTarget2D render;

        #region UI变量
        #region 星击子弹UI
        public StarBreakerUIState starBreaker_UI;
        internal UserInterface _userInterface;
        #endregion
        #region 星击能量条显示UI
        public StarChargeUIState chargeUIState;
        internal UserInterface chargeUser;
        #endregion
        #endregion
        public override void Load()
        {
            Instantiate = this;//加载时，获取Mod实例
            #region 鼠标按键
            ToggleGhostSwordAttack = KeybindLoader.RegisterKeybind(this, "星辰鬼刀-状态切换", "E");
            #endregion
            #region 普通shader
            FrostFistHealMagic = ModContent.Request<Effect>("StarBreaker/Effects/Content/FrostFistHealMagic").Value;
            LightStar = ModContent.Request<Effect>("StarBreaker/Effects/Content/LightStar").Value;
            EliminateRaysShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/EliminateRaysShader").Value;
            OffsetShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/Offset").Value;
            #endregion
            #region 屏幕shader
            if (!Main.dedServ)
            {
                GhostSlash = ModContent.Request<Effect>("StarBreaker/Effects/Content/GhostSlash").Value;
                Filters.Scene["StarBreaker:GhostSlash"] = new Filter(
                    new TestScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("StarBreaker/Effects/Content/GhostSlash").Value), "GhostSlash"), EffectPriority.Medium);
                Filters.Scene["StarBreaker:GhostSlash"].Load();
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
            #endregion
            #region 声音加载
            for (int i = 1; i <= 5; i++)
            {
                SoundLoader.AddSound(this, "Sounds/Kazoo/kazoo" + i.ToString());
            }
            SoundLoader.AddSound(this, "Sounds/Drum/Drum1");
            #endregion
            #region UI加载
            if (!Main.dedServ)//不在服务器上
            {
                starBreaker_UI = new();//创建UIState实例
                starBreaker_UI.Activate();//激活
                _userInterface = new();//创建实例

                _userInterface.SetState(starBreaker_UI);//送入userInterface
                                                        //更新UI到了System里面

                chargeUIState = new();
                chargeUIState.Activate();
                chargeUser = new();
                chargeUser.SetState(chargeUIState);
            }
            #endregion
            On.Terraria.Main.UpdateAudio_DecideOnNewMusic += Main_UpdateAudio_DecideOnNewMusic;//可以修改原版曲子
        }

        private void Main_UpdateAudio_DecideOnNewMusic(On.Terraria.Main.orig_UpdateAudio_DecideOnNewMusic orig, Main self)
        {
            orig(self);
            if (Main.gameMenu) return;
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
            #region 鼠标按键
            ToggleGhostSwordAttack = KeybindLoader.RegisterKeybind(this, "星辰鬼刀-状态切换", "E");
            #endregion
            #region 普通shader
            FrostFistHealMagic = ModContent.Request<Effect>("StarBreaker/Effects/Content/FrostFistHealMagic").Value;
            LightStar = ModContent.Request<Effect>("StarBreaker/Effects/Content/LightStar").Value;
            EliminateRaysShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/EliminateRaysShader").Value;
            OffsetShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/Offset").Value;
            #endregion
            #region 屏幕shader
            if (!Main.dedServ)
            {
                GhostSlash = ModContent.Request<Effect>("StarBreaker/Effects/Content/GhostSlash").Value;
                Filters.Scene["StarBreaker:GhostSlash"] = new Filter(
                    new TestScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("StarBreaker/Effects/Content/GhostSlash").Value), "GhostSlash"), EffectPriority.Medium);
                Filters.Scene["StarBreaker:GhostSlash"].Load();
            }
            #endregion
            #region sky
            SkyManager.Instance["StarBreaker:StarSky"] = new Backgronuds.StarSky();
            SkyManager.Instance["StarBreaker:Portal"] = new Backgronuds.Portal();
            #endregion
            #region RT2D
            On.Terraria.Graphics.Effects.FilterManager.EndCapture += FilterManager_EndCapture;
            On.Terraria.Main.LoadWorlds += Main_LoadWorlds;
            Terraria.Main.OnResolutionChanged += Main_OnResolutionChanged;
            #endregion
            #region 标题更改
            try
            {
                if (Main.rand.NextBool() && Language.ActiveCulture == GameCulture.FromCultureName(GameCulture.CultureName.Chinese))
                {
                    Main.instance.Window.Title = "泰拉瑞亚:星辰击碎者!";
                }
            }
            catch { }
            #endregion
        }
        public override void Unload()
        {
            starBreaker_UI = null;
            _userInterface = null;
            Instantiate = null;
            FrostFistHealMagic = null;
            LightStar = null;
            ToggleGhostSwordAttack = null;
            EliminateRaysShader = null;
            EndCapture -= FilterManager_EndCapture;
            On.Terraria.Main.LoadWorlds -= Main_LoadWorlds;
            On.Terraria.Main.UpdateAudio_DecideOnNewMusic -= Main_UpdateAudio_DecideOnNewMusic;
            Terraria.Main.OnResolutionChanged -= Main_OnResolutionChanged;
        }
        private void FilterManager_EndCapture(orig_EndCapture orig,FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            if (render == null) return;
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

            #region 繁星刺破 绘制
            foreach (Projectile Projectile in Main.projectile)
            {
                if (Projectile.active && Projectile.type == ModContent.ProjectileType<StarsPierceProj_Pierce>())
                {
                    List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

                    // 把所有的点都生成出来，按照顺序
                    for (int i = 1; i < Projectile.oldPos.Length; ++i)
                    {
                        if (Projectile.oldPos[i] == Vector2.Zero) break;

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
                        List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
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
                        gd.Clear(Color.Transparent);
                        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
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
                        OffsetShader.Parameters["offset"].SetValue(new Vector2(0.05f, 0.01f));//偏移度
                        OffsetShader.Parameters["invAlpha"].SetValue(0);//反色
                        sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//这个Draw是空间切割对应的
                        sb.End();

                        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                        //这一个是绘制星辰
                        gd.Textures[1] = ModContent.Request<Texture2D>("StarBreaker/Backgronuds/LightB").Value;
                        LightStar.CurrentTechnique.Passes[0].Apply();
                        LightStar.Parameters["m"].SetValue(0.5f);
                        LightStar.Parameters["n"].SetValue(0.01f);

                        sb.Draw(render, Vector2.Zero, Color.White);//这里把自己的画 画上去
                        sb.End();

                        gd.SetRenderTarget(Main.screenTarget);//设置为screenTarget
                        gd.Clear(Color.Transparent);//透明清除图片
                        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//Draw开始
                        sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);//绘制修改的画
                        sb.End();
                    }
                }
            }
            #endregion
            //sb.End();
            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //gd.Textures[1] = ModContent.Request<Texture2D>("StarBreaker/Backgronuds/LightB").Value;
            //LightStar.CurrentTechnique.Passes[0].Apply();
            //LightStar.Parameters["m"].SetValue(0.5f);
            //LightStar.Parameters["n"].SetValue(0.01f);
            //sb.Draw(render, Vector2.Zero, Color.White);
            //sb.End();

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