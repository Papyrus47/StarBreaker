using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace StarBreaker.Tiles
{
    public class StarHardRock : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;//设置是矿物
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 410;//金属探测器
            Main.tileShine2[Type] = true; // 稍微修改绘图颜色
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;


            ModTranslation name = CreateMapEntryName();//名字
            name.SetDefault("Star Hard Rock");
            name.AddTranslation((int)GameCulture.CultureName.Chinese, "星辰硬石");
            AddMapEntry(Color.Purple, name);

            MinPick = 200;//最低稿力
            ItemDrop = ModContent.ItemType<Items.ItemTiles.StarHardRock>();
            SoundType = SoundID.Tink;
            SoundStyle = 1;
        }
    }
}
