using System.Linq;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace StarBreaker.StarUI.Research
{
    public class StarBook_UI : UIState
    {
        public Dictionary<int,StarBook_TaskUI> starBook_Task;
        public bool CanDrawList = true;
        public int Page = 0;
        public int DrawTaskUI_Index = -1;
        public override void OnInitialize()
        {
            starBook_Task = new();

            StarBook_BookPanel starBook_BookPanel = new StarBook_BookPanel();
            starBook_BookPanel.Top.Set(Main.screenHeight / 2f, 0f);
            starBook_BookPanel.Left.Set(Main.screenWidth / 2f, 0f);
            starBook_BookPanel.Width.Set(618,0);
            starBook_BookPanel.Height.Set(498, 0);
            Append(starBook_BookPanel);
            #region 遍历所有的类,获取对应的事件
            var starBook_Event = typeof(StarBook_Event).Assembly.GetTypes()//获取所有的类库下所有的类
                .Where(t => typeof(StarBook_Event).IsAssignableFrom(t))//获取间接继承或者直接继承了它的类
                .Where(t => t.IsClass && !t.IsAbstract)//获取非抽象类的实例
                .Select(t => (StarBook_Event)Activator.CreateInstance(t)).ToArray();//创建实例
            #endregion
            #region 加载事件
            for (int i = 0;i<starBook_Event.Length;i++)
            {
                starBook_Task.Add(i,new(starBook_Event[i]));
                #region 设置高宽
                starBook_Task[i].Width.Set(160, 0);
                starBook_Task[i].Height.Set(22, 0);
                #endregion
                starBook_Task[i].Top.Set(i % 16 * 30, 0);//设置位置
                float left = 51.5f;
                if(i % 16 >= 8)
                {
                    left = 462;
                }
                starBook_Task[i].Left.Set(left, 0);
            }
            #endregion
            for(int i = 0;i< starBook_Task.Count;i++)
            {
                starBook_BookPanel.Append(starBook_Task[i]);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Main.LocalPlayer.HeldItem.Name.Equals("星书"))
            {
                Elements[0].Draw(spriteBatch);
                if(CanDrawList)//可以画事件预览UI
                {
                    for(int i =0;i<16;i++)//绘制事件UI
                    {
                        int index = (16 * Page) + i;
                        if (i >= starBook_Task.Count) break;
                        starBook_Task[index].Draw(spriteBatch);
                    }
                }
                else if(DrawTaskUI_Index > -1 && DrawTaskUI_Index < starBook_Task.Count && starBook_Task[DrawTaskUI_Index] is StarBook_TaskUI taskUI)
                {
                    taskUI.DrawTask(spriteBatch);
                }
                else
                {
                    DrawTaskUI_Index = -1;
                    CanDrawList = true;
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.HeldItem.Name.Equals("星书"))
            {
                if (CanDrawList)//可以画事件
                {
                    for (int i = 0; i < 16; i++)//控制事件UI
                    {
                        int index = (16 * Page) + i;
                        if (i >= starBook_Task.Count) break;
                        starBook_Task[index].Update(gameTime);
                    }
                }
                else if (DrawTaskUI_Index > -1 && DrawTaskUI_Index < starBook_Task.Count && starBook_Task[DrawTaskUI_Index] is StarBook_TaskUI taskUI)
                {
                    taskUI.Update(gameTime);
                }
            }
        }
    }
    public class StarBook_BookPanel : UIPanel
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>((GetType().Namespace + "." + GetType().Name).Replace('.', '/')).Value;
            spriteBatch.Draw(texture, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White, 0, texture.Size() * 0.5f, 3f, SpriteEffects.None, 0f);
        }
    }
}
