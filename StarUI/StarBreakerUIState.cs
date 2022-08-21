using StarBreaker.Items.Bullet;
using StarBreaker.Items.Weapon;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace StarBreaker.StarUI
{
    public class StarBreakerUIState : UIState//UI状态
    {
        public StarBreakerUIElement element1;
        public StarBreakerUIElement element2;
        public UIImageButton button;
        public override void OnInitialize()
        {
            UIPanel panel = new();
            panel.Width.Set(160, 0);
            panel.Height.Set(80, 0);
            //长宽
            panel.Left.Set(-260f, 0.5f);
            panel.Top.Set(-100f, 0.5f);
            //位置
            Append(panel);//将这个面板注册到UIState

            element1 = new(ItemSlot.Context.BankItem, 0.95f, "主动子弹栏;将能量子弹放入其中,装弹后,发射的子弹类型为此填入的能量子弹")
            {
                Left = { Pixels = 50 },
                Top = { Precent = 0.5f, Pixels = -30 },
                ValidItemFunc = item => item.IsAir || (!item.IsAir && item.ammo == ModContent.ItemType<NebulaBulletItem>())
            };
            panel.Append(element1);//向元素注册到面板上
            element2 = new(ItemSlot.Context.BankItem, 0.95f, "被动子弹栏:将能量子弹放入其中,装弹后,发射的子弹将拥有此能量子弹的被动能力")
            {
                Left = { Pixels = 0 },
                Top = { Precent = 0.5f, Pixels = -30 },
                ValidItemFunc = item => item.IsAir || (!item.IsAir && item.ammo == ModContent.ItemType<NebulaBulletItem>())
            };
            panel.Append(element2);
            button = new(ModContent.Request<Texture2D>("StarBreaker/StarUI/StarBreakerUseButton"));
            button.OnClick += Button_OnClick;
            button.OnUpdate += Button_OnUpdate;
            button.OnRightClick += Button_OnMouseDown;

            button.Width.Set(24, 0);
            button.Height.Set(18, 0);
            button.Left.Set(-24, 1);
            button.Top.Set(-18, 0.5f);
            panel.Append(button);
        }

        private void Button_OnUpdate(UIElement affectedElement)
        {
            if (affectedElement.ContainsPoint(Main.MouseScreen))
            {
                UIText buttonText = new("单击装填一发,右键装填所有子弹");
                affectedElement.Append(buttonText);
            }
            else
            {
                affectedElement.RemoveAllChildren();
            }
        }

        private void Button_OnMouseDown(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.LocalPlayer.GetHasItem<StarBreakerW>().ModItem is StarBreakerW item)
            {
                bool flag = false;
                while (item.UseAmmo.Count < 20)
                {
                    if (!ChangeTheBullets(item)) break;//没有子弹就跳出换弹
                    flag = true;
                }
                if (flag)//播放装弹声音
                {
                    var sound = SoundID.Item149.WithPitchOffset(-0.66f);
                    sound.MaxInstances = 3;
                    sound.PitchVariance = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        SoundEngine.PlaySound(sound, Main.LocalPlayer.Center);
                    }
                }
            }
        }

        private void Button_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.LocalPlayer.GetHasItem<StarBreakerW>().ModItem is StarBreakerW item && item.UseAmmo.Count < 20 && ChangeTheBullets(item))
            {
                var sound = SoundID.Item149.WithPitchOffset(-0.66f);
                sound.MaxInstances = 3;
                sound.PitchVariance = 0;
                for (int i = 0; i < 3; i++)
                {
                    SoundEngine.PlaySound(sound, Main.LocalPlayer.Center);
                }
            }
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
        }
        private bool ChangeTheBullets(StarBreakerW item)
        {
            bool flag = false;
            if (element1.Item != null)//如果UI有主弹药
            {
                item.UseAmmo.Add(element1.Item.type);
                element1.Item.ItemStackDeduct();
                if (element2.Item != null)//如果有被动子弹
                {
                    item.UseAmmo.Add(element2.Item.type);
                    element2.Item.ItemStackDeduct();
                }
            }
            else if (element2.Item != null)//如果只有被动子弹
            {
                item.UseAmmo.Add(element2.Item.type);
                element2.Item.ItemStackDeduct();
                item.UseAmmo.Add(0);//添加空子弹
            }
            return flag;
        }
    }
}
