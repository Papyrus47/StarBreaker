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
            Item.useTime = 1;
            Item.autoReuse = false;
            Item.width = 12;
            Item.height = 12;
            Item.value = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Wood, 399).AddTile(TileID.WorkBenches).Register();
        }

        public override bool? UseItem(Player player)
        {
            try
            {
                if (player.itemAnimation > 8)
                {
                    Point point = Main.MouseWorld.ToTileCoordinates();
                    int[,] tileSet = _tileSet;
                    for (int Y = 0; Y < _tileSet.GetLength(0); Y++)
                    {
                        for (int X = 0; X < tileSet.GetLength(1); X++)
                        {
                            int setX = point.X + X;
                            int setY = point.Y + Y;
                            Tile tile = Main.tile[setX, setY];
                            if (tileSet[Y, X] != 1)//放墙
                            {
                                WorldGen.PlaceWall(setX, setY, WallID.Wood);
                            }
                            switch (tileSet[Y, X])
                            {
                                case 1://放木头
                                    {
                                        WorldGen.PlaceTile(setX, setY, TileID.WoodBlock);
                                        break;
                                    }
                                case 2://火把
                                    {
                                        WorldGen.PlaceTile(setX, setY, TileID.Torches);
                                        break;
                                    }
                                case 3://工作台
                                    {
                                        WorldGen.PlaceTile(setX, setY, 18);
                                        break;
                                    }
                                case 4://椅子
                                    {
                                        WorldGen.PlaceTile(setX, setY, 15, false, false, 1);
                                        break;
                                    }
                                case 5:
                                    {
                                        WorldGen.PlaceTile(setX, setY, TileID.ClosedDoor);
                                        break;
                                    }
                            }
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
            return false;
        }
        //1是木块
        //2是火把
        //3是工作台
        //4是椅子
        //5是门
        private int[,] _tileSet
        {
            get
            {
                return new int[,]{{ 1, 1, 1, 1, 1, 1,1,1,1,1},
                { 1, 0, 0, 0,0,0,0,0, 0, 1},
                { 1,2,0,0,0,0,0,0,0,0 },
                { 1,0,0,0,0,0,0,0,0,0 },
                { 1,0,3,0,0,0,4,0,0,5 },
                { 1,1,1,1,1,1,1,1,1,1} };
            }
        }
    }
}
