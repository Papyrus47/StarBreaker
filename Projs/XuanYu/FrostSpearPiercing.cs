using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
namespace StarBreaker.Projs.XuanYu
{
    public class FrostSpearPiercing : ModProjectile
    {
        public override string Texture => "StarBreaker/Images/MyExtra_1";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Spear");
            DisplayName.AddTranslation(7, "寒霜刺枪");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.Size = new Vector2(5);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.alpha = 255;
            Projectile.scale = 0;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 0)
            {
                Projectile.scale += 0.1f;
                if(Projectile.scale > 1f)
                {
                    Projectile.ai[0]++;
                }
            }
            else
            {
                if(Projectile.ai[0] < 10) Projectile.ai[0]++;
                else if(Projectile.scale > 0)
                {
                    Projectile.scale -= 0.05f;
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            if (Projectile.scale < 0.1f) return false;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + Projectile.velocity.RealSafeNormalize() * 800f,
                Projectile.Center + Projectile.velocity.RealSafeNormalize() * -800f,
                50 * Projectile.scale, ref r);
        }
        public override bool ShouldUpdatePosition() => false;
        public override void PostDraw(Color lightColor)
        {
            CustomVertexInfo[] customs = new CustomVertexInfo[6];
            Vector2 vel = Projectile.velocity.RealSafeNormalize().NormalVector();
            const float dis = 800;
            float NormalDis = 50 * Projectile.scale;
            Color color = Color.Blue;
            color.A = 0;
            customs[0] = customs[3] = new(Projectile.Center + Projectile.velocity.RealSafeNormalize() * dis,color,new Vector3(0,0,0));
            customs[1] = customs[4] = new(Projectile.Center + Projectile.velocity.RealSafeNormalize() * -dis, color, new Vector3(1,1, 0));
            customs[2] = new(Projectile.Center + vel * NormalDis, color, new Vector3(1, 0, 0));
            customs[5] = new(Projectile.Center + vel * -NormalDis, color, new Vector3(0, 1, 0));

            for(int i = 0;i<6;i++)
            {
                customs[i].Position = customs[i].Position - Main.screenPosition;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap,
               DepthStencilState.Default, RasterizerState.CullNone);

            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Type].Value;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                customs, 0, customs.Length / 3);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
        }
    }
}
