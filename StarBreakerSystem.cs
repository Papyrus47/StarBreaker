using StarBreaker.Content.Appraise;
using StarBreaker.Content.ControlPlayerSystem;
using StarBreaker.Content.FreeDraw;
using StarBreaker.Content.Mission;
using StarBreaker.UIs.SelectUI;
using Terraria.GameContent.RGB;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Graphics.Capture;
using Terraria.IO;
using Terraria.UI;
using Terraria.WorldBuilding;

namespace StarBreaker
{
    public class StarBreakerSystem : ModSystem
    {
        public TestSelectUI testSelect;
        public static bool InWorld;
        public static bool WorldLoad;
        public static int Missions;
        public static string WorldMissionID;
        /// <summary>
        /// 星击的角色系统
        /// </summary>
        public static BasicControlPlayerSystem playerSystem;
        public override void Load()
        {
            On_Main.DrawCachedProjs += Main_DrawCachedProjs;
            On_Main.DrawInterface_27_Inventory += On_Main_DrawInterface_27_Inventory;
            On_Main.DrawSettingButton += On_Main_DrawSettingButton;
            On_Main.DrawToMap += On_Main_DrawToMap;
            On_Main.DrawToMap_Section += On_Main_DrawToMap_Section;
            On_Main.ShouldUpdateEntities += On_Main_ShouldUpdateEntities;
            On_Main.DoUpdateInWorld += On_Main_DoUpdateInWorld;
            On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += On_ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
            On_ItemSlot.Handle_ItemArray_int_int += On_ItemSlot_Handle_ItemArray_int_int;
            On_Main.OpenCharacterSelectUI += On_Main_OpenCharacterSelectUI;
            On_Main.GUIBarsDrawInner += On_Main_GUIBarsDrawInner;
            On_Main.DrawInterface_30_Hotbar += On_Main_DrawInterface_30_Hotbar;
            Main.OnPostDraw += Main_OnPostDraw;
            On_WorldGen.SaveAndQuitCallBack += On_WorldGen_SaveAndQuitCallBack;
            AddMission();
        }

        private void On_Main_DrawInterface_30_Hotbar(On_Main.orig_DrawInterface_30_Hotbar orig, Main self)
        {
            // orig.Invoke(self);
        }

        private void On_ItemSlot_Handle_ItemArray_int_int(On_ItemSlot.orig_Handle_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            //orig.Invoke(inv, context, slot);
        }

        private void On_WorldGen_SaveAndQuitCallBack(On_WorldGen.orig_SaveAndQuitCallBack orig, object threadContext)
        {
            //orig.Invoke(threadContext);
            Main.invasionProgress = -1;
            Main.invasionProgressDisplayLeft = 0;
            Main.invasionProgressAlpha = 0f;
            Main.invasionProgressIcon = 0;
            Main.menuMode = 10;
            Main.gameMenu = true;
            SoundEngine.StopTrackedSounds();
            Main.ambientWaterfallStrength = 0f;
            Main.ambientLavafallStrength = 0f;

            CaptureInterface.ResetFocus();
            Main.ActivePlayerFileData.StopPlayTimer();
            Player.ClearPlayerTempInfo();
            Rain.ClearRain();
            SystemLoader.OnWorldUnload();
            Main.UpdateTimeRate();
            Main.menuMode = 0;
            InWorld = WorldLoad = false;
            WorldMissionID = null;
            Main.ActivePlayerFileData = null;
            Main.ActiveWorldFileData = null;
            if (threadContext != null)
            {
                ((Action)threadContext)();
            }
        }

        private void On_Main_GUIBarsDrawInner(On_Main.orig_GUIBarsDrawInner orig, Main self)
        {
            // orig.Invoke(self);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            playerSystem.UpdateUI(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            playerSystem.DrawUI(layers);
        }

        private bool On_Main_ShouldUpdateEntities(On_Main.orig_ShouldUpdateEntities orig, Main self)
        {
            return InWorld && WorldLoad;
        }

        private void On_Main_OpenCharacterSelectUI(On_Main.orig_OpenCharacterSelectUI orig)
        {
            Main.menuMode = 888;
            if (testSelect == null)
            {
                testSelect = new();
                testSelect.Initialize();
            }
            Main.MenuUI.SetState(testSelect);
            //orig.Invoke();
        }

