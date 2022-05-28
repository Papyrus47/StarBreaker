using Terraria.Audio;

namespace StarBreaker.Items
{
    public class Kazudi : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("kazoo");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "卡祖笛");
            Tooltip.SetDefault("Can you really play the kazoo?");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "你真的会吹卡祖笛吗?");
        }
        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
        }
        public override bool? UseItem(Player player)
        {
            try
            {
                int soundID = (int)((Main.MouseWorld - player.Center).Length()) % 5;
                if (soundID > 5)
                {
                    soundID = 5;
                }
                else if (soundID < 1)
                {
                    soundID = 1;
                }

                SoundEngine.PlaySound(SoundLoader.GetSoundSlot("StarBraker/Sounds/Kazoo/kazoo" + soundID.ToString()), (int)player.Center.X, (int)player.Center.Y, 0, 4, 0.1f);
            }
            catch { }
            return base.UseItem(player);
        }
    }
}
