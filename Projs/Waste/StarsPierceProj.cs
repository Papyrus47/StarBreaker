using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Localization;
using System;
using System.Collections.Generic;
using Terraria.ID;

namespace StarBreaker.Projs.Waste
{
    public class StarsPierceProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("繁星刺破");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 300;
            Projectile.noDropItem = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);//修改中心
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            Projectile.timeLeft = 2;
            switch(Projectile.ai[0])
            {
                case 0://蓄力
                    {
                        if(Main.myPlayer == player.whoAmI)//判断主人
                        {
                            Projectile.velocity = (Main.MouseWorld - Projectile.Center).RealSafeNormalize() * 5;//轻微蓄力
                        }
                        if (!player.channel)
                        {
                            if(Projectile.ai[1] >= 3600)//蓄力超过10秒
                            {
                                Projectile.Kill();
                            }
                            else
                            {
                                Projectile.ai[0] = 1;
                                Projectile.ai[1] = 0;
                            }
                        }
                        else
                        {
                            Projectile.ai[1]++;
                        }
                        break;
                    }
                case 1://正常突刺
                    {
                        if (Projectile.velocity.Length() < 100f)
                        {
                            Projectile.velocity = Projectile.velocity.RealSafeNormalize() * (Projectile.velocity.Length() + 40);//变长
                        }
                        else
                        {
                            if (Projectile.ai[1] == 0)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RealSafeNormalize() * 20, ModContent.ProjectileType<StarsPierceProj_Pierce>(),
                                    Projectile.damage, Projectile.knockBack,player.whoAmI);
                            }
                            else if(Projectile.ai[1] > 90)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Projectile.ai[1]++;
                        }
                        break;
                    }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + Projectile.velocity, 20, ref r);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new(11, 40);
            float scale = 1f + (Projectile.velocity.Length() / 100f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor,
                Projectile.rotation, origin, scale,SpriteEffects.None,0);
            return false;
        }
    }
}

