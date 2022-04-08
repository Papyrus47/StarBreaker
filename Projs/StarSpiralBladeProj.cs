using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class StarSpiralBladeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰旋刃");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 1000;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.width = Projectile.height = 80;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.localAI[0];
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[0] += 0.1f;
                if (Projectile.localAI[0] > 30)
                {
                    Projectile.localAI[1]++;
                }
            }
            else
            {
                Projectile.localAI[0] -= 0.1f;
                if (Projectile.localAI[0] < -30)
                {
                    Projectile.localAI[1]--;
                }
            }
            Projectile.damage = Projectile.originalDamage + (int)Math.Abs(Projectile.localAI[0] * 5);
            Player player = Main.player[Projectile.owner];
            switch (Projectile.ai[0])
            {
                case 0://左键使用
                    {
                        if (!player.active)
                        {
                            Projectile.active = false;
                            return;
                        }
                        if (!player.channel)
                        {
                            Projectile.Kill();
                        }
                        else
                        {
                            Projectile.timeLeft = 2;
                        }
                        Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;
                        if (Projectile.spriteDirection == -1)
                        {
                            Projectile.rotation += MathHelper.Pi;
                        }
                        Projectile.timeLeft = 2;
                        player.ChangeDir(Projectile.direction);
                        player.heldProj = Projectile.whoAmI;
                        player.itemTime = 2;
                        player.itemAnimation = 2;
                        player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction,
                            Projectile.velocity.X * Projectile.direction);
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.velocity = (Main.MouseWorld - Projectile.Center) * 0.2f;
                        }
                        break;
                    }
                case 1://右键使用
                    {
                        if (Projectile.OwnerMinionAttackTargetNPC == null || !Projectile.OwnerMinionAttackTargetNPC.active || !Projectile.OwnerMinionAttackTargetNPC.CanBeChasedBy())
                        {
                            float max = 3000;
                            foreach (NPC npc in Main.npc)
                            {
                                float dis = npc.position.Distance(Projectile.position);
                                if (npc.active && npc.CanBeChasedBy() && !npc.friendly && dis < max)
                                {
                                    player.MinionAttackTargetNPC = npc.whoAmI;
                                    max = dis;

                                }
                            }
                        }
                        else
                        {
                            Vector2 toTarget = Projectile.OwnerMinionAttackTargetNPC.position - Projectile.position;
                            float vel = toTarget.Length() > 50 ? toTarget.Length() * 0.1f : 20;
                            Projectile.velocity = toTarget.SafeNormalize(default) * vel;
                            Projectile.timeLeft = 1000;
                        }
                        if (Projectile.timeLeft < 800 || (player.whoAmI == Main.myPlayer && Main.mouseLeft))
                        {
                            Projectile.ai[0] = 2;
                        }
                        break;
                    }
                case 2://星辰旋刃-回来
                    {
                        Projectile.velocity = (player.Center - Projectile.Center).RealSafeNormalize() * 30;
                        if(Vector2.Distance(player.Center,Projectile.Center) < 50)
                        {
                            Projectile.Kill();
                        }
                        break;
                    }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.immortal) return;
            target.GetGlobalNPC<NPCs.StarGlobalNPC>().StarSpiralBladeProj = Projectile.whoAmI;
            if (Projectile.localAI[1] == 0)
            {
                int dama = (int)((damage * 1.5f) + Math.Abs(Projectile.localAI[0] * 10) - target.defense);
                if (dama <= 0) dama = 1;
                target.life -= dama;
                Main.player[Projectile.owner].dpsDamage += dama;
                target.HitEffect(0, 10);
                target.checkDead();
                CombatText.NewText(target.Hitbox, Color.Purple, dama);
                if (Main.netMode == NetmodeID.Server) NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, target.whoAmI, dama);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int dama = (int)((damage * 0.2f) + Math.Abs(Projectile.localAI[0] * 10) - target.defense);
                    if (dama <= 0) dama = 1;
                    target.life -= dama;
                    Main.player[Projectile.owner].dpsDamage += dama;
                    CombatText.NewText(target.Hitbox, Color.Purple, dama);
                    if (Main.netMode == NetmodeID.Server) NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, target.whoAmI, dama);
                }
                target.HitEffect(0, 10);
                target.checkDead();
            }
        }
        //public override bool PreDraw(ref Color lightColor)
        //{
        //    List<CustomVertexInfo> customs = new List<CustomVertexInfo>();

        //    for (int i = 1; i < Projectile.oldPos.Length; i++)
        //    {
        //        if (Projectile.oldPos[i] == Vector2.Zero) continue;

        //        Vector2 normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];//这一个位置距离上一个位置的朝向
        //        normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));//逆时针旋转90度

        //        float factor = i / (float)Projectile.oldPos.Length;//插值
        //        Color color = Color.Lerp(Color.White, Color.Red, factor);//颜色
        //        float w = MathHelper.Lerp(1f, 0.05f, factor);//透明度获取
        //        customs.Add(new CustomVertexInfo(Projectile.oldPos[i] + new Vector2(Projectile.width / 2,Projectile.height / 2) + normalDir * 80, color, new Vector3((float)Math.Sqrt(factor), 1, w)));//为顶点赋值
        //        customs.Add(new CustomVertexInfo(Projectile.oldPos[i] + new Vector2(Projectile.width / 2, Projectile.height / 2) + normalDir * -80, color, new Vector3((float)Math.Sqrt(factor), 0, w)));//为顶点赋值
        //    }

        //    List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();

        //    if (customs.Count > 2)
        //    {
        //        triangleList.Add(customs[0]);//底边第一个点
        //        CustomVertexInfo vertex = new CustomVertexInfo((customs[0].Position + customs[1].Position) * 0.5f + Vector2.Normalize(Projectile.velocity) * 40, Color.White,
        //            new Vector3(0, 0.5f, 1));//底边两个点取位置的中心,加上弹幕速度为顶点
        //        triangleList.Add(customs[1]);//第二个
        //        triangleList.Add(vertex);//顶点
        //        for (int i = 0; i < customs.Count - 2; i += 2)//取其他的点
        //        {
        //            #region 生成一个由两个三角形组合的四边形
        //            #region 这个是左边(放水平的时候)的三角形连接
        //            triangleList.Add(customs[i]);
        //            triangleList.Add(customs[i + 2]);
        //            triangleList.Add(customs[i + 1]);
        //            #endregion
        //            #region 这个是右边
        //            triangleList.Add(customs[i + 1]);
        //            triangleList.Add(customs[i + 2]);
        //            triangleList.Add(customs[i + 3]);
        //            #endregion
        //            #endregion
        //        }

        //        var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
        //        var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

        //        StarBreaker.TheDrawEffect.Parameters["uTransform"].SetValue(model * projection);
        //        StarBreaker.TheDrawEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
        //        Main.spriteBatch.End();
        //        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap,
        //            DepthStencilState.Default, RasterizerState.CullNone);

        //        RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

        //        Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("StarBreaker/Images/Extra_189").Value;
        //        Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("StarBreaker/Images/BlackColor").Value;

        //        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;

        //        StarBreaker.TheDrawEffect.CurrentTechnique.Passes[0].Apply();

        //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
        //            triangleList.ToArray(), 0, triangleList.Count / 3);

        //        triangleList.Count / 3 是三角形的个数

        //        Main.graphics.GraphicsDevice.RasterizerState = originalState;
        //        Main.spriteBatch.End();
        //        Main.spriteBatch.Begin();
        //    }
        //    return base.PreDraw(ref lightColor);
        //}
    }
}
