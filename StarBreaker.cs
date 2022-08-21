using StarBreaker.Effects;
using StarBreaker.MyGraphics.RenderTargetProjDraws;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using StarBreaker.MyGraphics.RenderTargetBloom;
using static On.Terraria.Graphics.Effects.FilterManager;
using Filters = Terraria.Graphics.Effects.Filters;
using Terraria.Map;
using MonoMod.Cil;
using StarBreaker.MyGraphics.Clouds;

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
        public static RenderTarget2D render;
        public static Effect UseSwordShader;//剑类挥动

        internal static RenderTargetProjDrawsHelper RenderTargetProjDrawsHelper;
        internal static BloomStsyem BloomStsyem;
        internal static CloundsSystem cloundsSystem;
        public override void Load()
        {
            Instantiate = this;//加载时，获取Mod实例
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
            RenderTargetProjDrawsHelper = new();
            BloomStsyem = new();
            BloomStsyem.Load(Assets);
            cloundsSystem = new();
            #region 鼠标按键
            ToggleGhostSwordAttack = KeybindLoader.RegisterKeybind(this, "星辰鬼刀-状态切换", "E");
            #endregion
            #region 普通shader
            FrostFistHealMagic = ModContent.Request<Effect>("StarBreaker/Effects/Content/FrostFistHealMagic", mode).Value;
            LightStar = ModContent.Request<Effect>("StarBreaker/Effects/Content/LightStar", mode).Value;
            EliminateRaysShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/EliminateRaysShader", mode).Value;
            OffsetShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/Offset", mode).Value;
            UseSwordShader = ModContent.Request<Effect>("StarBreaker/Effects/Content/UseSwordShader", mode).Value;
            #endregion
            #region 屏幕shader
            if (!Main.dedServ)
            {
                Filters.Scene["StarBreaker:GhostSlash"] = new Filter(
                    new GhostSlash(new Ref<Effect>(ModContent.Request<Effect>("StarBreaker/Effects/Content/GhostSlash", mode).Value), "GhostSlash"), EffectPriority.Medium);
                Filters.Scene["StarBreaker:GhostSlash"].Load();//星辰鬼刀

                Filters.Scene["StarBreaker:ShockWave"] = new Filter(new ScreenShaderData(
                    new Ref<Effect>(ModContent.Request<Effect>("StarBreaker/Effects/Content/ShockWave", mode).Value), "ShockWave"), EffectPriority.VeryHigh);
                Filters.Scene["StarBreaker:ShockWave"].Load();//冲击波shader
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
            On.Terraria.Main.DrawProj += Main_DrawProj;
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
            On.Terraria.Main.DrawCachedNPCs += Main_DrawCachedNPCs;//用于云的绘制
            On.Terraria.Main.UpdateAudio_DecideOnNewMusic += Main_UpdateAudio_DecideOnNewMusic;//可以修改原版曲子
            StarBreakerAssetTexture.LoadAll(Assets);
        }

        private void Main_DrawCachedNPCs(On.Terraria.Main.orig_DrawCachedNPCs orig, Main self, List<int> npcCache, bool behindTiles)
        {
            orig.Invoke(self, npcCache, behindTiles);
            if (Main.LocalPlayer.ZoneSkyHeight)
            {
                cloundsSystem.Draw();
            }
        }

        private void Main_DrawProj(On.Terraria.Main.orig_DrawProj orig, Main self, int whoAmI)
        {
            orig.Invoke(self, whoAmI);
            if (!Main.drawToScreen)
            {
                RenderTargetProjDrawsHelper.UpdateDraw(whoAmI);//调用绘制
            }
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
            if (Instantiate is not null)
            {
                Instantiate = null;
                FrostFistHealMagic.Dispose();
                FrostFistHealMagic = null;
                LightStar.Dispose();
                LightStar = null;
                ToggleGhostSwordAttack = null;
                EliminateRaysShader.Dispose();
                EliminateRaysShader = null;
                cloundsSystem.Dispose();
                cloundsSystem = null;
                RenderTargetProjDrawsHelper = null;

                StarBreakerAssetTexture.UnLoadAll();
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
        }

        private void FilterManager_EndCapture(orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            if (render == null) return;

            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

            BloomStsyem.Draw(screenTarget1, screenTarget2, render);
            if (StarBreakerUtils.InBegin())
            {
                sb.End();
            }
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
        private void Main_OnResolutionChanged(Vector2 obj) => NewRender();

        private void Main_LoadWorlds(On.Terraria.Main.orig_LoadWorlds orig)
        {
            NewRender();
            orig();
        }
        private void NewRender()
        {
            if(render != null)//如果已经拥有一个render对象
            {
                render.Dispose();//释放render
                render = null;//使render指向null
            }
            render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight,
                false,SurfaceFormat.Rgba64,DepthFormat.None,0,RenderTargetUsage.DiscardContents);//创建一个新的renderTarget对象
        }
    }
}