using static Terraria.Recipe;

namespace StarBreaker
{
    /// <summary>
    /// 这是一个专门用于写合成表特殊信息的
    /// </summary>
    public class StarBreakerRecipe
    {
        public static Condition IfDownedStarBreakerEX =>
            new(NetworkText.FromKey("StarBreakerRecipe.IfDownedStarBreakerEX"),//这一段目前作用不清
            (Recipe _) => StarBreakerSystem.downed.downedStarBreakerEX);//这一段是判断合成表可以合成的条件
    }
}
