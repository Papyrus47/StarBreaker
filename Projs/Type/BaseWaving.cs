using Microsoft.Xna.Framework;
using StarBreaker.Items.UltimateCopperShortsword;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent;
using StarBreaker.Items.Weapon;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StarBreaker.Projs.Type
{
    public abstract class BaseWaving : ModProjectile
    {
        /// <summary>
        /// 椭圆的离心率
        /// </summary>
        protected Vector2 Eccentricity = Vector2.One;
        /// <summary>
        /// 椭圆的长度
        /// </summary>
        protected float Length = 10f;
        /// <summary>
        /// 时间
        /// </summary>
        protected float Time;
        protected Player Player => Main.player[Projectile.owner];
        protected Vector2[] OldVel;
        /// <summary>
        /// 从左下角做起点的Origin
        /// </summary>
        public virtual Vector2 Origin => Vector2.Zero;
        /// <summary>
        /// 缩放上限,如果null就是没有上限
        /// </summary>
        public virtual float? MaxDrawScale => 1.5f;
        /// <summary>
        /// Draw贴图
        /// </summary>
        public virtual Texture2D DrawWavingTex => ModContent.Request<Texture2D>("StarBreaker/Images/MyExtra_1").Value;
        /// <summary>
        /// 调用自己的顶点绘制与储存
        /// </summary>
        /// <returns>返回true调用基类</returns>
        public virtual bool PreVertex() { return true; }
        /// <summary>
        /// 在顶点绘制后的Draw
        /// </summary>
        public virtual void PostVertex() { }
        public abstract void OldVelInit();
        public virtual bool PreWavingAI() { return true; }
        public virtual void PostWavingAI() { }
        /// <summary>
        /// 自定义顶点算法
        /// </summary>
        public virtual void Vertex(Vector2[] oldVels,int index,ref Color color, List<CustomVertexInfo> customs) { }
        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }
        public sealed override void AI()//上锁的AI,可以用两个重写函数控制内部ai
        {
            if(PreWavingAI())
            {
                if(OldVel == null)
                {
                    OldVelInit();
                }
                Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter);//修改中心位置

                Vector2 pos = Time.ToRotationVector2() * Length * Eccentricity;//椭圆
                Projectile.velocity = pos - Projectile.Center;//修改位置

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;//修改旋转角度
                Player.itemRotation = Projectile.rotation - MathHelper.PiOver4;//修改手臂朝向
            }
            if(OldVel != null)//储存旧位置
            {
                for(int i = OldVel.Length - 1;i > 0; i--)
                {
                    OldVel[i] = OldVel[i - 1];
                }
                OldVel[0] = Projectile.velocity;
            }
            PostWavingAI();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Projectile.velocity, Projectile.width / 2,ref r);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 scale = Projectile.velocity.AbsVector2();
            if (MaxDrawScale.HasValue)
            {
                if (scale.X > MaxDrawScale.Value)
                {
                    scale.X = MaxDrawScale.Value;
                }
                if (scale.Y > MaxDrawScale.Value)
                {
                    scale.Y = MaxDrawScale.Value;
                }
            }
            sb.Draw(texture,Projectile.Center - Main.screenPosition, null, lightColor,
                Projectile.rotation,Origin,scale,SpriteEffects.None,0f);

            if (OldVel != null)
            {
                if (PreVertex())
                {
                    List<CustomVertexInfo> customs = new();
                    texture = DrawWavingTex;
                    for (int i = 1; i < Projectile.oldPos.Length; ++i)//取顶点
                    {
                        if (Projectile.oldPos[i] == Vector2.Zero) break;
                        Vertex(OldVel, i, ref lightColor, customs);
                    }
                    if (customs.Count > 2)//真正开始连接顶点
                    {
                        List<CustomVertexInfo> customs2 = new();
                        for (int i = 0; i < customs.Count - 2; i += 2)//绘制三角形顶点连接
                        {
                            customs2.Add(customs[i]);//这是四边形形成的第一个三角形
                            customs2.Add(customs[i + 2]);
                            customs2.Add(customs[i + 1]);

                            customs2.Add(customs[i + 1]);//这是第二个
                            customs2.Add(customs[i + 2]);
                            customs2.Add(customs[i + 3]);
                        }
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);//修改begin
                        RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

                        Main.graphics.GraphicsDevice.Textures[0] = texture;
                        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, customs2.ToArray(), 0, customs2.Count / 3);

                        Main.graphics.GraphicsDevice.RasterizerState = originalState;
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin();
                    }
                }
            }
            PostVertex();
            return false;
        }
    }
}
