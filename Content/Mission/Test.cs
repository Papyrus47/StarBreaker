using System.Threading.Tasks;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace StarBreaker.Content.Mission
{
    public class Test : BasicMission
    {
        public override string MissionID => "Training";

        public override void SummonMission()
        {
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (y > Main.maxTilesY / 2)
                    {
                        WorldGen.PlaceTile(x, y, 690,true);
                    }
                }
                Progress = x / (Main.maxTilesX - 1f);
            }
        }
    }
}
