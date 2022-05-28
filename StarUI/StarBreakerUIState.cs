using StarBreaker.Items.Bullet;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace StarBreaker.StarUI
{
    public class StarBreakerUIState : UIState//UI状态
    {
        public StarBreakerUIElement element1;
        public StarBreakerUIElement element2;
        public override void OnInitialize()
        {
            UIPanel panel = new();
            panel.Width.Set(80, 0);
            panel.Height.Set(120, 0);
            //长宽
            panel.Left.Set(-260f, 0.5f);
            panel.Top.Set(-100f, 0.5f);
            //位置

            Append(panel);//将这个面板注册到UIState

            element1 = new(ItemSlot.Context.BankItem, 0.95f)
            {
                Left = { Pixels = 0 },
                Top = { Pixels = 0 },
                ValidItemFunc = item => item.IsAir || !item.IsAir && item.ammo == ModContent.ItemType<NebulaBulletItem>()
            };
            panel.Append(element1);//向元素注册到面板上
            element2 = new(ItemSlot.Context.BankItem, 0.95f)
            {
                Left = { Pixels = 0 },
                Top = { Pixels = 50 },
                ValidItemFunc = item => item.IsAir || !item.IsAir && item.ammo == ModContent.ItemType<NebulaBulletItem>()
            };
            panel.Append(element2);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Main.playerInventory || !Main.LocalPlayer.GetModPlayer<StarPlayer>().SummonStarWenpon)
            {
                return;
            }
            base.Draw(spriteBatch);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            StarPlayer player = Main.LocalPlayer.GetModPlayer<StarPlayer>();
            player.Bullet1 = element1.Item;
            player.Bullet2 = element2.Item;
        }
    }
}
