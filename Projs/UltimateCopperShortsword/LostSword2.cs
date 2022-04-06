using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace StarBreaker.Projs.UltimateCopperShortsword
{
    public class LostSword2 : ModProjectile
    {
        private Vector2 oldVec;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Copper Short sword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铜短剑");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.rotation = oldVec.ToRotation() + MathHelper.PiOver4;

            switch (Projectile.ai[0])
            {
                case 0:
                    {
                        Projectile.hide = true;
                        oldVec = Projectile.velocity;
                        Projectile.ai[0] = 1;
                        break;
                    }
                case 1:
                    {
                        Projectile.hide = false;
                        Projectile.rotation = oldVec.ToRotation() + MathHelper.PiOver4;
                        Projectile.velocity *= 0;
                        break;
                    }
                case 2:
                    {
                        Projectile.velocity = oldVec;
                        if (Projectile.velocity == Vector2.Zero)
                        {
                            Projectile.velocity = Vector2.UnitX * 15;
                        }
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                        break;
                    }
                default:
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                        break;
                    }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 8,
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * -8,
                5, ref r);
        }
        public override void PostDraw(Color lightColor)
        {
            if (Projectile.ai[1] == 1 && Projectile.ai[0] != 0)
            {
                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
                Main.spriteBatch.Draw(texture,
                    Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() - Main.screenPosition,
                    null,
                    new Color(0, 255, 0, 100),
                    Projectile.rotation - MathHelper.PiOver4,
                    Vector2.Zero,
                    new Vector2(100, 0.5f),
                    SpriteEffects.None,
                    0);

            }
            else
            {
                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
                for (int i = 1; i < 7; i++)
                {
                    Main.spriteBatch.Draw(texture,
                        Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) / 2f - Main.screenPosition,
                        null,
                        new Color(1f / i, 1 / i, 0.2f, 0f),
                        Projectile.rotation,
                        texture.Size() * 0.5f,
                        1,
                        SpriteEffects.None,
                        0f
                        );
                }
            }
        }
    }
}
