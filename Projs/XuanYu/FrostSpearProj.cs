namespace StarBreaker.Projs.XuanYu
{
    public class FrostSpearProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Spear");
            DisplayName.AddTranslation(7, "寒霜刺枪");
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.Size = new Vector2(66);
            Projectile.usesLocalNPCImmunity = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];//获取玩家
            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }
            player.itemTime = player.itemAnimation = 2;
            player.heldProj = Projectile.whoAmI;
            switch (Projectile.ai[0])
            {
                case 0://旋转
                    {
                        Projectile.localNPCHitCooldown = 5;
                        Projectile.rotation += 0.35f;
                        if (Projectile.timeLeft % 5 == 0)
                        {
                            Projectile.velocity *= 0.9f;
                        }
                        if (Projectile.velocity.Length() < 5f || (Main.myPlayer == player.whoAmI && Main.mouseLeft && Projectile.timeLeft < 290))
                        {
                            Projectile.ai[0]++;
                        }
                        break;
                    }
                case 1://回归
                    {
                        Projectile.timeLeft = 300;
                        Projectile.rotation += 0.35f;
                        Projectile.velocity = (Projectile.velocity * 6 + (player.Center - Projectile.Center).RealSafeNormalize() * (20 + Projectile.ai[1] * 0.1f)) / 7;
                        Projectile.ai[1]++;
                        if (Projectile.Center.Distance(player.Center) < 33f || Projectile.ai[1] > 120)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[0]++;
                        }
                        break;
                    }
                case 2://抓住冲刺
                    {
                        Projectile.localNPCHitCooldown = -2;
                        Projectile.timeLeft = 300;
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                        if (Main.myPlayer == player.whoAmI)
                        {
                            if (Projectile.ai[1] == 0)
                            {
                                Projectile.damage *= 15;
                            }
                            else if (Projectile.ai[1] == 1)
                            {
                                Projectile.velocity = (Main.MouseWorld - Projectile.Center).RealSafeNormalize() * 60f;
                                for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
                                {
                                    if (i < Projectile.oldPos.Length)
                                    {
                                        Projectile.oldPos[i] = Vector2.Zero;
                                    }
                                    Projectile.localNPCImmunity[i] = 0;
                                }
                            }
                            else
                            {
                                if (Projectile.ai[1] % 5 == 0)
                                {
                                    Projectile.velocity *= 0.9f;
                                }

                                if (Projectile.velocity.Length() < 30)
                                {
                                    player.velocity *= 0.8f;
                                    Projectile.Kill();
                                    return;
                                }
                            }
                            if (Main.myPlayer == player.whoAmI && Main.mouseLeft && Projectile.ai[1] > 20)
                            {
                                if (Projectile.localAI[0] <= 4)
                                {
                                    Projectile.ai[1] = 1;
                                    Projectile.localAI[0]++;
                                }
                                else
                                {
                                    Projectile.Kill();
                                    player.velocity *= 0.2f;
                                }
                                return;
                            }
                            Projectile.ai[1]++;
                            player.MountedCenter = Projectile.Center - Projectile.velocity.RealSafeNormalize() * 30f;
                            player.velocity = Projectile.velocity;
                            player.ChangeDir((player.velocity.X > 0).ToDirectionInt());
                            player.immune = true;
                            player.immuneTime = 5;
                            player.immuneAlpha = 200;
                        }
                        break;
                    }
                default:
                    {
                        Projectile.ai[0] = 0f;
                        break;
                    }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[0] < 2)
            {
                target.GetGlobalNPC<NPCs.StarGlobalNPC>().XuanYuSlowTime = 300;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0]++;
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 33f,
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * -33f,
                8, ref r);
        }
        public override void PostDraw(Color lightColor)
        {
            if (Projectile.ai[0] == 2)
            {
                List<CustomVertexInfo> customs = new();
                Texture2D texture = StarBreakerAssetTexture.MyExtras[1].Value;
                for (int i = 1; i < Projectile.oldPos.Length; ++i)//取顶点
                {
                    if (Projectile.oldPos[i] == Vector2.Zero)
                    {
                        break;
                    }

                    Color color = Color.Blue;
                    color.A = 0;
                    Vector2 norDis = (Projectile.oldPos[i - 1] - Projectile.oldPos[i]).RealSafeNormalize().NormalVector();
                    Vector2 pos = Projectile.oldPos[i] + Projectile.velocity.RealSafeNormalize() * (120 - (i - 1) * 0.3f);
                    float factor = i / Projectile.oldPos.Length;
                    float width;
                    color = Color.Lerp(color, Color.Transparent, factor);
                    float w = MathHelper.Lerp(1f, 0.05f, factor);
                    if (i < 10)
                    {
                        width = i * 5;
                    }
                    else
                    {
                        width = 50;
                    }

                    Vector2 Size = Projectile.Size * 0.5f;
                    customs.Add(new(pos + norDis * width + Size, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
                    customs.Add(new(pos + norDis * -width + Size, color, new Vector3((float)Math.Sqrt(factor), 1, w)));//放置顶点
                }
                if (customs.Count > 2)//真正开始连接顶点
                {
                    var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                    var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

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


                    StarBreaker.EliminateRaysShader.Parameters["uTransform"].SetValue(model * projection);
                    StarBreaker.EliminateRaysShader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);//给shader传参
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);//修改begin
                    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

                    Main.graphics.GraphicsDevice.Textures[0] = texture;
                    Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

                    StarBreaker.EliminateRaysShader.CurrentTechnique.Passes[0].Apply();//调用shader
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, customs2.ToArray(), 0, customs2.Count / 3);

                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin();
                }
            }
        }
    }
}
