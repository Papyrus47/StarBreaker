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
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            oldVels = new Vector2[30];
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
            switch (Projectile.ai[0])
            {
                case < 3://挥舞
                    {
                        if (Math.Abs(Projectile.ai[1]) > 3f)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[0]++;
                            oldVels = new Vector2[30];
                        }
                        else if (Projectile.ai[1] == 0)
                        {
                            NPC n = null;
                            if (!player.HasMinionAttackTargetNPC)
                            {
                                float max = 2000;
                                foreach (NPC npc in Main.npc)
                                {
                                    float dis = Vector2.Distance(npc.Center, Projectile.Center);
                                    if (dis < max && npc.active && npc.CanBeChasedBy() && !npc.friendly)
                                    {
                                        max = dis;
                                        n = npc;
                                    }
                                }
                            }
                            else
                            {
                                n = Projectile.OwnerMinionAttackTargetNPC;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient && n != null)
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    Vector2 center = n.Center + Main.rand.NextVector2Unit() * 300;
                                    Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), center, Vector2.Zero,
                                        ModContent.ProjectileType<VolleySword_SmallLine>(), Projectile.damage, Projectile.knockBack, player.whoAmI, n.whoAmI);
                                }
                            }
                            Projectile.ai[1] = MathHelper.PiOver2 * (Projectile.ai[0] % 2 == 0 ? 1 : -1);
                            if (Main.myPlayer == player.whoAmI)
                            {
                                Projectile.localAI[0] = (Main.MouseWorld - Projectile.Center).ToRotation() + Main.rand.NextFloatDirection() * 0.5f;
                            }
                        }
                        else
                        {
                            Projectile.ai[1] += (float)Math.Sin(-0.25f * (Projectile.ai[0] % 2 == 0 ? 1 : -1)) * 0.7f;
                        }
                        Vector2 pos = Projectile.Center + Projectile.ai[1].ToRotationVector2() * Projectile.width * 2.5f;
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
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Projectile.velocity * 5f, Projectile.width / 2, ref Projectile.localAI[1]);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 scale = new Vector2(Projectile.velocity.X, Projectile.velocity.Y) / texture.Size() + Vector2.One;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White,
                Projectile.rotation, new Vector2(5, texture.Height - 10), scale, 0, 0);

            #region 顶点绘制
            List<CustomVertexInfo> bars = new();
            for (int i = 1; i < oldVels.Length; i++)
            {
                if (oldVels[i] == Vector2.Zero)
                {
                    continue;
                }

                Vector2 vel = oldVels[i - 1] - oldVels[i];
                vel = vel.NormalVector();
                var factor = i / (float)oldVels.Length;
                var color = Color.Lerp(Color.AliceBlue * 0.7f, Color.Blue, factor);
                var w = MathHelper.Lerp(0.5f, 0.05f, factor);
                bars.Add(new(Projectile.Center + (oldVels[i] + vel.RealSafeNormalize()) * 5f, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
                bars.Add(new(Projectile.Center + (oldVels[i] + vel.RealSafeNormalize()).RealSafeNormalize() * 5f, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
            }
            if (bars.Count > 2)
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
    public class VolleySword_SmallLine : ModProjectile
    {
        private Vector2[] linePos;
        public override string Texture => "StarBreaker/Images/Extra_49";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Line");
            DisplayName.AddTranslation(7, "线");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 80;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 5;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            linePos = null;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            Projectile.Center = -(npc.Center - Projectile.Center) + npc.Center;
            Projectile.rotation = (Projectile.Center - npc.Center).ToRotation();
            if (!npc.active)
            {
                Projectile.Kill();
            }

            if (Projectile.timeLeft <= 60)
            {
                if (linePos == null)
                {
                    linePos = new Vector2[Main.rand.Next(8, 16)];
                }
                if (Projectile.timeLeft % 5 == 0)
                {
                    linePos[0] = Projectile.Center;
                    linePos[^1] = npc.Center;
                    for (int i = 1; i < linePos.Length - 1; i++)
                    {
                        Vector2 vector = linePos[^1] - linePos[0];
                        vector = vector.RotatedByRandom(0.35);
                        linePos[i] = linePos[i - 1] + vector / linePos.Length;
                    }
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI != (int)Projectile.ai[0] || Projectile.timeLeft > 60)
            {
                return false;
            }
            else
            {
                return null;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.velocity *= 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(0, 0, 255, 180), Projectile.rotation, texture.Size() * 0.5f,
                new Vector2(0.3f, 0.8f) * 0.3f, SpriteEffects.None, 0);
            if (linePos != null)
            {
                CustomVertexInfo[] customs = new CustomVertexInfo[linePos.Length];
                for (int i = 0; i < linePos.Length; i++)
                {
                    customs[i] = new(linePos[i] - Main.screenPosition, Color.LightBlue, new Vector3(0.5f, 0.5f, 0));
                }
                Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.FishingLine.Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip,
                    customs, 0, customs.Length - 1);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
            return false;
        }
    }
}
