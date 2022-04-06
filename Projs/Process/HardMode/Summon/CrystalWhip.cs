using Microsoft.Xna.Framework;
using StarBreaker.Projs.Type;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace StarBreaker.Projs.Process.HardMode.Summon
{
    internal class CrystalWhip : BaseWhip
    {
        public override string Texture => "StarBreaker/Projs/UltimateCopperShortsword/ItemProj/LastCopperWhipProj";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Crystal Whip");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "水晶鞭");
        }
        public override void PostWhipAI()
        {
            Projectile.FillWhipControlPoints(Projectile, ListVector2);
            if (Projectile.ai[0] % 2 == 0)
            {
                Vector2 pos = ListVector2[^1];//获取鞭子顶点位置
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(null, pos, Main.rand.NextVector2Unit() * 10, ProjectileID.CrystalShard,
                            Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    }
                }
                Dust dust = Dust.NewDustDirect(pos, 3, 3, DustID.CrystalPulse);
                dust.velocity *= 2;
                dust.scale *= 1.5f;
                dust.noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            Projectile.damage = (int)(Projectile.damage * 0.2f);
        }
    }
}
