using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class StarShield : ModProjectile
    {
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰护盾");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 30000;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.height = 50;
            Projectile.width = 50;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.hide = false;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc != null)
            {
                Projectile.Center = npc.Center + ((npc.rotation +
                    (npc.spriteDirection == -1 ? 0f : MathHelper.Pi)).ToRotationVector2() * 60);
                Projectile.rotation = npc.rotation + MathHelper.PiOver2;
                if (!npc.active)
                {
                    Projectile.Kill();
                    return;
                }
                else
                {
                    int i = 0;
                    foreach (Projectile target in Main.projectile)
                    {
                        if ((target.friendly || !target.hostile)&& target.active && !target.minion &&
                            target.type != Projectile.type && target.type != ModContent.ProjectileType<StarShieldPlayer>())
                        {
                            if (target.Colliding(target.Hitbox, Projectile.Hitbox))
                            {
                                target.Kill();
                                Projectile.timeLeft -= target.damage - (int)(npc.defense * (Main.expertMode ? 0.75f : 0.5f));
                                npc.life += target.damage / 2;
                                if (npc.life > npc.lifeMax) npc.life = npc.lifeMax;
                                i++;
                                if (i > 3)
                                {
                                    i = 0;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 DrawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width / 2, (float)TextureAssets.Projectile[Projectile.type].Value.Height / 2);
            Vector2 center = Projectile.position + (new Vector2(Projectile.width, Projectile.height) / 2) - Main.screenPosition;
            center -= new Vector2(tex.Width, tex.Height) * Projectile.scale / 2f;
            center += DrawOrigin * Projectile.scale + new Vector2(0f, 4f + Projectile.gfxOffY);
            Main.spriteBatch.Draw(tex,
                center,
                null,
                new Color(0.2f, 0.2f, 1f, (30000 - Projectile.timeLeft) / 30000),
                Projectile.rotation,
                DrawOrigin,
                new Vector2(10, 2f),
                SpriteEffects.None,
                0f);
            return true;
        }
    }
}
