using Terraria.ModLoader.Config;

namespace StarBreaker.Config
{
    public abstract class BasicConfig<T> : ModConfig where T : BasicConfig<T>
    {
        public static T Instance { get; set; }
        public override void OnLoaded()
        {
            Instance = this as T; // 我谢谢你,tml
        }
    }
}
