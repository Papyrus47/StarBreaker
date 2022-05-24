using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace StarBreaker.Items.Weapon.DoomFight
{
    public class VolleySword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "凌空之剑");
        }
        public override void SetDefaults()
        {
            Item.damage = 72;
            Item.knockBack = 1.2f;
            Item.useTime = Item.useAnimation = 20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.width = Item.height = 66;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VolleySwordProj>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
    public class VolleySwordProj : ModProjectile
    {
        private Vector2[] oldVels;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Volley sword");
            DisplayName.AddTranslation(7, "凌空之剑");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 5;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 66;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            oldVels = new Vector2[60];
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Projectile.timeLeft = 5;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.velocity.Y,
                Projectile.velocity.X * Projectile.velocity.X);
            player.ChangeDir(Projectile.direction);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
            switch(Projectile.ai[0])
            {
                case < 3://挥舞
                    {
                        if(Math.Abs(Projectile.ai[1]) > 7f)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[0]++;
                            oldVels = new Vector2[60];
                        }
                        else if(Projectile.ai[1] == 0)
                        {
                            Projectile.ai[1] = -MathHelper.PiOver4 * 0.2f * (Projectile.ai[0] % 2 == 0 ? 1 : -1);
                            if(Main.myPlayer == player.whoAmI)
                            {
                                Projectile.localAI[0] = (Main.MouseWorld - Projectile.Center).ToRotation();
                            }
                        }
                        else
                        {
                            Projectile.ai[1] += (float)Math.Tan(-0.25f * (Projectile.ai[0] % 2 == 0 ? 1 : -1)) * 0.15f;
                        }
                        Vector2 pos = Projectile.Center + Projectile.ai[1].ToRotationVector2() * Projectile.width * 2;
                        Projectile.velocity = pos - Projectile.Center;
                        Projectile.velocity.Y *= 0.1f;
                        Projectile.velocity.X *= 0.3f;
                        Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.localAI[0]);
                        break;
                    }
                case 3://举起
                    {
                        Projectile.Kill();
                        break;
                    }
                default:
                    {
                        Projectile.Kill();
                        break;
                    }
            }
            for (int i = oldVels.Length - 1; i > 0; i--)
            {
                oldVels[i] = oldVels[i - 1];
            }
            oldVels[0] = Projectile.velocity;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 scale = Vector2.One + (new Vector2(Projectile.velocity.X, Projectile.velocity.Y) / texture.Size());
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White,
                Projectile.rotation, new Vector2(5, texture.Height - 10), scale, 0, 0);

            #region 顶点绘制
            List<CustomVertexInfo> bars = new();
            for(int i = 1;i<oldVels.Length;i++)
            {
                if (oldVels[i] == Vector2.Zero) continue;
                Vector2 vel = oldVels[i - 1] - oldVels[i];
                vel = vel.NormalVector();
                var factor = i / (float)oldVels.Length;
                var color = Color.Lerp(Color.AliceBlue, Color.Blue,factor);
                var w = MathHelper.Lerp(0.5f, 0.05f, factor);
                bars.Add(new(Projectile.Center + oldVels[i] + vel.RealSafeNormalize() * Projectile.width * 1.4f, color, new Vector3((float)Math.Sqrt(factor), 1,w)));
                bars.Add(new(Projectile.Center + oldVels[i] + vel.RealSafeNormalize() * Projectile.width * 0.2f, color, new Vector3((float)Math.Sqrt(factor), 0,w)));
            }
            if(bars.Count > 2)
            {
                List<CustomVertexInfo> triangleList = new();
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    triangleList.Add(bars[i]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 1]);

                    triangleList.Add(bars[i + 1]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 3]);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

                Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("StarBreaker/Images/MyExtra_2").Value;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                StarBreaker.UseSwordShader.Parameters["uTransform"].SetValue(model * projection);
                StarBreaker.UseSwordShader.CurrentTechnique.Passes[0].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin();
            }
            #endregion
            return false;
        }
    }
}
