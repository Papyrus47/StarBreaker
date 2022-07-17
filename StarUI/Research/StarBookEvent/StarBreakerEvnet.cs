using StarBreaker.Items.Weapon;

namespace StarBreaker.StarUI.Research.StarBookEvent
{
    internal class StarBreakerEvnet : StarBook_Event
    {
        public override string Event => "怒火 绝望 迷茫(灰烟缭绕解放战)";
        public override string Event_Name => "你自己";
        public override string Event_Show => "星辰击碎者随机游走在星空,据说他的主人疯了";
        public override Texture2D Texture => TextureAssets.MagicPixel.Value; 
    }
}
