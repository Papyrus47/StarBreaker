using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace StarBreaker.StarUI
{
    public class StarChargeUIElement : UIElement
    {
        private int _maxValue, _value;
        //设置构造进度条要输入maxValue和value参数
        public StarChargeUIElement(int maxValue, int value)
        {
            _maxValue = maxValue;
            _value = value;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();//ui左上角
            float progress = (float)(_value / (float)_maxValue);//百分比
            spriteBatch.Draw(ModContent.Request<Texture2D>("StarBreaker/StarUI/StarChargeUI").Value, new Rectangle((int)pos.X - 11, (int)pos.Y - 15, 48, 22), Color.White);//绘制外边
            spriteBatch.Draw(ModContent.Request<Texture2D>("StarBreaker/StarUI/StarChargeUILine").Value, new Rectangle((int)pos.X - 4, (int)pos.Y - 4, (int)(32 * progress), 6), Color.White);//绘制里面
            base.Draw(spriteBatch);
        }
        public void SetValue(int maxValue, int value)
        {
            _maxValue = maxValue;
            _value = value;
        }
    }
    public class StarChargeUIState : UIState
    {
        private StarChargeUIElement starCharge;
        public override void OnInitialize()
        {
            starCharge = new(0, 0);
            starCharge.Width.Set(48, 0);
            starCharge.Height.Set(22, 0);
            starCharge.Top.Set(600,0);
            starCharge.Left.Set(Main.screenWidth / 3f, 0);
            Append(starCharge);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<Items.Weapon.StarBreakerW>())
            {
                return;
            }
            starCharge.SetValue(100, Main.LocalPlayer.GetModPlayer<StarPlayer>().StarCharge);
            base.Draw(spriteBatch);
        }
    }
}
