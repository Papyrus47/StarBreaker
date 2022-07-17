using StarBreaker.Projs.Type;

namespace StarBreaker.Projs.XuanYu.Boss
{
    public class BuleIceSpikes : BaseThistle
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bule Ice");
            DisplayName.AddTranslation(7, "蓝冰");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 600;
            Projectile.width = 190;
            Projectile.height = 36;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);
    }
}
