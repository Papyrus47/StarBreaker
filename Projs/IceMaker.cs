using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class IceMaker : ModProjectile
    {
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("冰柱");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.damage = 0;
            Projectile.width = Projectile.height = 10;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            StarPlayer StarPlayer = player.GetModPlayer<StarPlayer>();
            Projectile.velocity *= 0.9f;
            if (!StarPlayer.InIdeaDriven)
            {
                StarPlayer.InIdeaDriven = true;
            }
            if (player.HeldItem.type != ModContent.ItemType<Items.Weapon.FrostFistW>())
            {
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();//通过end结束上面的绘制
            Main.spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.Additive,SamplerState.PointWrap,
                DepthStencilState.None,RasterizerState.CullNone,null);//开始自己的绘制,让它一次性绘制激光
            Texture2D texture = ModContent.Request<Texture2D>("StarBreaker/Projs/IceThorn").Value;
            Main.spriteBatch.Draw(texture,Projectile.Center - Main.screenPosition, new Rectangle(0,0,texture.Width * 2,120), Color.White);
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
    }
}
