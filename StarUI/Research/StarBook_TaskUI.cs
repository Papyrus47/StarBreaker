using System.Linq;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace StarBreaker.StarUI.Research
{
    public class StarBook_TaskUI : UIElement
    {
        StarBook_Event starBook_Event = null;
        public StarBook_TaskUI() : base() { }
        public StarBook_TaskUI(StarBook_Event @event)
        {
            if (starBook_Event == null)
            {
                starBook_Event = @event;
            }
        }
        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);
            if(Parent is StarBook_BookPanel starBook_BookPanel && starBook_BookPanel.Parent is StarBook_UI star)
            {
                star.CanDrawList = false;
                for(int i = 0;i<star.starBook_Task.Count;i++)
                {
                    if(star.starBook_Task[i] == this)
                    {
                        star.DrawTaskUI_Index = i;
                        break;
                    }
                }
                
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public void DrawTask(SpriteBatch spriteBatch)
        {

        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle rectangle = GetDimensions().ToRectangle();//获取对应的矩阵
            spriteBatch.Draw(ModContent.Request<Texture2D>((GetType().Namespace + "." + GetType().Name).Replace('.', '/')).Value,rectangle.TopLeft(), null ,Color.White);
            Utils.DrawBorderString(spriteBatch, starBook_Event.Event, rectangle.TopLeft(), Color.White,0.8f, starBook_Event.Event.Length * -0.1f);
        }
    }
}
