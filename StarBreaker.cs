using StarBreaker.Content.ControlPlayerSystem;
using StarBreaker.Items.StarOwner.StarsPierceWeapon;
using Terraria.IO;

namespace StarBreaker
{
    public class StarBreaker : Mod
    {
        public static StarBreaker Instance;
        public static string PlayerSavePath;
        public static WorldFileData StarBreakerWorldFileData;
        public static PlayerFileData StarBreakerPlayerFileData;
        public override void Load()
        {
            Instance = this;
            StarBreakerAssetHelper.Load(Assets, RootContentSource);
            MusicLoader.AddMusic(this, "Sounds/Musics/Subhuman");
            On_Main.UpdateAudio_DecideOnNewMusic += Main_UpdateAudio_DecideOnNewMusic;
            PlayerSavePath = Main.PlayerPath + "/Cache/StarBreakerPlayer";
            BasicControlPlayerSystem.LoadStatic();
            StarBreakerWorldFileData = new(Main.WorldPath, false)
            {
                GameMode = GameModeID.Expert,
                CreationTime = DateTime.Now,
                Metadata = FileMetadata.FromCurrentSettings(FileType.World),
                WorldGeneratorVersion = 828928688129uL
            };
            StarBreakerPlayerFileData = new()
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
            };
        }
        public override void PostSetupContent()
        {

        }
        private void Main_UpdateAudio_DecideOnNewMusic(Terraria.On_Main.orig_UpdateAudio_DecideOnNewMusic orig, Main self)
        {
            orig.Invoke(self);
            if (Main.dedServ || Main.gameMenu)
            {
                return;//避免服务器或者在游戏界面加载
            }

            if (Main.LocalPlayer.HeldItem.ModItem is StarsPierce)
            {
                Main.newMusic = MusicLoader.GetMusicSlot(this, "Sounds/Musics/Subhuman");
            }
        }

        public override void Unload()
        {
            StarBreakerAssetHelper.UnLoad();
            PlayerSavePath = null;
            //On_Main.UpdateAudio_DecideOnNewMusic -= Main_UpdateAudio_DecideOnNewMusic;
        }
    }
}