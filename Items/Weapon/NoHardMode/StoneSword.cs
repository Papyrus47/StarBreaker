namespace StarBreaker.Items.Weapon.NoHardMode//这一个是命名空间
    //命名空间的作用是给tml自己搜索贴图位置
    //所以不重写属性Tex的那个玩意,我们就需要把文件和图片放同一个文件夹下
    //而且要保证命名空间与文件夹位置对应
    //不然看起来会...
{
    public class StoneSword : ModItem //这里有两个类
                                      //StoneSword是我继承了ModItem的类
                                      //就是ModItem的子类
                                      //类的作用是为了给你写一堆balabala的玩意（
                                      //主要是继承了基类,方便你写
    {
        public override void SetStaticDefaults()//这一个带有override词缀的函数,叫做重写函数
            //重写函数的作用是...自己摸索吧（
        {
            DisplayName.SetDefault("Stone sword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "石剑");
            Tooltip.SetDefault("emm...This sword is \" HARD \"");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "唔...这剑是很\" 硬 \"");
        }
        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 40;//这一个行为我们叫做赋值
            //Item是一个类的实例
            //关于实例请学C#
            Item.damage = 8;
            Item.knockBack = 6.3f;
            Item.autoReuse = false;
            Item.useTurn = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 230;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.StoneBlock, 30).AddTile(TileID.WorkBenches).Register();
        }
    }
}
