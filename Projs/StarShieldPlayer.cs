using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarBreaker.Items.Weapon;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class StarShieldPlayer : ModProjectile
    {
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰护盾");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 30000;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = 50;
            Projectile.width = 50;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.hide = false;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player != null)
            {
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * 50;
                Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation() + MathHelper.PiOver2;
                if (!player.active || player.dead)
                {
                    player.GetModPlayer<StarPlayer>().SummonStarShieldTime = 0;
                    Projectile.Kill();
                    return;
                }
                else
                {
                    foreach (Projectile target in Main.projectile)
                    {
                        if (target.hostile && !target.friendly && target.active && !target.minion &&
                            target.type != Projectile.type && target.type != ModContent.ProjectileType<StarShield>())
                        {
                            if (target.Colliding(target.Hitbox, Projectile.Hitbox))
                            {
                                if (Projectile.timeLeft > 30000)
                                {
                                    int ProjTime = Projectile.timeLeft - 30000;
                                    Projectile.timeLeft = 30000;
                                    player.statLife += ProjTime;
                                    if (player.statLife > player.statLifeMax2) player.statLife = player.statLifeMax2;
                                }

                                int da = target.damage - (int)(player.statDefense * (Main.masterMode ? 1 : Main.expertMode ? 0.75f : 0.5f));
                                if (Main.masterMode) da *= 6;
                                else if (Main.expertMode) da *= 4;
                                da *= 3;
                                da = Math.Abs(da);
                                Projectile.timeLeft -= da;
                                CombatText.NewText(Projectile.Hitbox, Color.MediumVioletRed, da);
                                player.statLife += da;
                                if (player.statLife > player.statLifeMax2) player.statLife = player.statLifeMax2;
                                if (target.Distance(Projectile.Center) < 100) target.Kill();
                                if (player.HeldItem.type != ModContent.ItemType<StarBreakerW>())
                                {
                                    Projectile.Kill();
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
                new Color(0.2f, 0.2f, 1f, Math.Min(Projectile.timeLeft / 30000, 1)),
                Projectile.rotation,
                DrawOrigin,
                new Vector2(10, 2f),
                SpriteEffects.None,
                0f);
            return true;
        }
    }
}
