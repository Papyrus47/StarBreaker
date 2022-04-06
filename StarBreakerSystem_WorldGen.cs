using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace StarBreaker
{
    public partial class StarBreakerSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int FinalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalIndex != -1)
            {
                tasks.Insert(FinalIndex - 1, new PassLegacy("StarBreaker:StarSummon", (progress, con) =>
                 {
                     progress.Message = "星辰之地正在碎裂...";
                     Main.QueueMainThreadAction(() =>
                     {
                         Texture2D texture = ModContent.Request<Texture2D>("StarBreaker/Map_BorkenLandOfStar").Value;
                         Color[] colors = new Color[texture.Width * texture.Height];
                         ushort type = ((ushort)ModContent.TileType<Tiles.StarHardRock>());
                         texture.GetData(colors);
                         for (int x = 0; x < texture.Width; x++)
                         {
                             for (int y = 0; y < texture.Height; y++)
                             {
                                 if (colors[x + y * texture.Width].R == 255)
                                 {
                                     int posX = x + Main.maxTilesX / 4;
                                     int posY = y - 30;
                                     WorldGen.KillTile(posX, posY, false, false, false);
                                     WorldGen.KillWall(posX, posY, false);
                                     Main.tile[posX, posY].ClearEverything();
                                     Main.tile[posX, posY].LiquidAmount = 0;
                                     Main.tile[posX, posY].TileType = type;
                                     WorldGen.PlaceTile(posX, posY, type);
                                 }
                                 progress.CurrentPassWeight = x + y * texture.Width / colors.Length;
                             }
                         }
                     });
                 }));
            }
        }
    }
}