        public static void AddMission()
        {
            MissionSystem helper = MissionSystem.Instance;
            Test test = new();
            helper.AddMission(test.MissionID, test);
        }
        private void On_Main_DoUpdateInWorld(On_Main.orig_DoUpdateInWorld orig, Main self, System.Diagnostics.Stopwatch sw)
        {
            if(InWorld && !WorldLoad)
            {
                WorldLoad = MissionSystem.Instance.MissionLoad_Update(WorldMissionID);
                return;
            }
            orig.Invoke(self, sw);
        }

        private void On_Main_DrawToMap_Section(On_Main.orig_DrawToMap_Section orig, Main self, int secX, int secY)
        {
            //orig.Invoke(self, secX, secY);
        }

        private void On_Main_DrawToMap(On_Main.orig_DrawToMap orig, Main self)
        {
            //orig.Invoke(self);
        }


        private void On_ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor)
        {
            // orig.Invoke(spriteBatch, inv, context, slot, position, lightColor);
        }

        private void On_Main_DrawSettingButton(On_Main.orig_DrawSettingButton orig, ref bool mouseOver, ref float scale, int posX, int posY, string text, string textSizeMatcher, Action clickAction)
        {
            //if (InWorld) return;
            posX = 100;
            orig.Invoke(ref mouseOver, ref scale, posX, posY, text, textSizeMatcher, clickAction);
        }
        private void On_Main_DrawInterface_27_Inventory(On_Main.orig_DrawInterface_27_Inventory orig, Main self)
        {
            //orig.Invoke(self);
        }
        private void Main_OnPostDraw(GameTime obj)
        {
            AppraiseSystem.Instance.Draw();
        }
        private static void Main_DrawCachedProjs(On_Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
        {
            bool flag = StarBreakerUtils.InBegin();

            if (!flag)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor,
                DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }

            if (projCache == self.DrawCacheProjsBehindNPCsAndTiles)
            {
                FreeDrawSystem.Instance.Draw(FreeDrawEnum.BehindNPCsAndTiles);
            }
            else if (projCache == self.DrawCacheProjsBehindNPCs)
            {
                FreeDrawSystem.Instance.Draw(FreeDrawEnum.BehindNPCs);
            }
            else if (projCache == self.DrawCacheProjsBehindProjectiles)
            {
                FreeDrawSystem.Instance.Draw(FreeDrawEnum.BehindProjectiles);
            }
            else if (projCache == self.DrawCacheProjsOverPlayers)
            {
                FreeDrawSystem.Instance.Draw(FreeDrawEnum.OverPlayers);
            }
            else if (projCache == self.DrawCacheProjsOverWiresUI)
            {
                FreeDrawSystem.Instance.Draw(FreeDrawEnum.OverWiresUI);
            }

            if (!flag)
            {
                Main.spriteBatch.End();
            }

            orig.Invoke(self, projCache, startSpriteBatch);
        }
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            tileColor = Color.White * 0.8f;
            backgroundColor = Color.White * 0.8f;
            Lighting.AddLight(Main.LocalPlayer.position, new Vector3(2, 2, 2));
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (InWorld && !WorldLoad)
            {
                MissionSystem.Instance.MissionLoad_Draw(spriteBatch);
            }
        }
        public override void PostUpdateEverything()
        {
            AppraiseSystem.Instance.Update();
            Main.LocalPlayer.selectedItem = 1;
        }
        public override void Unload()
        {
            MissionSystem.Instance.UnLoad();
            AppraiseSystem.Instance.UnLoad();
            playerSystem = null;
            //On_Main.DrawCachedProjs -= Main_DrawCachedProjs;
            //On_Main.DrawInterface_27_Inventory -= On_Main_DrawInterface_27_Inventory;
            //On_Main.DrawSettingButton -= On_Main_DrawSettingButton;
            //On_Main.DrawToMap -= On_Main_DrawToMap;
            //On_Main.DrawToMap_Section -= On_Main_DrawToMap_Section;
            //On_Main.ShouldUpdateEntities -= On_Main_ShouldUpdateEntities;
            //On_Main.DoUpdateInWorld -= On_Main_DoUpdateInWorld;
            //On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color -= On_ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
            //On_Main.OpenCharacterSelectUI -= On_Main_OpenCharacterSelectUI;
            //Main.OnPostDraw -= Main_OnPostDraw;
        }
    }
}
