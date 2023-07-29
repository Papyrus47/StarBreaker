using System.IO;
using Terraria.GameContent.Creative;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.UI;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using static Terraria.GameContent.Creative.CreativePowers;
using StarBreaker.Content.ControlPlayerSystem;
using StarBreaker.UIs.PlayerControlSystemUI.StarOrigin;
using StarBreaker.UIs.PlayerControlSystemUI;
using Terraria.Social.Steam;

namespace StarBreaker.UIs.SelectUI
{
    public class TestSelectUI : UIState
    {
        public UITextPanel<string> TestUI;
        public override void OnInitialize()
        {
            TestUI = new("Test", 1f);
            TestUI.Left.Set(0, 0.5f);
            TestUI.Top.Set(0, 0.5f);
            TestUI.Width.Set(100, 0f);
            TestUI.Height.Set(50, 0f);
            TestUI.OnLeftClick += TestUI_OnClick;
            Append(TestUI);
        }

        private void TestUI_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            #region 世界生成
            WorldFileData worldFileData = StarBreaker.StarBreakerWorldFileData;
            worldFileData.Name = "Test";
            worldFileData.SetSeed("");

            worldFileData.SetAsActive();
            WorldGen.clearWorld();
            Main.maxTilesX = 1000;
            Main.maxTilesY = 800;
            WorldGen.setWorldSize();
            Main.spawnTileX = Main.maxTilesX / 2;
            Main.spawnTileY = Main.maxTilesY / 2;
            Main.worldSurface = Main.maxTilesY * 0.3;
            Main.rockLayer = Main.maxTilesY * 0.5;
            Main.weatherCounter = 1800;
            Cloud.resetClouds();
            Main.gameMenu = false;
            Main.gamePaused = false;
            #endregion
            Main.player[Main.myPlayer] = new();
            Player player = Main.player[Main.myPlayer];
            player.whoAmI = Main.myPlayer;
            player.name = "Test";
            player.savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules();
            CreativePowerManager.Instance.ResetDataForNewPlayer(Main.LocalPlayer);
            PlayerFileData playerFileData = StarBreaker.StarBreakerPlayerFileData;
            playerFileData.Player = Main.LocalPlayer;
            playerFileData.Name = "Test";
            Main.SelectPlayer(playerFileData);
            player.Update(Main.myPlayer);
            player.Spawn(PlayerSpawnContext.SpawningIntoWorld);
            foreach (Item item in Main.LocalPlayer.inventory)
            {
                item?.TurnToAir();
            }
            //PlayerLoader.OnEnterWorld(Main.myPlayer);
            StarBreakerSystem.InWorld = true;
            StarBreakerSystem.WorldLoad = false;
            StarBreakerSystem.WorldMissionID = "Training";
            StarBreakerSystem.playerSystem = new StarOriginControlSystem(Main.LocalPlayer);
            StarBreakerSystem.playerSystem.InitUI(new StarOriginControlUI());
        }
    }
}
