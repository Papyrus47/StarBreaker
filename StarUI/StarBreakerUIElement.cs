using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace StarBreaker.StarUI
{
    public class StarBreakerUIElement : UIElement//UI的一个元素
    {
        internal Item Item;
        private readonly int _context;
        private readonly float _scale;
        private readonly string _text;
        internal Func<Item, bool> ValidItemFunc;
        public StarBreakerUIElement(int context = ItemSlot.Context.BankItem, float scale = 1f, string text = null)
        {
            _context = context;
            _scale = scale;
            Item = new();
            Item.SetDefaults(0);

            Width.Set(TextureAssets.InventoryBack10.Width(), 0f);
            Height.Set(TextureAssets.InventoryBack10.Height(), 0f);
            _text = text;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            RemoveAllChildren();
            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (ValidItemFunc == null || ValidItemFunc(Main.mouseItem))
                {
                    ItemSlot.Handle(ref Item, _context);//使用原版的交互
                }
                UIText buttonText = new(_text);
                Append(buttonText);
            }
            ItemSlot.Draw(spriteBatch, ref Item, _context, rectangle.TopLeft());//绘制物品上去

            Main.inventoryScale = oldScale;
        }
    }
}
