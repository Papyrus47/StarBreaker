using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarBreaker.StarUI;
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
            #region 盔甲shader（
            FrostFistHealMagic = ModContent.Request<Effect>("StarBreaker/Effects/Content/FrostFistHealMagic").Value;
            LightStar = ModContent.Request<Effect>("StarBreaker/Effects/Content/LightStar").Value;
            EliminateRaysShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/EliminateRaysShader").Value;
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
            StarBreakerLoadString.LoadString();
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
            #region 盔甲shader（
            FrostFistHealMagic = ModContent.Request<Effect>("StarBreaker/Effects/Content/FrostFistHealMagic").Value;
            LightStar = ModContent.Request<Effect>("StarBreaker/Effects/Content/LightStar").Value;
            EliminateRaysShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/EliminateRaysShader").Value;
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

            gd.SetRenderTarget(Main.screenTargetSwap);//在上面绘制原图片，就相当于保存
            gd.Clear(Color.Transparent);//使用透明清除这个图片
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();

            gd.SetRenderTarget(render);//设置在自己的RT2D上的
            gd.Clear(Color.Transparent);//使用透明清除这个图片
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            sb.End();

            gd.SetRenderTarget(Main.screenTarget);//切换回屏幕的
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            gd.Textures[1] = ModContent.Request<Texture2D>("StarBreaker/Backgronuds/LightB").Value;
            LightStar.CurrentTechnique.Passes[0].Apply();
            LightStar.Parameters["m"].SetValue(0.5f);
            LightStar.Parameters["n"].SetValue(0.02f);
            sb.Draw(render, Vector2.Zero, Color.White);
            sb.End();


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