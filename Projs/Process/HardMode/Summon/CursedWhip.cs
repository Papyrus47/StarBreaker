using Microsoft.Xna.Framework;
using StarBreaker.Projs.Type;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace StarBreaker.Projs.Process.HardMode.Summon
{
    public class CursedWhip : BaseWhip
    {
        public override string Texture => "StarBreaker/Projs/UltimateCopperShortsword/ItemProj/LastCopperWhipProj";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Cursed Whip");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "诅咒鞭");
        }
        public override void PostWhipAI()
        {
            Projectile.FillWhipControlPoints(Projectile, ListVector2);
            if (Projectile.ai[0] % 2 == 0)
            {
                Vector2 pos = ListVector2[^1];//获取鞭子顶点位置
                Dust dust = Dust.NewDustDirect(pos, 3, 3, DustID.GreenFairy);
                dust.velocity *= 2;
                dust.scale *= 1.5f;
                dust.noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            NPCs.StarGlobalNPC starGlobalNPC = target.GetGlobalNPC<NPCs.StarGlobalNPC>();
            Projectile.damage = (int)(Projectile.damage * 0.7f);
            if (starGlobalNPC != null)
            {
                starGlobalNPC.CursedWhipHit++;
            }
        }
    }
}
