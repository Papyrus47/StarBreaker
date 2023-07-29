using Terraria.ModLoader.Config;

namespace StarBreaker.Config
{
    public class StarConfig : BasicConfig<StarConfig>
    {
        public override ConfigScope Mode => ConfigScope.ClientSide; // 服务器不加载shader,和服务器哪里来的关系
    }
}
