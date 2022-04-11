using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;


namespace StarBreaker.Items.ItemTiles
{
    public class NewHome : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Wood}";//这里是一个表达式,获取原版贴图
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("New Home");
            Tooltip.SetDefault("Summon a home for you.");
            DisplayName.AddTranslation(7, "新房");
            Tooltip.AddTranslation(7, "为你生成一个新的房子");
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.autoReuse = false;
            Item.width = 12;
            Item.height = 12;
            Item.value = 0;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.Wood,399).AddTile(TileID.WorkBenches).Register();
        public override bool? UseItem(Player player)
        {
            try
            {
                int x = (int)Main.MouseWorld.X / 16 - 5;
                int y = (int)Main.MouseWorld.Y / 16 - 3;
                for (int i = -3; i <= 3; i++)//Y轴
                {
                    for (int j = -5; j <= 5; j++)//X轴
                    {
                        Tile tile = Main.tile[x + j, y + i];
                        if (i == -3 || i == 3 || j == 5 || j == -5)//生成边框
                        {
                            if((i == -3 || i == 3) && j == 3 && tile.HasTile)//如果两面有房子,生成平台
                            {
                                for(int k = 0;k<2;k++)
                                {
                                    Tile tile1 = Main.tile[x + j - k, y + i];
                                    tile1.ClearTile();
                                    WorldGen.PlaceTile(x + j - k, y + i, 19);
                                    WorldGen.PlaceWall(x + j - k, y + i, WallID.Wood);
                                }
                            }
                            else WorldGen.PlaceTile(x + j, y + i, TileID.WoodBlock);
                        }
                        else
                        {
                            WorldGen.PlaceWall(x + j, y + i, WallID.Wood);
                        }
                        if (i == 2 && j == 5)
                        {
                            for (int k = 0; k > -3; k--)
                            {
                                Tile tile1 = Main.tile[x + j, y + i + k];
                                tile1.ClearEverything();
                                WorldGen.PlaceWall(x + j, y + i + k, WallID.Wood);
                            }
                            WorldGen.PlaceDoor(x + j, y + i - 1, TileID.ClosedDoor);
                        }
                        if (i == 2 && j == -3)
                        {
                            WorldGen.PlaceTile(x + j, y + i, TileID.WorkBenches);
                            WorldGen.PlaceTile(x + j + 2, y + i, 15);
                            WorldGen.PlaceTile(x + j, y + i - 2, TileID.Torches);
                        }

                    }
                }
            }
            catch 
            {
                PopupText.NewText(new()
                {
                    Text = "生成失败!",
                    Color = Color.White,
                    DurationInFrames = 120,
                    Velocity = Vector2.UnitY * 5
                }, player.Center);
            }
            return base.UseItem(player);
        }
    }
}
