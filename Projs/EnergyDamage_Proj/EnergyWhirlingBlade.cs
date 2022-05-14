using Microsoft.Xna.Framework;
using StarBreaker.Projs.Type;
using Terraria;
using Terraria.Localization;

namespace StarBreaker.Projs.EnergyDamage_Proj
{
    public class EnergyWhirlingBlade : EnergyProj
    {
        public override string Texture => (GetType().Namespace + "." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "能量回旋刃");
        }
        public override void NewSetDef()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 150;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.width = 80;
            Projectile.height = 90;
            Projectile.alpha = 0;
            DrawBulletBody = null;
        }
        public override void StateAI()
        {
            Projectile.rotation = Projectile.timeLeft / -5.2f;
            if (Projectile.timeLeft <= 75)//通过简单的if语句判断
            {
                Projectile.velocity = ((Main.player[Projectile.owner].Center - Projectile.Center).SafeNormalize(default) * 20 + Projectile.velocity * 5) / 6;
                if (Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center) < 50)
                {
                    Projectile.Kill();
                }
            }
            else if (Projectile.timeLeft % 10 == 0)
            {
                Projectile.velocity *= 0.7f;
            }
        }
    }
}
