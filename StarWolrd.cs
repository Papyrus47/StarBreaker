using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace StarBreaker
{
    public class StarWolrd : ModSystem
    {
        public override void PostUpdateWorld()
        {
            if (NPCs.StarGlobalNPC.StarBreaker == -1)
            {
                if (SkyManager.Instance["StarBreaker:StarSky"].IsActive())
                {
                    SkyManager.Instance.Deactivate("StarBreaker:StarSky");
                }
            }
            if (NPCs.StarGlobalNPC.StarGhostKnife == -1)
            {
                if (SkyManager.Instance["StarBreaker:Portal"].IsActive())
                {
                    SkyManager.Instance.Deactivate("StarBreaker:Portal");
                }
            }
        }
    }
}
