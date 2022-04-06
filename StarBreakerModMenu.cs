using Terraria.ModLoader;

namespace StarBreaker
{
    public class StarBreakerModMenu : ModMenu
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/StarBreakerOP");
        public override string DisplayName => "星辰击碎者";
        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }
    }
}
