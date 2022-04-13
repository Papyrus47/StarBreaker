using Terraria.ModLoader;

namespace StarBreaker
{
    internal class StarBreakerLoadString//静态类无法实例化
    {
        private StarBreakerLoadString() { }
        public static void LoadString()
        {
            #region 星辰鬼刀
            ModTranslation atk = LocalizationLoader.CreateTranslation("StarGhostKnife.Atk1");
            atk.SetDefault("");
            atk.AddTranslation(7, "冥炎之卡洛");
            LocalizationLoader.AddTranslation(atk);
            #endregion
        }
    }
}