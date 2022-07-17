using ReLogic.Graphics;
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
            if(starBook_Event != null)
            {
                float scaleX = Parent.Width.Pixels / 618;
                float scaleY = Parent.Height.Pixels / 498;
                var dimensinos = Parent.GetDimensions();
                Rectangle rectangle = new((int)(dimensinos.X + dimensinos.Width * (8f / 103f * scaleX)),(int)(dimensinos.Y + dimensinos.Height * (6f / 83f) * scaleY),
                    (int)(Parent.Width.Pixels * (35f / 103f)), (int)(Parent.Height.Pixels * (23f / 83f)));
                spriteBatch.Draw(starBook_Event.Texture, rectangle, null, Color.White, 0f,Vector2.Zero, SpriteEffects.None, 0f);

                Vector2 drawCenter = rectangle.BottomLeft() + new Vector2(5, 30);
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, starBook_Event.Event, drawCenter.X, drawCenter.Y, Color.Yellow, Color.Black, Vector2.Zero,1.2f);
            }
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle rectangle = GetDimensions().ToRectangle();//获取对应的矩阵
            spriteBatch.Draw(ModContent.Request<Texture2D>((GetType().Namespace + "." + GetType().Name).Replace('.', '/')).Value,rectangle.TopLeft(), null ,Color.White,0,Vector2.Zero,1.2f,SpriteEffects.None,0f);
            Utils.DrawBorderString(spriteBatch, starBook_Event.Event, rectangle.TopLeft(), Color.White,1f, starBook_Event.Event.Length * -0.1f);
        }
    }
}
